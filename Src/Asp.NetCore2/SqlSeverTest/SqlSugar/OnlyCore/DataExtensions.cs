using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
namespace SqlSugar
{

    /// <summary>
    /// 数据填充器
    /// </summary>
    public class SqlDataAdapter : IDataAdapter
    {
        private SqlCommand command;
        private string sql;
        private SqlConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public SqlDataAdapter(SqlCommand command)
        {
            this.command = command;
        }

        public SqlDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public SqlDataAdapter(string sql, SqlConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public SqlCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new SqlCommand(this.sql, this._sqlConnection);
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
            using (SqlDataReader dr = command.ExecuteReader())
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
            using (SqlDataReader dr = command.ExecuteReader())
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
    /// <summary>
    /// 数据填充器
    /// </summary>
    public class MySqlDataAdapter : IDataAdapter
    {
        private MySqlCommand command;
        private string sql;
        private MySqlConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public MySqlDataAdapter(MySqlCommand command)
        {
            this.command = command;
        }

        public MySqlDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public MySqlDataAdapter(string sql, MySqlConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public MySqlCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new MySqlCommand(this.sql, this._sqlConnection);
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
            using (MySqlDataReader dr = command.ExecuteReader())
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
            using (MySqlDataReader dr = command.ExecuteReader())
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

    /// <summary>
    /// 数据填充器
    /// </summary>
    public class SqliteDataAdapter : IDataAdapter
    {
        private SqliteCommand command;
        private string sql;
        private SqliteConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public SqliteDataAdapter(SqliteCommand command)
        {
            this.command = command;
        }

        public SqliteDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public SqliteDataAdapter(string sql, SqliteConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public SqliteCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new SqliteCommand(this.sql, this._sqlConnection);
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
            using (SqliteDataReader dr = command.ExecuteReader())
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
            using (SqliteDataReader dr = command.ExecuteReader())
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

    /// <summary>
    /// 数据填充器
    /// </summary>
    public class MyOracleDataAdapter : IDataAdapter
    {
        private OracleCommand command;
        private string sql;
        private OracleConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public MyOracleDataAdapter(OracleCommand command)
        {
            this.command = command;
        }

        public MyOracleDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public MyOracleDataAdapter(string sql, OracleConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public OracleCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new OracleCommand(this.sql, this._sqlConnection);
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
            using (OracleDataReader dr = command.ExecuteReader())
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
            using (OracleDataReader dr = command.ExecuteReader())
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

    /// <summary>
    /// 数据填充器
    /// </summary>
    public class NpgsqlDataAdapter : IDataAdapter
    {
        private NpgsqlCommand command;
        private string sql;
        private NpgsqlConnection _sqlConnection;

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="command"></param>
        public NpgsqlDataAdapter(NpgsqlCommand command)
        {
            this.command = command;
        }

        public NpgsqlDataAdapter()
        {

        }

        /// <summary>
        /// SqlDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="_sqlConnection"></param>
        public NpgsqlDataAdapter(string sql, NpgsqlConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }

        /// <summary>
        /// SelectCommand
        /// </summary>
        public NpgsqlCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = new NpgsqlCommand(this.sql, this._sqlConnection);
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
            using (NpgsqlDataReader dr = command.ExecuteReader())
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
            using (NpgsqlDataReader dr = command.ExecuteReader())
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