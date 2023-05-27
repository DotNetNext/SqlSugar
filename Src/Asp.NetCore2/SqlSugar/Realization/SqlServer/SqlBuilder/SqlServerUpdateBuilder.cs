using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlServerUpdateBuilder: UpdateBuilder
    {
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, " Update List<T> need Primary key");
            int pageSize = 200;
            int pageIndex = 1;
            int totalRecord = groupList.Count;
            int pageCount = (totalRecord + pageSize - 1) / pageSize;
            StringBuilder batchUpdateSql = new StringBuilder();
            while (pageCount >= pageIndex)
            {
                StringBuilder updateTable = new StringBuilder();
                string setValues = string.Join(",", groupList.First().Where(it => it.IsPrimarykey == false && (it.IsIdentity == false || (IsOffIdentity && it.IsIdentity))).Select(it =>
                {
                    if (SetValues.IsValuable())
                    {
                        var setValue = SetValues.Where(sv => sv.Key == Builder.GetTranslationColumnName(it.DbColumnName));
                        if (setValue != null && setValue.Any())
                        {
                            return setValue.First().Value;
                        }
                    }
                    var result = string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(it.DbColumnName));
                    return result;
                }));
                batchUpdateSql.AppendFormat(SqlTemplateBatch.ToString(), setValues, GetTableNameStringNoWith, TableWithString);
                int i = 0;
                foreach (var columns in groupList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList())
                {
                    var isFirst = i == 0;
                    if (!isFirst)
                    {
                        updateTable.Append(SqlTemplateBatchUnion);
                    }
                    updateTable.Append("\r\n SELECT " + string.Join(",", columns.Select(it => string.Format(base.SqlTemplateBatchSelect, base.GetDbColumn(it, GetValue(it)), Builder.GetTranslationColumnName(it.DbColumnName)))));
                    ++i;
                }
                pageIndex++;
                updateTable.Append("\r\n");
                string whereString = null;
                if (this.WhereValues.HasValue())
                {
                    foreach (var item in WhereValues)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += Regex.Replace(item, "\\" + this.Builder.SqlTranslationLeft, "S." + this.Builder.SqlTranslationLeft);
                    }
                }
                if (PrimaryKeys.HasValue())
                {
                    foreach (var item in PrimaryKeys)
                    {
                        var isFirst = whereString == null;
                        whereString += (isFirst ? null : " AND ");
                        whereString += string.Format("S.{0}=T.{0}", Builder.GetTranslationColumnName(item));
                    }
                }
                batchUpdateSql.AppendFormat(SqlTemplateJoin, updateTable, whereString);
            }
            batchUpdateSql = GetBatchUpdateSql(batchUpdateSql);
            return batchUpdateSql.ToString();
        }

        private StringBuilder GetBatchUpdateSql(StringBuilder batchUpdateSql)
        {
            if (ReSetValueBySqlExpListType == null && ReSetValueBySqlExpList != null)
            {
                var result = batchUpdateSql.ToString();
                foreach (var item in ReSetValueBySqlExpList)
                {
                    var dbColumnName = item.Value.DbColumnName;
                    if (item.Value.Type==ReSetValueBySqlExpListModelType.List)
                    { 
                        result = result.Replace($"T.{dbColumnName}", $" S.{dbColumnName}{item.Value.Sql}T.{dbColumnName}");
                    }
                    else
                    {
                        result = result.Replace($"T.{dbColumnName}", item.Value.Sql.Replace(dbColumnName, "S." + dbColumnName));
                    }
                    batchUpdateSql = new StringBuilder(result);
                }
            }

            return batchUpdateSql;
        }

        private object GetValue(DbColumnInfo it)
        {
            if (it.SqlParameterDbType!=null&&it.SqlParameterDbType.Equals(System.Data.DbType.AnsiString))
            {
                var value = FormatValue(it.Value);
                if (value is string&&value.ObjToString().StartsWith("N'"))
                {
                    return value.ObjToString().TrimStart('N');
                }
                else 
                {
                    return value;
                }
            }
            else
            {
                return FormatValue(it.Value);
            }
        }
        public override string FormatDateTimeOffset(object value)
        {
            return "'"+((DateTimeOffset)value).ToString("o")+"'";
        }
    }
}
