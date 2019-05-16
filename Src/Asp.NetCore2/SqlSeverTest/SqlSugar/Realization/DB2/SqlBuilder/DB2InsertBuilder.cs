﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DB2InsertBuilder : InsertBuilder
    {
        public override string SqlTemplateBatchSelect
        {
            get
            {
                return "{0}  {1}";
            }
        }
        public override string SqlTemplate
        {
            get
            {
                return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ";

            }
        }
        public override string SqlTemplateBatch
        {
            get
            {
                return "INSERT INTO {0} ({1})";
            }
        }
        public override string ToSqlString()
        {
            var identities = this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).ToList();
            if (IsNoInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            var colinfs = groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName));
            string columnsString = string.Join(",", colinfs);
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.SqlParameterKeyWord));
                if (identities.HasValue())
                {
                    //columnsString = columnsString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
                    //columnParametersString = columnParametersString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => it.OracleSequenceName + ".nextval"));
                    columnsString = string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName))) + (colinfs.Count() > 0 ? ("," + columnsString.TrimEnd(',')) : "");
                    columnParametersString =   string.Join(",", identities.Select(it => it.OracleSequenceName + ".nextval")) + (colinfs.Count() > 0 ? ("," + columnParametersString.TrimEnd(',')) : "");
                }
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
                if (identities.HasValue())
                {
                    columnsString = columnsString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
                }
                while (pageCount >= pageIndex)
                {
                    batchInsetrSql.AppendFormat(SqlTemplateBatch, GetTableNameString, columnsString);
                    int i = 0;
                    foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                    {
                        var isFirst = i == 0;
                        if (!isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        var insertColumns = string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect, FormatValue(it.Value), "")));
                        if (identities.HasValue())
                        {
                            insertColumns = insertColumns.TrimEnd(',') + "," + string.Join(",", identities.Select(it =>
                            {
                                var seqValue = this.OracleSeqInfoList[it.OracleSequenceName];
                                this.OracleSeqInfoList[it.OracleSequenceName] = this.OracleSeqInfoList[it.OracleSequenceName] + 1;
                                return seqValue + 1+"  "+it.DbColumnName;
                            }));
                        }
                        batchInsetrSql.Append("\r\n SELECT " + insertColumns + " FROM sysibm.dual ");
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append("\r\n;\r\n");
                }
                return "BEGIN\r\n"+ batchInsetrSql.ToString()+"\r\nEND;";
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
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.DateType)
                {
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
                    }
                    return "timestamp(trim(char('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'))) ";  
                }
                else if (type.IsEnum())
                {
                    return Convert.ToInt64(value);
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
                else if (type == UtilConstants.StringType || type == UtilConstants.ObjType)
                {
                    return "N'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return "N'" + value.ToString() + "'";
                }
            }
        }
    }
}
