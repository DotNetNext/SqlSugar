using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public class AccessInsertBuilder : InsertBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return "INSERT INTO {0} ({1})";
            }
        }
        public override bool IsOleDb { get; set; } = true;
        public override string SqlTemplate
        {
            get
            {
                if (IsReturnIdentity)
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;select @@identity";
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
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = groupList.Count;
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex)
                {
                    var temp=string.Format(SqlTemplateBatch, GetTableNameString, columnsString);
                    //batchInsetrSql.AppendFormat("\r\nSelect  *  FROM(");
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        //var isFirst = i == 0;
                        //if (!isFirst)
                        //{
                        //    batchInsetrSql.Append("\t\r\nUNION ");
                        //}
                        //temp+" Values ( " + string.Join(",", columns.Select(it =>  FormatValue(it.Value) )+")"
                        batchInsetrSql.Append($"{temp} values ({string.Join(",", columns.Select(it =>base.GetDbColumn(it,FormatValue(it.Value))))}) "+UtilConstants.ReplaceCommaKey);
                        ++i;
                    }
                    pageIndex++;
                    //batchInsetrSql.Append(")  AS tblTemp\r\n");
                }
                var result = batchInsetrSql.ToString();
                return result;
            }
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
