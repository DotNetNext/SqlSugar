using System;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class QuestDBInsertBuilder : InsertBuilder
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
           ({2})";
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
                string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it =>
                   {
                       var spk = Builder.SqlParameterKeyWord + it.DbColumnName;
                       //if (it.Value is DateTime)
                       //{
                       //    return $"to_timestamp('{it.Value.ObjToString("yyyy-MM-ddTHH:mm:ss")}', 'yyyy-MM-ddTHH:mm:ss')";
                       //}
                       return  GetDbColumn(it,spk);
                    }
                
                ));
                ActionMinDate();
                return string.Format(SqlTemplate, GetTableNameString, columnsString, columnParametersString);
            }
            else
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                int pageSize = 200;
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
                        if (isFirst)
                        {
                            batchInsetrSql.Append(SqlTemplateBatchUnion);
                        }
                        batchInsetrSql.Append("\r\n ( " + string.Join(",", columns.Select(it =>
                        {
                            if (it.InsertServerTime || it.InsertSql.HasValue()) 
                            {
                                return GetDbColumn(it,null);
                            }
                            object value = null;
                            if (it.Value is DateTime)
                            {
                               return $" cast('{it.Value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.ffffff")}' as timestamp)";
                            }
                            else if (it.Value is int || it.Value is long ||it.Value is short || it.Value is short || it.Value is byte || it.Value is double)
                            {
                                return  it.Value;
                            }
                            else if (it.Value is bool)
                            {
                                return it.Value.ObjToString().ToLower();
                            } 
                            else
                            {
                                value = it.Value;
                            }
                            if (value == null||value==DBNull.Value)
                            {
                                return string.Format(SqlTemplateBatchSelect, "NULL");
                            }
                            return string.Format(SqlTemplateBatchSelect, "'" + value.ObjToString().ToSqlFilter() + "'");
                        })) + "),");
                        ++i;
                    }
                    pageIndex++;
                    batchInsetrSql.Remove(batchInsetrSql.Length - 1,1).Append("\r\n;\r\n");
                }
                return batchInsetrSql.ToString();
            }
        }
    }
}
