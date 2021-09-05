using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OscarUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return @"UPDATE  {1} {2} SET {0} FROM  ${{0}}  ";
            }
        }
        public override string SqlTemplateJoin
        {
            get
            {
                return @"            (VALUES
              {0}

            ) AS T ({2}) WHERE {1}
                 ";
            }
        }

        public override string SqlTemplateBatchUnion
        {
            get
            {
                return ",";
            }
        }

        public override object FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
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
                    string bytesString = "0x" + BitConverter.ToString((byte[])value);
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
                    return "'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "'" + value.ToString() + "'";
                }
            }
        }

        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            sb.AppendLine(string.Join("\r\n", groupList.Select(t =>
            {
                var updateTable = string.Format("UPDATE {0} SET", base.GetTableNameStringNoWith);
                var setValues = string.Join(",", t.Where(s => !s.IsPrimarykey).Select(m => GetOracleUpdateColums(i, m)).ToArray());
                var pkList = t.Where(s => s.IsPrimarykey).ToList();
                List<string> whereList = new List<string>();
                foreach (var item in pkList)
                {
                    var isFirst = pkList.First() == item;
                    var whereString = "";
                    whereString += GetOracleUpdateColums(i, item);
                    whereList.Add(whereString);
                }
                i++;
                return string.Format("{0} {1} WHERE {2};", updateTable, setValues, string.Join("AND", whereList));
            }).ToArray()));
            return sb.ToString();
        }
        private string GetOracleUpdateColums(int i, DbColumnInfo m)
        {
            return string.Format("\"{0}\"={1}", m.DbColumnName.ToUpper(), FormatValue(m.Value));
        }

    }
}
