using System;
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
        public override object FormatValue(object value)
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
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
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
                    return Convert.ToInt64(value);
                }
                else if (type == UtilConstants.BoolType)
                {
                    return value.ObjToBool() ? "1" : "0";
                }
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return n+"'" + GetString(value).ToSqlFilter() + "'";
                }
                else
                {
                    return n+"'" + GetString(value) + "'";
                }
            }
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
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.SqlParameterKeyWord + it.DbColumnName));
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
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
                    insertColumns = string.Join(",", item.Select(it => FormatValue(it.Value)));
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
                return result;
            }
        }
    }
}
