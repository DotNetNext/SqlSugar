using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class OracleUpdateBuilder : UpdateBuilder
    {
        public override string ReSetValueBySqlExpListType { get; set; } = "oracle";
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            if (groupList.Count == 0)
            {
                return " select 0 from dual";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Begin");
            sb.AppendLine(string.Join("\r\n", groupList.Select(t =>
            {
                var updateTable = string.Format("UPDATE {0} SET", base.GetTableNameStringNoWith);
                var setValues = string.Join(",", t.Where(s => !s.IsPrimarykey).Select(m => GetOracleUpdateColums(m)).ToArray());
                var pkList = t.Where(s => s.IsPrimarykey).ToList();
                List<string> whereList = new List<string>();
                foreach (var item in pkList)
                {
                    var isFirst = pkList.First() == item;
                    var whereString = isFirst ? " " : " AND ";
                    whereString += GetOracleUpdateColums(item);
                    whereList.Add(whereString);
                }
                return string.Format("{0} {1} WHERE {2};", updateTable, setValues, string.Join("",whereList));
            }).ToArray()));
            sb.AppendLine("End;");
            return sb.ToString();
        }

        private string GetOracleUpdateColums(DbColumnInfo m)
        {
            return string.Format("\"{0}\"={1}", m.DbColumnName.ToUpper(IsUppper), base.GetDbColumn(m,FormatValue(m.Value,m.IsPrimarykey,m.PropertyName)));
        }
        int i = 0;
        public  object FormatValue(object value,bool isPrimaryKey,string name)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                string N =this.Context.GetN();
                if (isPrimaryKey) 
                {
                    N = "";
                }
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
                    return Convert.ToInt64(value);
                }
                else if (type.IsIn(UtilConstants.IntType,UtilConstants.LongType,UtilConstants.ShortType))
                {
                    return value;
                }
                else if (type==UtilConstants.GuidType)
                {
                    return  "'" + value.ToString() + "'";
                }
                else if (type == UtilConstants.ByteArrayType)
                {
                    string bytesString = "0x" + BitConverter.ToString((byte[])value).Replace("-", "");
                    return bytesString;
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    var date = UtilMethods.ConvertFromDateTimeOffset((DateTimeOffset)value);
                    return "to_timestamp('" + date.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "', 'YYYY-MM-DD HH24:MI:SS.FF') ";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    if (value.ToString().Length > 1000)
                    {
                        ++i;
                        var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                        this.Parameters.Add(new SugarParameter(parameterName, value));
                        return parameterName;
                    }
                    else
                    {
                        return N + "'" + value.ToString().ToSqlFilter() + "'";
                    }
                }
                else
                {
                    return N + "'" + value.ToString() + "'";
                }
            }
        }
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
        #region Helper
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
        #endregion
    }
}
