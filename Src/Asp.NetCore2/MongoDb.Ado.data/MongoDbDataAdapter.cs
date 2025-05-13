using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace MongoDb.Ado.data
{
    /// <summary>
    /// 数据填充器
    /// </summary>
    public class MongoDbDataAdapter : IDataAdapter
    {
        private MongoDbCommand command;
        private string sql;
        private MongoDbConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public MongoDbDataAdapter(MongoDbCommand command)
        {
            this.command = command;
        }

        public MongoDbDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public MongoDbDataAdapter(string sql, MongoDbConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public MongoDbCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new MongoDbCommand(this.sql, this._sqlConnection);
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
                if (dr.Read())
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
                    AddRow(dt, columns, dr);
                    while (dr.Read())
                    {
                        AddRow(dt, columns, dr);
                    }
                }
            }
            dt.AcceptChanges();
        }

        private static void AddRow(DataTable dt, DataColumnCollection columns, DbDataReader dr)
        {
            DataRow daRow = dt.NewRow();
            for (int i = 0; i < columns.Count; i++)
            {
                var value = dr.GetValue(i);
                if(value is  ObjectId) 
                {
                    value = value?.ToString();
                }
                daRow[columns[i].ColumnName] = value;
            }
            dt.Rows.Add(daRow);
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
            var dt = new DataTable();
            using (DbDataReader dr = command.ExecuteReader())
            {
                var columns = dt.Columns;
                var rows = dt.Rows;
                if (dr.Read())
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
                    AddRow(dt, columns, dr);
                    while (dr.Read())
                    {
                        AddRow(dt, columns, dr);
                    }
                }
                ds.Tables.Add(dt);
            }
        }

        int IDataAdapter.Fill(DataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }

        public IDataParameter[] GetFillParameters()
        {
            throw new NotImplementedException();
        }

        public int Update(DataSet dataSet)
        {
            throw new NotImplementedException();
        }
    }
}
