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
            this.Parameters = new List<SugarParameter>();
            this.DbColumnInfoList = new List<DbColumnInfo>();
        }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public bool IsInsertNull { get; set; }
        public bool IsReturnIdentity { get; set; }
        public EntityInfo EntityInfo { get;  set; }

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
                return "{0} AS {1}";
            }
        }

        public virtual string SqlTemplateBatchUnion
        {
            get
            {
                return "\t\r\nUNION ALL ";
            }
        }

        public virtual void Clear()
        {

        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityInfo.EntityName);
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
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }
        public virtual string ToSqlString()
        {
            if (IsInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.SqlParameterKeyWord + it.DbColumnName));
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex) {
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex-1)*pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (!isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), it.DbColumnName))));
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append("\r\n;\r\n");
                }
                return batchInsetrSql.ToString();
            }
        }

        public object FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else {
                var type = value.GetType();
                if (type == PubConst.DateType)
                {
                    return "'" + value.ObjToDate().ToString("yyyy-MM-dd hh:mm:ss.ms") + "'";
                }
                else if (type == PubConst.StringType|| type == PubConst.ObjType)
                {
                    return "N'" + value.ToString().ToSqlFilter() + "'";
                }
                else {
                    return "N'" + value.ToString() + "'";
                }
            }
        }
    }
}
