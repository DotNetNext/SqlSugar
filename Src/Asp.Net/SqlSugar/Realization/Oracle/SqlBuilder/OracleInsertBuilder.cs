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
            if (this.Context.CurrentConnectionConfig?.MoreSettings?.EnableOracleIdentity == true)
            {
                this.DbColumnInfoList = this.DbColumnInfoList.Where(it => it.IsIdentity == false).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            if (isSingle && this.EntityInfo.EntityName != "Dictionary`2")
            {
              
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it => base.GetDbColumn(it, Builder.SqlParameterKeyWord + it.DbColumnName)));
                if (identities.HasValue())
                {
                    columnsString = columnsString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
                    columnParametersString = columnParametersString.TrimEnd(',') + "," + string.Join(",", identities.Select(it => it.OracleSequenceName + ".nextval"));
                }
                ActionMinDate();
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                var bigSize = 500;
                if (groupList.Count < bigSize||this.Context?.CurrentConnectionConfig?.MoreSettings?.EnableOracleIdentity==true)
                {
                    string result = Small(identities, groupList, columnsString);
                    return result;
                }
                else
                {
                    string result = Big(identities, groupList, columnsString);
                    return result;
                }
            }
        }
        private string Big(List<EntityColumnInfo> identities, List<IGrouping<int, DbColumnInfo>> groupList, string columnsString)
        {
            this.Context.Utilities.PageEach(groupList, 100, groupListPasge =>
            {
                this.Parameters = new List<SugarParameter>();
                var sql = Small(identities, groupListPasge, columnsString);
                this.Context.Ado.ExecuteCommand(sql, this.Parameters);
            });
            if (identities != null && identities.Count > 0 && this.OracleSeqInfoList != null && this.OracleSeqInfoList.Any())
            {
                return $"SELECT {this.OracleSeqInfoList.First().Value - 1} FROM DUAL";
            }
            else
            {
                return $"SELECT {groupList.Count} FROM DUAL";
            }
        }

        private string Small(List<EntityColumnInfo> identities, List<IGrouping<int, DbColumnInfo>> groupList, string columnsString)
        {
            StringBuilder batchInsetrSql = new StringBuilder();
            batchInsetrSql.AppendLine("INSERT ALL");
            foreach (var item in groupList)
            {
                batchInsetrSql.Append("INTO " + GetTableNameString + " ");
                string insertColumns = "";

                batchInsetrSql.Append("(");
                batchInsetrSql.Append(columnsString);
                if (identities.HasValue()&& this.IsOffIdentity==false)
                {
                    batchInsetrSql.Append("," + string.Join(",", identities.Select(it => Builder.GetTranslationColumnName(it.DbColumnName))));
                }
                batchInsetrSql.Append(") VALUES");


                batchInsetrSql.Append("(");
                insertColumns = string.Join(",", item.Select(it => GetDbColumn(it, FormatValue(it.Value, it.PropertyName))));
                batchInsetrSql.Append(insertColumns);
                if (this.IsOffIdentity == false)
                {
                    if (identities.HasValue())
                    {
                        batchInsetrSql.Append(",");
                        foreach (var idn in identities)
                        {
                            var seqvalue = this.OracleSeqInfoList[idn.OracleSequenceName];
                            this.OracleSeqInfoList[idn.OracleSequenceName] = this.OracleSeqInfoList[idn.OracleSequenceName] + 1;
                            if (identities.Last() == idn)
                            {
                                batchInsetrSql.Append(seqvalue);
                            }
                            else
                            {
                                batchInsetrSql.Append(seqvalue + ",");
                            }
                        }
                    }
                }
                batchInsetrSql.AppendLine(")  ");

            }
            if (identities.HasValue())
            {
                batchInsetrSql.AppendLine("SELECT " + (this.OracleSeqInfoList.First().Value - 1) + "  FROM DUAL");
            }
            else
            {
                batchInsetrSql.AppendLine("SELECT 1 FROM DUAL");
            }
            var result = batchInsetrSql.ToString();
            return result;
        }

        int i = 0;
        public object FormatValue(object value, string name)
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
                else if (type == UtilConstants.ByteArrayType)
                {
                    ++i;
                    var parameterName = this.Builder.SqlParameterKeyWord + name + i;
                    this.Parameters.Add(new SugarParameter(parameterName, value, System.Data.DbType.Binary));
                    return parameterName;
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
    }
}
