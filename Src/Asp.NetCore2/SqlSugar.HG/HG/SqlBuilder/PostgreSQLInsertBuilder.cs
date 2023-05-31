using System;
using System.Linq;
using System.Text;

namespace SqlSugar.HG
{
    public class HGInsertBuilder : InsertBuilder
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
           ({2}) returning $PrimaryKey";
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
        public override string SqlTemplateBatch => "INSERT INTO {0} ({1})";
        public override string SqlTemplateBatchUnion => " VALUES ";

        public override string SqlTemplateBatchSelect => " {0} ";

        public override Func<string, string, string> ConvertInsertReturnIdFunc { get; set; } = (name, sql) =>
        {
            return sql.Trim().TrimEnd(';')+ $"returning {name} ";
        };
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
                ActionMinDate();
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                int pageIndex = 1;
                if (IsNoPage&&IsReturnPkList) 
                {
                    pageSize = groupList.Count;
                }
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                while (pageCount >= pageIndex)
                {
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n ( " + string.Join(",", columns.Select(it =>
                        {
                            if (it.InsertServerTime || it.InsertSql.HasValue()||it.SqlParameterDbType is Type|| it?.PropertyType?.Name=="DateOnly" || it?.PropertyType?.Name == "TimeOnly")
                            {
                                return GetDbColumn(it, null);
                            }
                            object value = null;
                            if (it.Value is DateTime)
                            {
                                value = ((DateTime)it.Value).ToString("O");
                            }
                            else if (it.Value  is DateTimeOffset)
                            {
                                return FormatDateTimeOffset(it.Value);
                            }
                            else if (it.IsArray&&it.Value!=null) 
                            {
                                return FormatValue(it.Value,it.PropertyName,i,it);
                            }
                            else
                            {
                                value = it.Value;
                            }
                            if (value == null||value==DBNull.Value)
                            {
                                return string.Format(SqlTemplateBatchSelect, "NULL");
                            }
                            return string.Format(SqlTemplateBatchSelect, "'" + value.ObjToStringNoTrim().ToSqlFilter() + "'");
                        })) + "),");
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Remove(batchInsetrSql.Length - 1,1).Append("\r\n;\r\n");
                }
                return batchInsetrSql.ToString();
            }
        }

        public object FormatValue(object value, string name, int i, DbColumnInfo columnInfo)
        {
            if (value == null)
            {
                return "NULL";
            }
            else
            {
                var type = value.GetType();
                if (type == UtilConstants.DateType || columnInfo.IsArray || columnInfo.IsJson)
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
                else if (type == UtilConstants.DateTimeOffsetType)
                {
                    return FormatDateTimeOffset(value);
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
        public override string FormatDateTimeOffset(object value)
        {
            return "'" + ((DateTimeOffset)value).ToString("o") + "'";
        }

    }
}
