using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

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
            this.Parameters = new List<SugarParameter>();
        }
        public SqlSugarProvider Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public string TableName { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public List<string> WhereValues { get; set; }
        public List<KeyValuePair<string, string>> SetValues { get; set; }
        public bool IsNoUpdateNull { get; set; }
        public bool IsNoUpdateDefaultValue { get; set; }
        public List<string> PrimaryKeys { get; set; }
        public bool IsOffIdentity { get; set; }
        public bool IsWhereColumns { get; set; }

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
                return @"UPDATE S SET {0} FROM {1} S {2}   INNER JOIN ";
            }
        }

        public virtual string SqlTemplateJoin
        {
            get
            {
                return @"            (
              {0}

            ) T ON {1}
                ; ";
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
                return "\t\t\r\nUNION ALL ";
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
                result += UtilConstants.Space;
                if (this.TableWithString.HasValue())
                {
                    result += TableWithString + UtilConstants.Space;
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
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                resolveExpress.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                resolveExpress.PgSqlIsAutoToLower = true;
            }
            if (isMapping)
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
                resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
                resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            }
            resolveExpress.InitMappingInfo = Context.InitMappingInfo;
            resolveExpress.RefreshMapping = () =>
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
                resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
                resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            };
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var result = resolveExpress.Result;
            return result;
        }
        public virtual string ToSqlString()
        {
            if (IsNoUpdateNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            if (IsNoUpdateDefaultValue)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value.ObjToString() !=UtilMethods.DefaultForType(it.PropertyType).ObjToString()).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            if (isSingle)
            {
                return ToSingleSqlString(groupList);
            }
            else
            {
                return TomultipleSqlString(groupList);
            }
        }

        protected virtual string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, " Update List<T> need Primary key");
            int pageSize = 200;
            int pageIndex = 1;
            int totalRecord = groupList.Count;
            int pageCount = (totalRecord + pageSize - 1) / pageSize;
            StringBuilder batchUpdateSql = new StringBuilder();
            while (pageCount >= pageIndex)
            {
                StringBuilder updateTable = new StringBuilder();
                string setValues = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
                            {
                                if (SetValues.IsValuable())
                                {
                                    var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                                    if (setValue != null && setValue.Any())
                                    {
                                        return setValue.First().Value;
                                    }
                                }
                                var result = string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(it.DbColumnName));
                                return result;
                            }));
                batchUpdateSql.AppendFormat(SqlTemplateBatch.ToString(), setValues, GetTableNameStringNoWith, TableWithString);
                int i = 0;
                foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                {
                    var isFirst = i == 0;
                    if (!isFirst)
                    {
                        updateTable.Append(SqlTemplateBatchUnion);
                    }
                    updateTable.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), Builder.GetTranslationColumnName(it.DbColumnName)))));
                    ++i;
                }
                pageIndex++;
                updateTable.Append("\r\n");
                string whereString = null;
                if (this.WhereValues.HasValue())
                {
                    foreach (var item in WhereValues)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += Regex.Replace(item,"\\"+this.Builder.SqlTranslationLeft,"S."+ this.Builder.SqlTranslationLeft);
                    }
                }
                if (PrimaryKeys.HasValue())
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(item));
                    }
                }
                batchUpdateSql.AppendFormat(SqlTemplateJoin, updateTable, whereString);
            }
            return batchUpdateSql.ToString();
        }

        protected virtual string ToSingleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            string columnsString = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
            {
                if (SetValues.IsValuable())
                {
                    var setValue = SetValues.Where(sv => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName) || sv.Key == Builder.GetTranslationColumnName(it.PropertyName));
                    if (setValue != null && setValue.Any())
                    {
                        return setValue.First().Value;
                    }
                }
                var result = Builder.GetTranslationColumnName(it.DbColumnName) + "=" + this.Context.Ado.SqlParameterKeyWord + it.DbColumnName;
                return result;
            }));
            string whereString = null;
            if (this.WhereValues.HasValue())
            {
                foreach (var item in WhereValues)
                {
                    var isFirst = whereString == null;
                    whereString += (isFirst ? " WHERE " : " AND ");
                    whereString += item;
                }
            }
            else if (PrimaryKeys.HasValue())
            {
                foreach (var item in PrimaryKeys)
                {
                    var isFirst = whereString == null;
                    whereString += (isFirst ? " WHERE " : " AND ");
                    whereString += Builder.GetTranslationColumnName(item) + "=" + this.Context.Ado.SqlParameterKeyWord + item;
                }
            }
            if (PrimaryKeys.HasValue()&&IsWhereColumns)
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

        public virtual object FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType)
                {
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
                    }
                    return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    string bytesString = "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
                    return bytesString;
                }
                else if (type.IsEnum())
                {
                    return Convert.ToInt64(value);
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
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
