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
        public string AppendWhere { get; set; }
        public List<KeyValuePair<string, string>> SetValues { get; set; }
        public bool IsNoUpdateNull { get; set; }
        public bool IsNoUpdateDefaultValue { get; set; }
        public List<string> PrimaryKeys { get; set; }
        public List<string> OldPrimaryKeys { get; set; }
        public bool IsOffIdentity { get; set; }
        public bool IsWhereColumns { get; set; }
        public  bool? IsListUpdate { get; set; }
        public List<string> UpdateColumns { get; set; }
        public List<JoinQueryInfo> JoinInfos { get; set; }
        public  string ShortName { get; set; }
        public Dictionary<string, ReSetValueBySqlExpListModel> ReSetValueBySqlExpList { get;  set; }
        public virtual string ReSetValueBySqlExpListType { get; set; }
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
                resolveExpress.TableEnumIsString = this.Context.CurrentConnectionConfig.MoreSettings.TableEnumIsString;
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
            resolveExpress.SugarContext = new ExpressionOutParameter() { Context = this.Context };
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var result = resolveExpress.Result;
            return result;
        }
        public virtual string ToSqlString()
        {
            if (IsNoUpdateNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null||(it.UpdateServerTime == true ||!string.IsNullOrEmpty(it.UpdateSql))).ToList();
            }
            if (IsNoUpdateDefaultValue)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => {
                    if (it.Value.ObjToString() == "0" && it.PropertyType.IsEnum)
                    {
                        return it.Value.ObjToLong() != UtilMethods.DefaultForType(it.PropertyType).ObjToLong();
                    }
                    else if (it.UpdateServerTime == true || !string.IsNullOrEmpty(it.UpdateSql)) 
                    {
                        return true;
                    }
                    else
                    {
                        return it.Value.ObjToString() != UtilMethods.DefaultForType(it.PropertyType).ObjToString();
                    }

                    }).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            if (isSingle&&this.IsListUpdate==null)
            {
                ActionMinDate();
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
                    else if (JoinInfos!=null&&JoinInfos.Any()) 
                    {
                        setValue = SetValues.Where(sv => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Where(sv => sv.Key == Builder.GetNoTranslationColumnName(it.DbColumnName) || sv.Key == Builder.GetNoTranslationColumnName(it.PropertyName));
                        return Builder.GetTranslationColumnName(this.ShortName)+"."+ setValue.First().Key+"="+ setValue.First().Value ;
                    }
                }
                var result = Builder.GetTranslationColumnName(it.DbColumnName) + "=" + GetDbColumn(it,this.Context.Ado.SqlParameterKeyWord + it.DbColumnName);
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
                if (IsWhereColumns == false)
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? " WHERE " : " AND ");
                        whereString += Builder.GetTranslationColumnName(item) + "=" + this.Context.Ado.SqlParameterKeyWord + item;
                    }
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
            if (this.JoinInfos != null && this.JoinInfos.Any())
            {
                return GetJoinUpdate(columnsString, ref whereString);
            }
            return string.Format(SqlTemplate, GetTableNameString, columnsString, whereString);
        }

        protected virtual string GetJoinUpdate(string columnsString, ref string whereString)
        {
            var tableName = Builder.GetTranslationColumnName(this.TableName);
            this.TableName = Builder.GetTranslationColumnName(this.ShortName);
            var joinString = $" FROM {tableName} {Builder.GetTranslationColumnName(this.ShortName)} ";
            foreach (var item in this.JoinInfos)
            {
                joinString += $"\r\n JOIN {Builder.GetTranslationColumnName(item.TableName)}  {Builder.GetTranslationColumnName(item.ShortName)} ON {item.JoinWhere} ";
            }
            whereString = joinString + "\r\n" + whereString;
            return string.Format(SqlTemplate, GetTableNameString, columnsString, whereString);
        }

        public virtual void ActionMinDate()
        {
            if (this.Parameters != null)
            {
                foreach (var item in this.Parameters)
                {
                    if (item.DbType == System.Data.DbType.Date || item.DbType == System.Data.DbType.DateTime)
                    {
                        if (item.Value != null && item.Value != DBNull.Value)
                        {
                            if (item.Value is DateTime)
                            {
                                if (Convert.ToDateTime(item.Value) == DateTime.MinValue)
                                {
                                    item.Value = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                                }
                            }
                        }
                    }
                }
            }
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
                    if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
                    {
                        date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
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
                    if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                    {
                        return value.ToSqlValue();
                    }
                    else
                    {
                        return Convert.ToInt64(value);
                    }
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return FormatDateTimeOffset(value);
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "N'" + value.ToString().ToSqlFilter() + "'";
                }
                else if (type==UtilConstants.IntType||type==UtilConstants.LongType)
                {
                    return value;
                }
                else if (UtilMethods.IsNumber(type.Name)) 
                {
                    if (value.ObjToString().Contains(","))
                    {
                        return $"'{value}'";
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return "N'" + value.ToString() + "'";
                }
            }
        }

        public virtual string FormatDateTimeOffset(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
        }
        private int GetDbColumnIndex = 0;
        public virtual string GetDbColumn(DbColumnInfo columnInfo, object name)
        {
            if (columnInfo.UpdateServerTime)
            {
                return LambdaExpressions.DbMehtods.GetDate();
            }
            else if (UtilMethods.IsErrorDecimalString()==true) 
            {
                var pname = Builder.SqlParameterKeyWord + "Decimal" + GetDbColumnIndex;
                var p = new SugarParameter(pname, columnInfo.Value);
                this.Parameters.Add(p);
                GetDbColumnIndex++;
                return pname;
            }
            else if (IsListSetExp(columnInfo) || IsSingleSetExp(columnInfo))
            {
                if (this.ReSetValueBySqlExpList[columnInfo.PropertyName].Type == ReSetValueBySqlExpListModelType.List)
                {
                    return Builder.GetTranslationColumnName(columnInfo.DbColumnName) + this.ReSetValueBySqlExpList[columnInfo.PropertyName].Sql + name;
                }
                else
                {
                    return this.ReSetValueBySqlExpList[columnInfo.PropertyName].Sql;
                }
            }
            else if (columnInfo.UpdateSql.HasValue())
            {
                return columnInfo.UpdateSql;
            }
            else if (columnInfo.SqlParameterDbType is Type && (Type)columnInfo.SqlParameterDbType == UtilConstants.SqlConvertType)
            {
                var type = columnInfo.SqlParameterDbType as Type;
                var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(typeof(string));
                var obj = Activator.CreateInstance(type);
                var p = ParameterConverter.Invoke(obj, new object[] { columnInfo.Value, GetDbColumnIndex }) as SugarParameter;
                return p.ParameterName;
            }
            else if (columnInfo.SqlParameterDbType is Type)
            {
                var type = columnInfo.SqlParameterDbType as Type;
                var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyType);
                var obj = Activator.CreateInstance(type);
                var p = ParameterConverter.Invoke(obj, new object[] { columnInfo.Value, GetDbColumnIndex }) as SugarParameter;
                GetDbColumnIndex++;
                //this.Parameters.RemoveAll(it => it.ParameterName == it.ParameterName);
                this.Parameters.Add(p);
                return p.ParameterName;
            }
            else if (columnInfo.PropertyType != null && columnInfo.PropertyType.Name == "TimeOnly" && name != null && !name.ObjToString().StartsWith(Builder.SqlParameterKeyWord))
            {
                var timeSpan = UtilMethods.TimeOnlyToTimeSpan(columnInfo.Value);
                var pname = Builder.SqlParameterKeyWord + columnInfo.DbColumnName + "_ts" + GetDbColumnIndex;
                if (timeSpan == null)
                {
                    this.Parameters.Add(new SugarParameter(pname, null) { DbType = System.Data.DbType.Date });
                }
                else
                {
                    this.Parameters.Add(new SugarParameter(pname, timeSpan));
                }
                GetDbColumnIndex++;
                return pname;
            }
            else if (columnInfo.PropertyType != null && columnInfo.PropertyType.Name == "DateOnly")
            {
                var timeSpan = UtilMethods.DateOnlyToDateTime(columnInfo.Value);
                var pname = Builder.SqlParameterKeyWord + columnInfo.DbColumnName + "_ts" + GetDbColumnIndex;
                if (timeSpan == null)
                {
                    this.Parameters.Add(new SugarParameter(pname, null) { DbType = System.Data.DbType.Date });
                }
                else
                {
                    this.Parameters.Add(new SugarParameter(pname, Convert.ToDateTime(timeSpan)));
                }
                GetDbColumnIndex++;
                return pname;
            }
            else if (UtilMethods.IsErrorParameterName(this.Context.CurrentConnectionConfig, columnInfo))
            {
                var pname = Builder.SqlParameterKeyWord + "CrorrPara" + GetDbColumnIndex;
                var p = new SugarParameter(pname, columnInfo.Value);
                this.Parameters.Add(p);
                GetDbColumnIndex++;
                return pname;

            }
            else
            {
                return name + "";
            }
        }
        private bool IsSingleSetExp(DbColumnInfo columnInfo) 
        {
            return this.ReSetValueBySqlExpList != null && 
                this.ReSetValueBySqlExpList.ContainsKey(columnInfo.PropertyName) && 
                this.IsListUpdate == null&& 
                DbColumnInfoList.GroupBy(it => it.TableId).Count()==1;
        }
        private bool IsListSetExp(DbColumnInfo columnInfo)
        {
            return this.ReSetValueBySqlExpListType != null && this.ReSetValueBySqlExpList != null && this.ReSetValueBySqlExpList.ContainsKey(columnInfo.PropertyName);
        }
        //public virtual string GetDbColumn(DbColumnInfo columnInfo, string name)
        //{
        //    if (columnInfo.UpdateServerTime)
        //    {
        //        return LambdaExpressions.DbMehtods.GetDate();
        //    }
        //    else if (columnInfo.UpdateSql.HasValue())
        //    {
        //        return columnInfo.UpdateSql;
        //    }
        //    else
        //    {
        //        return name + "";
        //    }
        //}
    }
}
