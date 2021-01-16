using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class OracleInsertBuilder : InsertBuilder
    {
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
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            if (isSingle&&this.EntityInfo.EntityName!= "Dictionary`2")
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => Builder.SqlParameterKeyWord + it.DbColumnName));
                if (identities.HasValue())
                {
                    columnsString = columnsString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
                    columnParametersString = columnParametersString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => it.OracleSequenceName + ".nextval"));
                }
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                batchInsetrSql.AppendLine("INSERT ALL");
                foreach (var item in groupList)
                {
                    batchInsetrSql.Append("INTO " + GetTableNameString + " ");
                    string insertColumns = "";

                    batchInsetrSql.Append("(");
                    batchInsetrSql.Append(columnsString);
                    if (identities.HasValue())
                    {
                        batchInsetrSql.Append("," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName))));
                    }
                    batchInsetrSql.Append(") VALUES");


                    batchInsetrSql.Append("(");
                    insertColumns =  string.Join(",", item.Select(it =>FormatValue(it.Value)));
                    batchInsetrSql.Append(insertColumns);
                    if (identities.HasValue())
                    {
                        batchInsetrSql.Append(",");
                        foreach (var idn in identities)
                        {
                            var seqvalue = this.OracleSeqInfoList[idn.OracleSequenceName];
                            this.OracleSeqInfoList[idn.OracleSequenceName] = this.OracleSeqInfoList[idn.OracleSequenceName] + 1;
                            if (identities.Last() == idn)
                            {
                                batchInsetrSql.Append(seqvalue );
                            }
                            else
                            {
                                batchInsetrSql.Append(seqvalue + ",");
                            }
                        }
                    }
                    batchInsetrSql.AppendLine(")  ");

                }
                if (identities.HasValue())
                {
                    batchInsetrSql.AppendLine("SELECT "+ (this.OracleSeqInfoList.First().Value-1) + "  FROM DUAL");
                }
                else
                {
                    batchInsetrSql.AppendLine("SELECT 1 FROM DUAL");
                }
                var result= batchInsetrSql.ToString();
                return result;
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
                string N = this.Context.GetN();
                var type = UtilMethods.GetUnderType(value.GetType());
                if (type == UtilConstants.StringType && value.ToString().Contains("{SugarSeq:=}"))
                {
                    return value.ToString().Replace("{SugarSeq:=}", "");
                }
                if (type == UtilConstants.DateType)
                {
                    var date = value.ObjToDate();
                    if (date < Convert.ToDateTime("1900-1-1"))
                    {
                        date = Convert.ToDateTime("1900-1-1");
                    }
                    return "to_date('" + date.ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS') ";
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
                    return N+"'" + value.ToString().ToSqlFilter() + "'";
                }
                else
                {
                    return N+"'" + value.ToString() + "'";
                }
            }
        }
    }
}
