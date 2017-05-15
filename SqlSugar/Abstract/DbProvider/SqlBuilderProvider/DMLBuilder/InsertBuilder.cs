using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar
{
    public class InsertBuilder : IDMLBuilder
    {
        public InsertBuilder()
        {
            this.sql = new StringBuilder();
            this.DbColumnInfoList = new List<DbColumnInfo>();
        }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public string TableName { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public bool IsInsertNull { get; set; }
        public bool IsReturnIdentity { get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                if (IsReturnIdentity)
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;SELECT SCOPE_IDENTITY();";
                }
                else
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;";

                }
            }
        }

        public virtual string SqlTemplateBatch {
            get {
                return "INSERT {0} ({1})";
            }
        }

        public virtual string SqlTemplateBatchSelect {
            get {
                return "N'{0}' AS {1}";
            }
        }

        public virtual string SqlTemplateBatchUnion
        {
            get
            {
                return "\r\nUNION ALL ";
            }
        }

        public virtual void Clear()
        {

        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(TableName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                return result;
            }
        }
        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters = new List<SugarParameter>();
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }
        public virtual string ToSqlString()
        {
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.GetTranslationColumnName(it.ColumnName)));
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.SqlParameterKeyWord + it.ColumnName));
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else {
                StringBuilder batchInsetrSql = new StringBuilder();
                batchInsetrSql.AppendFormat(GetTableNameString, columnsString);
                int i = 0;
                foreach (var columns in groupList)
                {
                    var isFirst = i == 0;
                    if (!isFirst) {
                        batchInsetrSql.Append(SqlTemplateBatchUnion);
                    }
                    batchInsetrSql.Append("\r\n SELECT "+string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect,it.ColumnName,it.Value))));
                    ++i;  
                }
                return batchInsetrSql.ToString();
            }
        }
    }
}
