using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public class AccessUpdateBuilder : UpdateBuilder
    {
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine(string.Join("", groupList.Select(t =>
            {
                var updateTable = string.Format("UPDATE {0} SET", base.GetTableNameStringNoWith);
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
                return string.Format("{0} {1} WHERE {2}", updateTable, setValues, string.Join(" AND ", whereList)) + UtilConstants.ReplaceCommaKey;
            }).ToArray()));
            return sb.ToString();
        }
        private string GetOracleUpdateColums(int i, DbColumnInfo m, bool iswhere)
        {
            return string.Format(" {0}={1} ", m.DbColumnName , base.GetDbColumn(m,FormatValue(m.Value)));
        }

        public override object FormatValue(object value)
        {
            var n = "";
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
                else if (SqlSugar.UtilMethods.IsNumber(type.Name))
                {
                    return value;
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else
                {
                    return n + "'" + value + "'";
                }
            }
        }
        private object GetDateTimeOffsetString(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            if (date == DateTime.MinValue)
            {
                date = Convert.ToDateTime("1900-01-01");
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }
        private object GetDateTimeString(object value)
        {
            var date = value.ObjToDate();
            if (date == DateTime.MinValue)
            {
                date = Convert.ToDateTime("1900-01-01");
            }
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }
    }
}
