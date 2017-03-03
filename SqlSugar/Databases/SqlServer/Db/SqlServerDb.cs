using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class SqlServerDb : DbProvider
    {
        public SqlServerDb() {}
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    base._DbConnection = new SqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
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
            ((SqlConnection)this.Connection).BeginTransaction(transactionName);
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            ((SqlConnection)this.Connection).BeginTransaction(iso, transactionName);
        }
        public override IDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }
        public override IDbCommand GetCommand(string sql, SugarParameter[] pars)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, (SqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (SqlTransaction)this.Transaction;
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
            ((SqlDataAdapter)dataAdapter).SelectCommand = (SqlCommand)command;
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
