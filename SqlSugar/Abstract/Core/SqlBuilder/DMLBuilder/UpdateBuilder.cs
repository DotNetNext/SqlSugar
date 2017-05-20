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
        public List<string> PrimaryKeys { get; set; }

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
                return @"UPDATE S SET {0} FROM {1} S {2}   INNER JOIN 
            (
              {{0}}

            ) T ON {{1}}";
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
        public virtual string GetTableNameStringNoWith
        {
            get
            {
                var result = Builder.GetTranslationTableName(TableName);
                return result;
            }
        }
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
            if (isSingle)
            {
                string columnsString = string.Join(",", groupList.First().Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result = Builder.GetTranslationColumnName(it.DbColumnName) + "=" + this.Context.Ado.SqlParameterKeyWord + it.DbColumnName;
                    return result;
                }));
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
                else if (PrimaryKeys.IsValuable())
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
                string setValues = string.Join(",", groupList.First().Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result =string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(it.DbColumnName));
                    return result;
                }));
                StringBuilder batchUpdateSql = new StringBuilder();
                batchUpdateSql.AppendFormat(SqlTemplateBatch,setValues,GetTableNameStringNoWith, TableWithString);
                int pageSize = 200;
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                StringBuilder updateTable = new StringBuilder();
                while (pageCount >= pageIndex)
                {
               
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (!isFirst)
                        {
                            updateTable.Append(SqlTemplateBatchUnion);
                        }
                        updateTable.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), it.DbColumnName))));
                        ++i;
                    }
                    pageIndex++;
                    updateTable.Append("\r\nGO\r\n");
                }
                return string.Format(batchUpdateSql.ToString(), updateTable.ToString(),"");
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
