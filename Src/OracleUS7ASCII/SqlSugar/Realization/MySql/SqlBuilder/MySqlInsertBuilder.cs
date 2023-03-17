using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MySqlInsertBuilder : InsertBuilder
    {
        public override string SqlTemplate
        {
            get
            {
                if (IsReturnIdentity)
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;SELECT LAST_INSERT_ID();";
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
        int i = 0;
        public  object FormatValue(object value,string name)
        {
            var n = "N";
            if (this.Context.CurrentConnectionConfig.MoreSettings != null && this.Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
            {
                n = "";
            }
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType)
                {
                    return GetDateTimeString(value);
                }
                else if (value is DateTimeOffset)
                {
                    return GetDateTimeOffsetString(value);
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
                         return value.ToSqlValue(); ;
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
                    ++i;
                    var parameterName = this.Builder.SqlParameterKeyWord + name +"_"+ i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
                }
                else
                {
                    return n+"'" + GetString(value) + "'";
                }
            }
        }

        private object GetDateTimeOffsetString(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
            {
                date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
        }

        private object GetDateTimeString(object value)
        {
            var date = value.ObjToDate();
            if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
            {
                date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
        }

        private string GetString(object value)
        {
            var result = value.ToString();
            if (result.HasValue() && result.Contains("\\"))
            {
                result = result.Replace("\\", "\\\\");
            }
            return result;
        }

        public override string ToSqlString()
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
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it =>base.GetDbColumn(it, Builder.SqlParameterKeyWord + it.DbColumnName)));
                ActionMinDate();
                var result= string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
                result = GetMySqlIgnore(result);
                return result;
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                batchInsetrSql.Append("INSERT INTO " + GetTableNameString + " ");
                batchInsetrSql.Append("(");
                batchInsetrSql.Append(columnsString);
                batchInsetrSql.Append(") VALUES");
                string insertColumns = "";
                foreach (var item in groupList)
                {
                    batchInsetrSql.Append("(");
                    insertColumns = string.Join(",", item.Select(it => base.GetDbColumn(it, FormatValue(it.Value, it.PropertyName))));
                    batchInsetrSql.Append(insertColumns);
                    if (groupList.Last() == item)
                    {
                        batchInsetrSql.Append(") ");
                    }
                    else
                    {
                        batchInsetrSql.Append("),  ");
                    }
                }

                batchInsetrSql.AppendLine(";select @@IDENTITY");
                var result = batchInsetrSql.ToString();
                result = GetMySqlIgnore(result);
                return result;
            }
        }

        private string GetMySqlIgnore(string result)
        {
            if (this.MySqlIgnore)
            {
                result = result.Replace("INSERT INTO", " INSERT IGNORE INTO");
            }

            return result;
        }
    }
}
