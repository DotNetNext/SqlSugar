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
            
            var dt= this.Context.Ado.GetDataTable("select top 0 * from " + InsertBuilder.GetTableNameString);
            foreach (var rowInfos in DbColumnInfoList)
            {
                var dr = dt.NewRow();
                foreach (DataColumn item in dt.Columns)
                {
                    var rows= rowInfos.ToList();
                    var value = rows.FirstOrDefault(it => 
                                                             it.DbColumnName.Equals(item.ColumnName, StringComparison.CurrentCultureIgnoreCase)||
                                                             it.PropertyName.Equals(item.ColumnName, StringComparison.CurrentCultureIgnoreCase)
                                                        );
                    if (value != null)
                    {
                        if (value.Value != null && UtilMethods.GetUnderType(value.Value.GetType()) == UtilConstants.DateType)
                        {
                            if (value.Value != null && value.Value.ToString() == DateTime.MinValue.ToString())
                            {
                                value.Value = Convert.ToDateTime("1753/01/01");
                            }
                        }
                        dr[item.ColumnName] = value.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            SqlBulkCopy bulkCopy = null;
            if (this.Context.Ado.Transaction != null)
            {
                var sqlTran = this.Context.Ado.Transaction as SqlTransaction;
                bulkCopy = new SqlBulkCopy(this.Context.Ado.Connection as SqlConnection,SqlBulkCopyOptions.CheckConstraints, sqlTran);
            }
            else
            {
                bulkCopy=new SqlBulkCopy(this.Context.Ado.Connection as SqlConnection);
            }
            //获取目标表的名称
            bulkCopy.DestinationTableName = InsertBuilder.GetTableNameString;
            //写入DataReader对象
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            try
            {
                bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                this.Context.Ado.Connection.Close();
                throw ex;
            }
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
