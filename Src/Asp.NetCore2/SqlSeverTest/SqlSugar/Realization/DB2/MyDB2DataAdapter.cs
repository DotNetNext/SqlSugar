using IBM.Data.DB2.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SqlSugar.Realization.DB2
{
    class MyDB2DataAdapter : IDataAdapter
    {
        private DB2Command command;
        private string sql;
        private DB2Connection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public MyDB2DataAdapter(DB2Command command)
        {
            this.command = command;
        }

        public MyDB2DataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public MyDB2DataAdapter(string sql, DB2Connection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public DB2Command SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new DB2Command(this.sql, this._sqlConnection);
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
            using (DB2DataReader dr = command.ExecuteReader())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    string name = dr.GetName(i).Trim();
                    if (!columns.Contains(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
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
            using (DB2DataReader dr = command.ExecuteReader())
            {
                do
                {
                    var dt = new DataTable();
                    var columns = dt.Columns;
                    var rows = dt.Rows;
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string name = dr.GetName(i).Trim();
                        if (!columns.Contains(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
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
                    ds.Tables.Add(dt);
                } while (dr.NextResult());
            }
        }
    }
}
