using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseUpdateBuilder : UpdateBuilder
    {
        public override string SqlTemplate => "ALTER TABLE {0} UPDATE {1} {2}";
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

        public  object FormatValue(object value,string name,int i,DbColumnInfo columnInfo)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
                if (type == UtilConstants.DateType||columnInfo.IsArray||columnInfo.IsJson)
                {
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    var paramter = new SugarParameter(parameterName, value);
                    if (columnInfo.IsJson) 
                    {
                        paramter.IsJson = true;
                    }
                    if (columnInfo.IsArray) 
                    {
                        paramter.IsArray = true;
                    }
                    this.Parameters.Add(paramter);
                    return parameterName;
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    string bytesString = "0x" + BitConverter.ToString((byte[])value);
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
            sb.AppendLine(string.Join(UtilConstants.ReplaceCommaKey, groupList.Select(t =>
            {
                var updateTable = string.Format("ALTER TABLE {0} UPDATE ", base.GetTableNameStringNoWith);
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
                return string.Format("{0} {1} WHERE {2} ", updateTable, setValues, string.Join(" AND", whereList));
            }).ToArray()));
            return sb.ToString();
        }
        private string GetOracleUpdateColums(int i, DbColumnInfo m, bool iswhere)
        {
            return string.Format("\"{0}\"={1}", m.DbColumnName, base.GetDbColumn(m,FormatValue(i, m.DbColumnName, m.Value, iswhere,m)));
        }
        public object FormatValue(int i, string name, object value, bool iswhere,DbColumnInfo dbColumnInfo)
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
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    i++;
                    return parameterName;
                }
                //else if (type == UtilConstants.DateType && iswhere)
                //{
                //    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                //    this.Parameters.Add(new SugarParameter(parameterName, value));
                //    return parameterName;
                //}
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
                else if (dbColumnInfo.IsArray && value != null)
                {
                    return   "'" + this.Context.Utilities.SerializeObject(value) + "'";
                }
                else if (dbColumnInfo.IsJson && value != null)
                {
                    return  "'" + this.Context.Utilities.SerializeObject(value) + "'";
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
                }
                else if (value is int || value is long || value is short || value is short || value is byte)
                {
                    return value;
                }
                else if (value is bool)
                {
                    return value.ObjToString().ToLower();
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
