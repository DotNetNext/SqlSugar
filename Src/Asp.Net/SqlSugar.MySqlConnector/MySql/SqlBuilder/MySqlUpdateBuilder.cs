using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar.MySqlConnector
{
    public class MySqlUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE  {1} S {2}   INNER JOIN ${{0}}  SET {0} ";
            }
        }
        public override string SqlTemplateJoin
        {
            get
            {
                return @"            (
              {0}

            ) T ON {1}
                 ";
            }
        }
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
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
                    updateTable.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value,it.PropertyName),this.Builder.GetTranslationColumnName(it.DbColumnName)))));
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
                        whereString += Regex.Replace(item, " \\" + this.Builder.SqlTranslationLeft, "S." + this.Builder.SqlTranslationLeft);
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
                var format= string.Format(SqlTemplateJoin, updateTable, whereString);
                batchUpdateSql.Replace("${0}",format);
                batchUpdateSql.Append(";");
            }
            return batchUpdateSql.ToString();
        }
        int i = 0;
        public  object FormatValue(object value,string name)
        {
            var n = "N";
            if (this.Context.CurrentConnectionConfig.MoreSettings != null&&this.Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
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
                        return value.ToSqlValue();
                    }
                    else
                    {
                        return Convert.ToInt64(value);
                    }
                }
                else if (type == UtilConstants.LongType)
                {
                    return GetString(value);
                }
                else if (type == UtilConstants.IntType)
                {
                    return GetString(value);
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

        private object GetDateTimeString(object value)
        {
            var date = value.ObjToDate();
            if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
            {
                date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
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

        private string GetString(object value)
        {
            var result = value.ToString();
            if (result.HasValue() && result.Contains("\\"))
            {
                result = result.Replace("\\", "\\\\");
            }
            return result;
        }
    }
}
