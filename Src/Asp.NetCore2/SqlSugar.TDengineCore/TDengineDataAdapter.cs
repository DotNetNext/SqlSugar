using SqlSugar.TDengineAdo;
using System;
using System.Data.Common;
using System.Data;

namespace SqlSugar.TDengineCore
{
    /// <summary>
    /// 数据填充器
    /// </summary>
    public class TDengineDataAdapter : IDataAdapter
    {
        private TDengineCommand command;
        private string sql;
        private TDengineConnection _TDengineConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public TDengineDataAdapter(TDengineCommand command)
        {
            this.command = command;
        }

        public TDengineDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_TDengineConnection"></param>
        public TDengineDataAdapter(string sql, TDengineConnection _TDengineConnection)
        {
            this.sql = sql;
            this._TDengineConnection = _TDengineConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public TDengineCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new TDengineCommand(this.sql, this._TDengineConnection);
                }
                return this.command;
            }
            set
            {
                this.command = value;
            }
        }

        public MissingMappingAction MissingMappingAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public MissingSchemaAction MissingSchemaAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ITableMappingCollection TableMappings => throw new NotImplementedException();

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
            using (DbDataReader dr = command.ExecuteReader())
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
            using (DbDataReader dr = command.ExecuteReader())
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
                    dt.AcceptChanges();
                    ds.Tables.Add(dt);
                } while (dr.NextResult());
            }
        } 

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }
         
    }
}
