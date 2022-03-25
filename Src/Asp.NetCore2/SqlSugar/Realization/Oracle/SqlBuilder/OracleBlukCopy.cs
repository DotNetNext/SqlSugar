using System;

using System.Collections.Generic;

using System.Data;

using Oracle.ManagedDataAccess.Client;

using System.Linq;

using System.Text;

using System.Threading.Tasks;


namespace SqlSugar
{

    public class OracleBlukCopy

    {

        internal List<IGrouping<int, DbColumnInfo>> DbColumnInfoList { get; set; }

        internal SqlSugarProvider Context { get; set; }

        internal ISqlBuilder Builder { get; set; }

        internal InsertBuilder InsertBuilder { get; set; }

        internal object[] Inserts { get; set; }



        public int ExecuteBulkCopy()

        {

            if (DbColumnInfoList == null || DbColumnInfoList.Count == 0) return 0;



            if (Inserts.First().GetType() == typeof(DataTable))

            {

                return WriteToServer();

            }

            DataTable dt = GetCopyData();

            OracleBulkCopy bulkCopy = GetBulkCopyInstance();

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



        public async Task<int> ExecuteBulkCopyAsync()

        {

            if (DbColumnInfoList == null || DbColumnInfoList.Count == 0) return 0;



            if (Inserts.First().GetType() == typeof(DataTable))

            {

                return WriteToServer();

            }

            DataTable dt = GetCopyData();

            OracleBulkCopy bulkCopy = GetBulkCopyInstance();

            bulkCopy.DestinationTableName = InsertBuilder.GetTableNameString;

            try

            {

                await Task.Run(() => bulkCopy.WriteToServer(dt));

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

            OracleBulkCopy copy = GetBulkCopyInstance();

            copy.DestinationTableName = this.Builder.GetTranslationColumnName(dt.TableName);

            copy.WriteToServer(dt);

            CloseDb();

            return dt.Rows.Count;

        }

        private DataTable GetCopyWriteDataTable(DataTable dt)

        {

            var result = this.Context.Ado.GetDataTable("select * from " + this.Builder.GetTranslationColumnName(dt.TableName) + " where 1 > 2 ");

            foreach (DataRow item in dt.Rows)

            {

                DataRow dr = result.NewRow();

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

        private OracleBulkCopy GetBulkCopyInstance()

        {
            if (this.Context.Ado.Connection.State == ConnectionState.Closed)

            {

                this.Context.Ado.Connection.Open();

            }

            OracleBulkCopy copy;

            if (this.Context.Ado.Transaction == null)

            {

                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, Oracle.ManagedDataAccess.Client.OracleBulkCopyOptions.Default);

            }

            else

            {

                copy = new OracleBulkCopy((OracleConnection)this.Context.Ado.Connection, OracleBulkCopyOptions.UseInternalTransaction);

            }
            return copy;

        }

        private DataTable GetCopyData()

        {

            var dt = this.Context.Ado.GetDataTable("select  * from " + InsertBuilder.GetTableNameString + " where 1 > 2 ");

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

                                value.Value = Convert.ToDateTime("1900/01/01");

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
            if (this.InsertBuilder.OracleSeqInfoList != null && this.InsertBuilder.OracleSeqInfoList.Any()) 
            {
                var ids = this.InsertBuilder.OracleSeqInfoList.Select(it => it.Value).ToList();
                var columnInfo = this.InsertBuilder.EntityInfo.Columns.Where(it => !string.IsNullOrEmpty(it.OracleSequenceName)).First();
                var identityName = columnInfo.DbColumnName;
                ids.Add(this.Context.Ado.GetInt(" select " + columnInfo.OracleSequenceName + ".nextval from dual"));
                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    item[identityName] = ids[i];
                    ++i;
                }
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
