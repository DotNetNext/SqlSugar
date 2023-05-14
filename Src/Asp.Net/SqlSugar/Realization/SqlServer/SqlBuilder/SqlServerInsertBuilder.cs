using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlServerInsertBuilder:InsertBuilder
    {
        public override Func<string, string, string> ConvertInsertReturnIdFunc { get; set; } = (name, sql) =>
        {
            return sql.Replace("select SCOPE_IDENTITY();", "").Replace(")\r\n SELECT", $")\r\n OUTPUT INSERTED.{name} as {name}  \r\nSELECT");
        };
        public override bool IsNoPage { get; set; } = true;
        public override string ToSqlString()
        {
            if (IsNoInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(it => it.Value != null).ToList();
            }
            var groupList = DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            var isSingle = groupList.Count() == 1;
            string columnsString = string.Join(",", groupList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)));
            var result = "";
            if (isSingle)
            {
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it =>base.GetDbColumn(it,Builder.SqlParameterKeyWord + it.DbColumnName)));
                result= string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
                if (this.EntityInfo.Columns.Count > 30)
                {
                    pageSize = 50;
                }
                else if (this.EntityInfo.Columns.Count > 20)
                {
                    pageSize = 100;
                }
                if (IsNoPage && IsReturnPkList) 
                {
                    pageSize = groupList.Count;
                }
                int pageIndex = 1;
                int totalRecord = groupList.Count;
                int pageCount = (totalRecord + pageSize - 1) / pageSize;
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
                        batchInsetrSql.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(SqlTemplateBatchSelect,base.GetDbColumn(it, FormatValue(it.Value)), Builder.GetTranslationColumnName(it.DbColumnName)))));
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Append("\r\n;\r\n");
                }
                 result = batchInsetrSql.ToString();
                if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                {
                    result += "select SCOPE_IDENTITY();";
                }
            }

            if (this.IsOffIdentity)
            {
                var tableName = this.GetTableNameString;
                result= $"SET IDENTITY_INSERT {tableName} ON;" + result.TrimEnd(';') + $";SET IDENTITY_INSERT {tableName} OFF"; ;
            }
            return result;
        }
        public override string FormatDateTimeOffset(object value)
        {
            return "'" + ((DateTimeOffset)value).ToString("o") + "'";
        }
    }
}
