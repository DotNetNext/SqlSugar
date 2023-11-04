using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DmUpdateBuilder : UpdateBuilder
    {
        public override string ReSetValueBySqlExpListType { get; set; } = "dm";
        protected override string GetJoinUpdate(string columnsString, ref string whereString)
        {
            var joinString = $"  {Builder.GetTranslationColumnName(this.TableName)}  {Builder.GetTranslationColumnName(this.ShortName)} ";
            foreach (var item in this.JoinInfos)
            {
                joinString += $"\r\n USING {Builder.GetTranslationColumnName(item.TableName)}  {Builder.GetTranslationColumnName(item.ShortName)} ON {item.JoinWhere} ";
            }
            var tableName = joinString + "\r\n ";
            var newTemp = SqlTemplate.Replace("UPDATE", "MERGE INTO").Replace("SET", "WHEN MATCHED THEN \r\nUPDATE SET");
            return string.Format(newTemp, tableName, columnsString, whereString);
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
            var result= sb.ToString();
            if (groupList.Count==0) 
            {
                return null;
            }
            return result;
        }

        private string GetOracleUpdateColums(int i, DbColumnInfo m)
        {
            return string.Format("\"{0}\"={1}", m.DbColumnName.ToUpper(IsUppper), base.GetDbColumn(m,FormatValue(i, m.DbColumnName, m.Value)));
        }
        public bool IsUppper
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.MoreSettings == null)
                {
                    return true;
                }
                else
                {
                    return this.Context.CurrentConnectionConfig.MoreSettings.IsAutoToUpper == true;
                }
            }
        }
        public override string FormatDateTimeOffset(object value)
        {
            var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
            return "'" + date.ToString("yyyy-MM-dd HH:mm:ss.fff zzz") + "'";
        }
        public object FormatValue(int i, string name, object value)
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
                    if (this.Context.CurrentConnectionConfig?.MoreSettings?.DisableMillisecond == true)
                    {
                        return "to_date('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS')  ";
                    }
                    else
                    {
                        return "to_timestamp('" + date.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "', 'YYYY-MM-DD HH24:MI:SS.FF') ";
                    }
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
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value));
                    return parameterName;
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
    }
}
