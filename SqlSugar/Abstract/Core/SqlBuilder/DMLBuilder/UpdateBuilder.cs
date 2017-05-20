using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class UpdateBuilder : IDMLBuilder
    {
        public UpdateBuilder()
        {
            this.sql = new StringBuilder();
            this.DbColumnInfoList = new List<DbColumnInfo>();
            this.SetValues = new List<KeyValuePair<string, string>>();
            this.WhereValues = new List<string>();
        }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public string TableName { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public List<string> WhereValues { get; set; }
        public List<KeyValuePair<string, string>> SetValues { get; set; }
        public bool IsUpdateNull { get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                return @"UPDATE {0} SET
           {1} {2}";

            }
        }

        public virtual string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE S SET {{0}} FROM {0} S INNER JOIN 
            (
              { { 1} }

            ) T ON ";
            }
        }

        public virtual string SqlTemplateBatchSet
        {
            get
            {
                return "{0} AS {1}";
            }
        }
        public virtual string SqlTemplateBatchSelect
        {
            get
            {
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
                var result = Builder.GetTranslationTableName(TableName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                return result;
            }
        }

        public List<string> PrimaryKeys { get; internal set; }

        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType, bool isMapping = true)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            if (isMapping)
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
                resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            }
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
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.ColumnName) + "=" + this.Context.Ado.SqlParameterKeyWord + it.ColumnName));
            if (isSingle)
            {
                string whereString = null;
                if (this.WhereValues.IsValuable())
                {
                    foreach (var item in WhereValues)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? " WHERE " : " AND ");
                        whereString += item;
                    }
                }
                else if(PrimaryKeys.IsValuable())
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? " WHERE " : " AND ");
                        whereString += Builder.GetTranslationColumnName(item) + "=" + this.Context.Ado.SqlParameterKeyWord + item;
                    }
                }
                return string.Format(SqlTemplate, GetTableNameString, columnsString, whereString);
            }
            else
            {
                Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, " Update List<T> need Primary key");
                var whereString = string.Join(",", PrimaryKeys.Select(it => string.Format("T.{0}=S.{0}", Builder.GetTranslationColumnName(it))));
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex)
                {
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (!isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), it.ColumnName))));
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append("\r\nGO\r\n");
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
            else
            {
                var type = value.GetType();
                if (type == PubConst.DateType)
                {
                    return "'" + value.ObjToDate().ToString("yyyy-MM-dd hh:mm:ss.ms") + "'";
                }
                else if (type == PubConst.StringType || type == PubConst.ObjType)
                {
                    return "N'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "N'" + value.ToString() + "'";
                }
            }
        }
    }
}
