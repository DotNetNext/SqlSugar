using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SqlServerBlueCopy
    {
        internal List<IGrouping<int, DbColumnInfo>> DbColumnInfoList { get;   set; }
        internal SqlSugarProvider Context { get;   set; }
        internal ISqlBuilder Builder { get; set; }
        internal InsertBuilder InsertBuilder { get; set; }
        public int ExecuteBlueCopy()
        {
            if (DbColumnInfoList==null||DbColumnInfoList.Count == 0) return 0;
            int pmax = 2030;
            decimal count = DbColumnInfoList.Count;
            var columns = DbColumnInfoList.First().Select(it => Builder.GetTranslationColumnName(it.DbColumnName)).ToList();
            decimal columnCount = columns.Count();
            decimal pageSize = count / ((count * columnCount) / pmax);
            this.Context.Utilities.PageEach(DbColumnInfoList,Convert.ToInt32(pageSize), pageItems =>
            {
                StringBuilder batchInsetrSql = new StringBuilder();
                batchInsetrSql.AppendFormat(InsertBuilder.SqlTemplateBatch, InsertBuilder.GetTableNameString, string.Join(",", columns));
                int i = 0;
                foreach (var item in pageItems)
                {
                    batchInsetrSql.Append("\r\n SELECT " + string.Join(",", item.ToList().Select(it => string.Format(InsertBuilder.SqlTemplateBatchSelect, AddParameter(i,it.DbColumnName,it.Value), Builder.GetTranslationColumnName(it.DbColumnName)))));
                    if (pageItems.Last() != item)
                    {
                        batchInsetrSql.Append(" UNION ALL");
                    }
                    ++i;
                }
                this.Context.Ado.ExecuteCommand(batchInsetrSql.ToString(),InsertBuilder.Parameters);
                InsertBuilder.Parameters = new List<SugarParameter>();
            });
            return count.ObjToInt();
        }

        private object AddParameter(int i,string dbColumnName, object value)
        {
            var name =Builder.SqlParameterKeyWord+dbColumnName+i;
            InsertBuilder.Parameters.Add(new SugarParameter(name,value));
            return name;
        }
    }
}
