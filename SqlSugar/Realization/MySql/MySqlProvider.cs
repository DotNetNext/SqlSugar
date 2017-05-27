using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace SqlSugar
{
    public class MySqlProvider : AdoProvider
    {
        public MySqlProvider() {}
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    base._DbConnection = new MySqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="transactionName"></param>
        public override void BeginTran(string transactionName)
        {
            throw  new NotImplementedException();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            throw new NotImplementedException();
        }
        public override IDataAdapter GetAdapter()
        {
            return new MySqlDataAdapter();
        }
        public override IDbCommand GetCommand(string sql, SugarParameter[] pars)
        {
            MySqlCommand sqlCommand = new MySqlCommand(sql, (MySqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (MySqlTransaction)this.Transaction;
            }
            if (pars.IsValuable())
            {
                IDataParameter[] ipars= ToIDbDataParameter(pars);
                sqlCommand.Parameters.AddRange((SqlParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, IDbCommand command)
        {
            ((MySqlDataAdapter)dataAdapter).SelectCommand = (MySqlCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] pars)
        {
            if (pars == null || pars.Length == 0) return null;
            SqlParameter[] reval = new SqlParameter[pars.Length];
            int i = 0;
            foreach (var par in pars)
            {
                if (par.Value == null) par.Value = DBNull.Value;
                var p = new SqlParameter();
                p.ParameterName = par.ParameterName;
                p.UdtTypeName = par.UdtTypeName;
                p.Size = par.Size;
                p.Value = par.Value;
                p.DbType = par.DbType;
                reval[i] =p;
                ++i;
            }
            return reval;
        }
    }
}
