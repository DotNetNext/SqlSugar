using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text;
namespace SqlSugar.GBase 
{
    /// <summary>
    /// 数据填充器
    /// </summary>
    public class GBaseDataAdapter : IDataAdapter
    {
        private OdbcCommand command;
        private string sql;
        private OdbcConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public GBaseDataAdapter(OdbcCommand command)
        {
            this.command = command;
        }

        public GBaseDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public GBaseDataAdapter(string sql, OdbcConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public OdbcCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    var conn = (OdbcConnection)this._sqlConnection;
                    this.command = conn.CreateCommand();
                    this.command.CommandText = sql;
                }
                return this.command;
            }
            set
            {
                this.command = value;
            }
        }

        /// <summary>
        /// Fill
        /// </summary>
        /// <param name="dt"></param>
        public void Fill(DataTable dt)
        {
            if (dt == null)
            {
                dt = new DataTable();
            }
            var columns = dt.Columns;
            var rows = dt.Rows;
            using (var dr = command.ExecuteReader())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i).Trim();
                    if (!columns.Contains(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    else
                    {
                        columns.Add(new DataColumn(name + i, dr.GetFieldType(i)));
                    }
                }

                while (dr.Read())
                {
                    DataRow daRow = dt.NewRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        daRow[columns[i].ColumnName] = dr.GetValue(i);
                    }
                    dt.Rows.Add(daRow);
                }
            }
            dt.AcceptChanges();
        }

        /// <summary>
        /// Fill
        /// </summary>
        /// <param name="ds"></param>
        public void Fill(DataSet ds)
        {
            if (ds == null)
            {
                ds = new DataSet();
            }
            using (var dr = command.ExecuteReader())
            {
                do
                {
                    var dt = new DataTable();
                    var columns = dt.Columns;
                    var rows = dt.Rows;
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string name = dr.GetName(i).Trim();
                        if (dr.GetFieldType(i).Name == "DateTime")
                        {
                            if (!columns.Contains(name))
                                columns.Add(new DataColumn(name, UtilConstants.DateType));
                            else
                            {
                                columns.Add(new DataColumn(name + i, UtilConstants.DateType));
                            }
                        }
                        else
                        {
                            if (!columns.Contains(name))
                                columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                            else
                            {
                                columns.Add(new DataColumn(name + i, dr.GetFieldType(i)));
                            }
                        }
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = dt.NewRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            daRow[columns[i].ColumnName] = dr.GetValue(i);
                        }
                        dt.Rows.Add(daRow);
                    }
                    dt.AcceptChanges();
                    ds.Tables.Add(dt);
                } while (dr.NextResult());
            }
        }
    }

}
