using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
namespace SqlSugar
{
    public partial class InsertBuilder : IDMLBuilder
    {
        #region  Init
        public InsertBuilder()
        {
            this.sql = new StringBuilder();
            this.Parameters = new List<SugarParameter>();
            this.DbColumnInfoList = new List<DbColumnInfo>();
        }

        #endregion

        #region Common Properties
        public SqlSugarProvider Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public bool IsNoInsertNull { get; set; }
        public bool IsReturnIdentity { get; set; }
        public EntityInfo EntityInfo { get; set; }
        public Dictionary<string, int> OracleSeqInfoList { get; set; }
        public bool IsBlukCopy { get; set; }
        public virtual bool IsOleDb { get; set; }
        public virtual Func<string, string, string> ConvertInsertReturnIdFunc { get; set; }
        public virtual bool IsNoPage { get; set; }

        public virtual bool IsReturnPkList { get; set; }
        public string AsName { get; set; }
        public bool IsOffIdentity { get;  set; }
        #endregion

        #region SqlTemplate
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
        public virtual string SqlTemplateBatch
        {
            get
            {
                return "INSERT {0} ({1})";
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

        #endregion

        #region Methods

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
        public virtual void Clear()
        {

        }
        public virtual string GetTableNameString
        {
            get
            {
                if (AsName.HasValue()) 
                {
                    return Builder.GetTranslationTableName(AsName);
                }
                var result = Builder.GetTranslationTableName(EntityInfo.EntityName);
                result += UtilConstants.Space;
                if (this.TableWithString.HasValue())
                {
                    result += TableWithString + UtilConstants.Space;
                }
                return result;
            }
        }

        public bool MySqlIgnore { get; internal set; }

        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                resolveExpress.TableEnumIsString = this.Context.CurrentConnectionConfig.MoreSettings.TableEnumIsString;
                resolveExpress.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                resolveExpress.PgSqlIsAutoToLower = true;
            }
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var result = resolveExpress.Result;
            return result;
        }
        public virtual string ToSqlString()
        {
            if (IsNoInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it =>this.GetDbColumn(it, Builder.SqlParameterKeyWord + it.DbColumnName)));
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                if (this.EntityInfo.Columns.Count > 30)
                {
                    pageSize = 50;
                }
                else if (this.EntityInfo.Columns.Count > 20)
                {
                    pageSize = 100;
                }
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
                        batchInsetrSql.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect,this.GetDbColumn(it, FormatValue(it.Value)),Builder.GetTranslationColumnName(it.DbColumnName)))));
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append("\r\n;\r\n");
                }
                var result= batchInsetrSql.ToString();
                if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                {
                    result += "select @@identity;";
                }
                return result;
            }
        }
        public virtual object FormatValue(object value)
        {
            var N = "N";
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Sqlite) 
            {
                N = "";
            }
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type =UtilMethods.GetUnderType(value.GetType());
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
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return N+"'" + value.ToString().ToSqlFilter() + "'";
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return FormatDateTimeOffset(value);
                }
                else if (type == UtilConstants.FloatType) 
                {
                    return N+"'" +Convert.ToDouble(value).ToString() + "'";
                }
                else
                {
                    return N+"'" + value.ToString() + "'";
                }
            }
        }

        public virtual string FormatDateTimeOffset(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
        }

        private int GetDbColumnIndex = 0;
        public virtual string GetDbColumn(DbColumnInfo columnInfo ,object name) 
        {
            if (columnInfo.InsertServerTime)
            {
                return LambdaExpressions.DbMehtods.GetDate();
            }
            else if (UtilMethods.IsErrorDecimalString() == true)
            {
                var pname = Builder.SqlParameterKeyWord + "Decimal" + GetDbColumnIndex;
                var p = new SugarParameter(pname, columnInfo.Value);
                this.Parameters.Add(p);
                GetDbColumnIndex++;
                return pname;
            }
            else if (columnInfo.InsertSql.HasValue())
            {
                return columnInfo.InsertSql;
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
                var type=columnInfo.SqlParameterDbType as Type;
                var ParameterConverter=type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyType);
                var obj=Activator.CreateInstance(type);
                var p = ParameterConverter.Invoke(obj,new object[] {columnInfo.Value, GetDbColumnIndex }) as SugarParameter;
                GetDbColumnIndex++;
                //this.Parameters.RemoveAll(it => it.ParameterName == it.ParameterName);
                UtilMethods.ConvertParameter(p,this.Builder);
                this.Parameters.Add(p);
                return p.ParameterName;
            }
            else if (columnInfo.DataType?.Equals("nvarchar2")==true) 
            {
                var pname = Builder.SqlParameterKeyWord + columnInfo.DbColumnName + "_ts" + GetDbColumnIndex;
                var p = new SugarParameter(pname, columnInfo.Value);
                p.IsNvarchar2 = true;
                this.Parameters.Add(p);
                GetDbColumnIndex++;
                return pname;
            }
            else if (columnInfo.PropertyType!=null&&columnInfo.PropertyType.Name == "TimeOnly" )
            {
                var timeSpan = UtilMethods.TimeOnlyToTimeSpan(columnInfo.Value);
                var pname = Builder.SqlParameterKeyWord + columnInfo.DbColumnName + "_ts" + GetDbColumnIndex;
                this.Parameters.Add(new SugarParameter(pname, timeSpan));
                GetDbColumnIndex++;
                return pname;
            }
            else if (columnInfo.PropertyType != null && columnInfo.PropertyType.Name == "DateOnly")
            {
                var timeSpan = UtilMethods.DateOnlyToDateTime(columnInfo.Value);
                var pname = Builder.SqlParameterKeyWord + columnInfo.DbColumnName + "_ts" + GetDbColumnIndex;
                if (timeSpan == null)
                {
                    this.Parameters.Add(new SugarParameter(pname, null) { DbType=System.Data.DbType.Date });
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

        #endregion
    }
}
