using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TDengine.Data.Client;
namespace SqlSugar.TDengineAdo
{
    public class TDengineCommand : DbCommand
    {
        private string commandText;
        private TDengineConnection connection;
        private IntPtr currentRes;
        public TDengineCommand()
        {
            // Add any required initialization logic here.
        }

        public TDengineCommand(string commandText, TDengineConnection connection)
        {
            this.CommandText = commandText;
            this.Connection = connection;
        }

        public override string CommandText
        {
            get => commandText;
            set => commandText = value;
        }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection
        {
            get => connection;
            set => connection = (TDengineConnection)value;
        }
        private TDengineParameterCollection _DbParameterCollection;
        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                if (_DbParameterCollection == null) 
                {
                    _DbParameterCollection = new TDengineParameterCollection();
                }
                return _DbParameterCollection;
            }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
            // Implement if needed
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            try
            {
                connection.Open();

                var sql = GetNoParameterSql(commandText);
                long res = connection.connection.Exec(sql);

                connection.Close();
                return res > int.MaxValue ? int.MaxValue : Convert.ToInt32(res);
            }
            catch (Exception)
            {
                connection.Close();
                throw;
            }
        }

        public override object ExecuteScalar()
        {
            try
            { 
                connection.Open();

                var sql = GetNoParameterSql(commandText);

                var res = connection.connection.Query(sql);
                using (res)
                {
                    res.Read();
                    connection.Close();
                    return res.GetValue(0);
                }
            }
            catch (Exception)
            {
                connection.Close();
                throw;
            }
        }
         
        public new DbDataReader ExecuteReader()
        {
            try
            {
                connection.Open();

                var sql = GetNoParameterSql(commandText);

                var res = connection.connection.Query(sql);
                TDengineDataReader reader = new TDengineDataReader(res); 

                connection.Close();
                return reader;
            }
            catch (Exception)
            {
                connection.Close();
                throw;
            } 
        }


        public override void Prepare()
        {
            // Implement if needed
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            //// Release the result pointer if it exists
            //if (currentRes != IntPtr.Zero)
            //{
            //    TDengine.FreeResult(connection.connection);
            //    currentRes = IntPtr.Zero;
            //}

            base.Dispose(disposing);
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return ExecuteReader();
        }

        #region Helper 
        string GetNoParameterSql(string sql)
        {
            foreach (TDengineParameter item in this.Parameters.Cast<TDengineParameter>().OrderByDescending(it => it.parameterName.Length))
            {
                if (!item.parameterName.Contains("@"))
                {
                    item.parameterName = "@" + item.parameterName;
                }
                var value = item.value;
                if (item.value == null || item.value == DBNull.Value)
                {
                    sql = Regex.Replace(sql, item.parameterName, "null", RegexOptions.IgnoreCase);
                }
                else if (item.value is DateTime)
                {
                    var dt = (DateTime)item.value;
                    if (item.IsMicrosecond)//有微妙
                    {
                        sql = Regex.Replace(sql, item.parameterName, Helper.DateTimeToLong16(dt) + "", RegexOptions.IgnoreCase);
                    }
                    else if (item.IsNanosecond)//有纳妙
                    {
                        sql = Regex.Replace(sql, item.parameterName, Helper.DateTimeToLong19(dt) + "", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        sql = Regex.Replace(sql, item.parameterName, Helper.ToUnixTimestamp(dt) + "", RegexOptions.IgnoreCase);
                    }
                }
                else if (!(item.value is string || item.value is object))
                {
                    sql = Regex.Replace(sql, item.parameterName, "'" + item.value + "'", RegexOptions.IgnoreCase);
                }
                else
                {
                    sql = Regex.Replace(sql, item.parameterName, "'" + item.value.ToString().Replace("'", "''") + "'", RegexOptions.IgnoreCase);
                }
            } 
            return sql;
        }

        #endregion
    }
}