using System.Data;
using XuguClient;

namespace SqlSugar.Xugu
{
    /// <summary>
    /// 虚谷数据填充器
    /// </summary>
    public class XuguDataAdapter : IDataAdapter
    {
        private string sql;
        private XGConnection _sqlConnection;
        private XGCommand command;
        /// <summary>
        /// 构造函数
        /// </summary>
        public XuguDataAdapter() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="command">SQL命令</param>
        public XuguDataAdapter(XGCommand command) => this.command = command;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="_sqlConnection">连接对象</param>
        public XuguDataAdapter(string sql, XGConnection _sqlConnection)
        {
            this.sql = sql;
            this._sqlConnection = _sqlConnection;
        }
        /// <summary>
        /// 查询SQL命令对象
        /// </summary>
        public XGCommand SelectCommand
        {
            get
            {
                if (this.command == null)
                {
                    this.command = this._sqlConnection.CreateCommand();
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
        /// 填充数据（当前集合）
        /// </summary>
        /// <param name="dt">填充目标DataTable</param>
        private DataTable Fill(XGDataReader dr, DataTable dt = null)
        {
            if (dt == null) dt = new DataTable();
            var columns = dt.Columns;
            //构造列
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string name = dr.GetName(i).Trim();
                //重名时的处理
                if (!columns.Contains(name)) name += i;
                columns.Add(new DataColumn(name, dr.GetFieldType(i)));
            }
            //填充行
            while (dr.Read())
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < columns.Count; i++)
                    row[columns[i].ColumnName] = dr.GetValue(i);
                dt.Rows.Add(row);
            }
            dt.AcceptChanges();
            return dt;
        }
        /// <summary>
        /// 填充数据（单个集合）
        /// </summary>
        /// <param name="dt">填充目标DataTable</param>
        public void Fill(DataTable dt) { using (var dr = command.ExecuteReader()) Fill(dt); }
        /// <summary>
        /// 填充数据（多个集合）
        /// </summary>
        /// <param name="dt">填充目标DataSet</param>
        public void Fill(DataSet ds)
        {
            if (ds == null) ds = new DataSet();
            using (var dr = command.ExecuteReader())
            {
                do ds.Tables.Add(Fill(dr));
                while (dr.NextResult());
            }
        }
    }
}