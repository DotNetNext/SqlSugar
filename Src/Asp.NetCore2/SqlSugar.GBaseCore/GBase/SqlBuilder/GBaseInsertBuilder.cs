using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GBase
{
    public class GBaseInsertBuilder:InsertBuilder
    {
        public override string SqlTemplateBatch
        {
            get
            {
                return "INSERT  into {0} ({1})";
            }
        }
        public override string SqlTemplate
        {
            get
            {
                if (IsReturnIdentity)
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2})"+UtilConstants.ReplaceCommaKey.Replace("{","").Replace("}", "") + " SELECT dbinfo('sqlca.sqlerrd1') FROM dual";
                }
                else
                {
                    return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) "+UtilConstants.ReplaceCommaKey.Replace("{", "").Replace("}", "") + "SELECT dbinfo('sqlca.sqlerrd1') FROM dual";

                }
            }
        }
        public override string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityInfo.EntityName);
                result += UtilConstants.Space;
                if (this.TableWithString.HasValue())
                {
                    result += TableWithString + UtilConstants.Space;
                }
                return result;
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
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    batchInsetrSql.AppendFormat("SELECT * FROM (");
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (!isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, base.GetDbColumn(it,FormatValue(it.Value)), Builder.GetTranslationColumnName(it.DbColumnName))))+" from dual");
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append(") temp1\r\n;\r\n");
                }
                var result = batchInsetrSql.ToString();
                //if (this.Context.CurrentConnectionConfig.DbType == DbType.GBase)
                //{
                //    result += "";
                //}
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
                else if (type == UtilConstants.BoolType)
                {
                    return string.Format("CAST({0} AS boolean)", value.ObjToBool()?1:0) ;
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

    }
}
