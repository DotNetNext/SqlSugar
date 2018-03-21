using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
namespace SqlSugar
{
    /// <summary>
    /// 作者：sunkaixuan 
    /// 创建时间：2016/7/31
    /// 修改时间：-
    /// 说明：让.netCore支持DataSet
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// 数据表
        /// </summary>
        public List<DataTable> Tables = new List<DataTable>();
    }

    /// <summary>
    /// 作者：sunkaixuan 
    /// 创建时间：2016/7/31
    /// 修改时间：-
    /// 说明：让.netCore支持DataTable
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// 列信息
        /// </summary>
        public DataColumnCollection Columns = new DataColumnCollection();
        /// <summary>
        /// 行信息
        /// </summary>
        public DataRowCollection Rows = new DataRowCollection();
    }
    /// <summary>
    /// 数据列
    /// </summary>
    public class DataColumn
    {
        /// <summary>
        /// Data Column
        /// </summary>
        public DataColumn()
        {

        }
        /// <summary>
        /// Column Name
        /// </summary>
        /// <param name="columnName"></param>
        public DataColumn(string columnName)
        {
            this.ColumnName = columnName;
        }
        /// <summary>
        /// Data Column
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        public DataColumn(string columnName, Type dataType)
        {
            this.ColumnName = columnName;
            this.DataType = dataType;
        }
        /// <summary>
        /// Column Name
        /// </summary>
        public string ColumnName { get; internal set; }
        /// <summary>
        /// Data Type
        /// </summary>
        public Type DataType { get; internal set; }
    }
    /// <summary>
    /// 数据列集合
    /// </summary>
    public class DataColumnCollection : IEnumerable, ICollection, IEnumerator
    {
        /// <summary>
        /// Get Item By Index
        /// </summary>
        /// <param name="thisIndex"></param>
        /// <returns></returns>
        public DataColumn this[int thisIndex]
        {
            get
            {
                return cols[thisIndex];
            }
        }
        private int index = -1;
        private List<DataColumn> cols;
        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get
            {
                if (this.cols == null)
                {
                    this.cols = new List<DataColumn>();
                }
                return this.cols.Count;
            }
        }
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="col"></param>
        public void Add(DataColumn col)
        {
            if (this.cols == null)
            {
                this.cols = new List<DataColumn>();
            }
            this.cols.Add(col);
        }
        /// <summary>
        /// Is Synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Sync Root
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Current
        /// </summary>
        public object Current
        {
            get
            {
                return cols[index];
            }
        }
        /// <summary>
        /// Copy To
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取该集合的 System.Collections.IEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this; ;
        }
        /// <summary>
        /// Move Next
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            index++;
            var isNext = index < cols.Count;
            if (!isNext)
                Reset();
            return isNext;
        }
        /// <summary>
        /// Reset
        /// </summary>

        public void Reset()
        {
            index = -1;
        }
        /// <summary>
        /// Contains Key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        public bool ContainsKey(string name)
        {
            if (this.cols == null) return false;
            return (this.cols.Any(it => it.ColumnName == name));
        }
    }
    /// <summary>
    /// 数据行集合
    /// </summary>
    public class DataRowCollection : IEnumerable, ICollection, IEnumerator
    {
        /// <summary>
        /// Get item by index
        /// </summary>
        /// <param name="thisIndex"></param>
        /// <returns></returns>
        public DataRow this[int thisIndex]
        {
            get
            {
                return Rows[thisIndex];
            }
        }
        private int index = -1;
        private List<DataRow> Rows = null;
        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows.Count;
            }
        }

        /// <summary>
        /// Current
        /// </summary>
        public object Current
        {
            get
            {
                if (this.Rows == null)
                {
                    this.Rows = new List<DataRow>();
                }
                return Rows[index];
            }
        }
        /// <summary>
        /// Is Synchronized
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Sync Root
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Copy To
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取该集合的 System.Collections.IEnumerator。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this; ;
        }

        /// <summary>
        /// Move Next
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            index++;
            var isNext = index < (Rows == null ? 0 : Rows.Count);
            if (!isNext)
                Reset();
            return isNext;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            index = -1;
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="daRow"></param>
        internal void Add(DataRow daRow)
        {
            if (Rows == null)
            {
                Rows = new List<DataRow>();
            }
            Rows.Add(daRow);
        }
    }
    /// <summary>
    /// 数据行
    /// </summary>

    public class DataRow
    {
        private Dictionary<string, object> obj = new Dictionary<string, object>();

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object value)
        {
            obj.Add(key, value);
        }

        /// <summary>
        /// Get Item  By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return obj[name];
            }
        }

        public object this[DataColumn column]
        {
            get
            {
                return obj[column.ColumnName];
            }
        }

        /// <summary>
        /// Get Item By Index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                int i = 0;
                object reval = null;
                foreach (var item in obj)
                {
                    if (i == index)
                    {
                        reval = item.Value;
                        break;
                    }
                    i++;
                }
                return reval;
            }
        }

        /// <summary>
        /// Contains Key
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public bool ContainsKey(string columnName)
        {
            if (this.obj == null) return false;
            return (this.obj.ContainsKey(columnName));
        }
    }
    /// <summary>
    /// 数据填充器
    /// </summary>
    public class SqlDataAdapter: IDataAdapter
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
            set {
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
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }

                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                        if (!columns.ContainsKey(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = new DataRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (!daRow.ContainsKey(columns[i].ColumnName))
                                daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }

                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                        if (!columns.ContainsKey(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = new DataRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (!daRow.ContainsKey(columns[i].ColumnName))
                                daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }

                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                        if (!columns.ContainsKey(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = new DataRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (!daRow.ContainsKey(columns[i].ColumnName))
                                daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                    if (!columns.ContainsKey(name))
                        columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                }

                while (dr.Read())
                {
                    DataRow daRow = new DataRow();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (!daRow.ContainsKey(columns[i].ColumnName))
                            daRow.Add(columns[i].ColumnName, dr.GetValue(i));
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
                        if (!columns.ContainsKey(name))
                            columns.Add(new DataColumn(name, dr.GetFieldType(i)));
                    }

                    while (dr.Read())
                    {
                        DataRow daRow = new DataRow();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (!daRow.ContainsKey(columns[i].ColumnName))
                                daRow.Add(columns[i].ColumnName, dr.GetValue(i));
                        }
                        dt.Rows.Add(daRow);
                    }
                    ds.Tables.Add(dt);
                } while (dr.NextResult());
            }
        }
    }
}