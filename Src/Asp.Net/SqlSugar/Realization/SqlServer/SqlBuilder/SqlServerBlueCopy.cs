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
        internal object[] Inserts { get;  set; }

        public int ExecuteBlueCopy()
        {
            if (DbColumnInfoList == null || DbColumnInfoList.Count == 0) return 0;

            if (Inserts.First().GetType() == typeof(DataTable))
            {
                return WriteToServer();
            }
            DataTable dt = GetCopyData();
            SqlBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = InsertBuilder.GetTableNameString;
            try
            {
                bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return DbColumnInfoList.Count;
        }

        public async Task<int> ExecuteBlueCopyAsync()
        {
            if (DbColumnInfoList == null || DbColumnInfoList.Count == 0) return 0;

            if (Inserts.First().GetType() == typeof(DataTable))
            {
                return WriteToServer();
            }
            DataTable dt=GetCopyData();
            SqlBulkCopy bulkCopy = GetBulkCopyInstance();
            bulkCopy.DestinationTableName = InsertBuilder.GetTableNameString;
            try
            {
                await bulkCopy.WriteToServerAsync(dt);
            }
            catch (Exception ex)
            {
                CloseDb();
                throw ex;
            }
            CloseDb();
            return DbColumnInfoList.Count;
        }

        private int WriteToServer()
        {
            var dt = this.Inserts.First() as DataTable;
            if (dt == null)
                return 0;
            Check.Exception(dt.TableName == "Table", "dt.TableName can't be null ");
            dt = GetCopyWriteDataTable(dt);
            SqlBulkCopy copy = GetBulkCopyInstance();
            copy.DestinationTableName = this.Builder.GetTranslationColumnName(dt.TableName);
            copy.WriteToServer(dt);
            CloseDb();
            return dt.Rows.Count;
        }
        private DataTable GetCopyWriteDataTable(DataTable dt)
        {
            var result = this.Context.Ado.GetDataTable("select top 0 * from " + this.Builder.GetTranslationColumnName(dt.TableName));
            foreach (DataRow item in dt.Rows)
            {
                DataRow  dr= result.NewRow();
                foreach (DataColumn column in result.Columns)
                {

                    if (dt.Columns.Cast<DataColumn>().Select(it => it.ColumnName.ToLower()).Contains(column.ColumnName.ToLower()))
                    {
                        dr[column.ColumnName] = item[column.ColumnName];
                        if (dr[column.ColumnName] == null)
                        {
                            dr[column.ColumnName] = DBNull.Value;
                        }
                    }
                }
                result.Rows.Add(dr);
            }
            result.TableName = dt.TableName;
            return result;
        }
        private SqlBulkCopy GetBulkCopyInstance()
        {
            SqlBulkCopy copy;
            if (this.Context.Ado.Transaction == null)
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection);
            }
            else
            {
                copy = new SqlBulkCopy((SqlConnection)this.Context.Ado.Connection, SqlBulkCopyOptions.CheckConstraints, (SqlTransaction)this.Context.Ado.Transaction);
            }
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)
            {
                this.Context.Ado.Connection.Open();
            }
            return copy;
        }
        private DataTable GetCopyData()
        {
            var dt = this.Context.Ado.GetDataTable("select top 0 * from " + InsertBuilder.GetTableNameString);
            foreach (var rowInfos in DbColumnInfoList)
            {
                var dr = dt.NewRow();
                foreach (DataColumn item in dt.Columns)
                {
                    var rows = rowInfos.ToList();
                    var value = rows.FirstOrDefault(it =>
                                                             it.DbColumnName.Equals(item.ColumnName, StringComparison.CurrentCultureIgnoreCase) ||
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
                        if (value.Value == null)
                        {
                            value.Value = DBNull.Value;
                        }
                        dr[item.ColumnName] = value.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        private void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Ado.Connection.Close();
            }
        }
    }
}
