using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            DataTable dt = new DataTable();
            var columns = DbColumnInfoList.First().Select(it => it.DbColumnName ).ToList();
            foreach (var item in columns)
            {
                dt.Columns.Add(item);
            }
            foreach (var rowInfos in DbColumnInfoList)
            {
                var dr = dt.NewRow();
                foreach (var item in rowInfos.ToList())
                {
                    dr[item.DbColumnName] = item.Value;
                }
                dt.Rows.Add(dr);
            }
            SqlBulkCopy bulkCopy = new SqlBulkCopy(this.Context.Ado.Connection as SqlConnection);
            //获取目标表的名称
            bulkCopy.DestinationTableName = InsertBuilder.EntityInfo.EntityName;
            //写入DataReader对象
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            bulkCopy.WriteToServer(dt);
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Ado.Connection.Close();
            }
            return DbColumnInfoList.Count;
        }

        private object AddParameter(int i,string dbColumnName, object value)
        {
            var name =Builder.SqlParameterKeyWord+dbColumnName+i;
            InsertBuilder.Parameters.Add(new SugarParameter(name,value));
            return name;
        }
    }
}
