using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SqlSugar;

namespace SqlSugar.GBase
{
    public class GBaseUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplate
        {
            get
            {
                return @"UPDATE {0} SET {1} {2}";

            }
        }
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            if (groupList == null || groupList.Count == 0)
            {
                return "select 0 from DUAL";
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine(string.Join(UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", ""), groupList.Select(t =>
            {
                var updateTable = string.Format("UPDATE {0} SET ", base.GetTableNameStringNoWith);
                var setValues = string.Join(",", t.Where(s => !s.IsPrimarykey).Select(m => GetOracleUpdateColums(i, m, false)).ToArray());
                var pkList = t.Where(s => s.IsPrimarykey).ToList();
                List<string> whereList = new List<string>();
                foreach (var item in pkList)
                {
                    var isFirst = pkList.First() == item;
                    var whereString = "";
                    whereString += GetOracleUpdateColums(i, item, true);
                    whereList.Add(whereString);
                }
                i++;
                return string.Format("{0} {1} WHERE {2} ", updateTable, setValues, string.Join(" AND ", whereList));
            }).ToArray()));
            return sb.ToString();
        }
        private object GetValue(DbColumnInfo it)
        {
            return FormatValue(it.Value);
        }
        private string GetOracleUpdateColums(int i, DbColumnInfo m, bool iswhere)
        {
            return string.Format("{0}={1}", m.DbColumnName, base.GetDbColumn(m,FormatValue(i, m.DbColumnName, m.Value, iswhere)));
        }
        public override object FormatValue(object value)
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
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else if (type == UtilConstants.IntType || type == UtilConstants.LongType)
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
                    return "'" + value.ToString() + "'";
                }
            }
        }
        public object FormatValue(int i, string name, object value, bool iswhere)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType && iswhere == false)
                {
                    var date = value.ObjToDate();
                    if (date < UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig))
                    {
                        date = UtilMethods.GetMinDate(this.Context.CurrentConnectionConfig);
                    }
                    if (this.Context.CurrentConnectionConfig?.MoreSettings?.DisableMillisecond == true)
                    {
                        return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    else
                    {
                        return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                    }
                }
                else if (type == UtilConstants.DateType && iswhere)
                {
                    var parameterName = i + this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
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
                else if (type == UtilConstants.ByteArrayType)
                {
                    var parameterName = i + this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
                }
                else if (value is int || value is long || value is short || value is short || value is byte)
                {
                    return value;
                }
                else if (value is bool)
                {
                    return string.Format("CAST({0} AS boolean)", value.ObjToBool() ? 1 : 0);
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }
    }
}
