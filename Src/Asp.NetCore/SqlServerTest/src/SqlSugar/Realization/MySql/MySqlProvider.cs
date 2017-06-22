//using System;
//using System.Data;
//using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

//namespace SqlSugar
//{
//    public class MySqlProvider : AdoProvider
//    {
//        public MySqlProvider() {}
//        public override IDbConnection Connection
//        {
//            get
//            {
//                if (base._DbConnection == null)
//                {
//                    base._DbConnection = new MySqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
//                }
//                return base._DbConnection;
//            }
//            set
//            {
//                base._DbConnection = value;
//            }
//        }
//        /// <summary>
//        /// Only SqlServer
//        /// </summary>
//        /// <param name="transactionName"></param>
//        public override void BeginTran(string transactionName)
//        {
//            throw  new NotImplementedException();
//        }
//        /// <summary>
//        /// Only SqlServer
//        /// </summary>
//        /// <param name="iso"></param>
//        /// <param name="transactionName"></param>
//        public override void BeginTran(IsolationLevel iso, string transactionName)
//        {
//            throw new NotImplementedException();
//        }
//        public override IDataAdapter GetAdapter()
//        {
//            return new MySqlDataAdapter();
//        }
//        public override IDbCommand GetCommand(string sql, SugarParameter[] parameters)
//        {
//            MySqlCommand sqlCommand = new MySqlCommand(sql, (MySqlConnection)this.Connection);
//            sqlCommand.CommandType = this.CommandType;
//            sqlCommand.CommandTimeout = this.CommandTimeOut;
//            if (this.Transaction != null)
//            {
//                sqlCommand.Transaction = (MySqlTransaction)this.Transaction;
//            }
//            if (parameters.IsValuable())
//            {
//                IDataParameter[] ipars= ToIDbDataParameter(parameters);
//                sqlCommand.Parameters.AddRange((SqlParameter[])ipars);
//            }
//            CheckConnection();
//            return sqlCommand;
//        }
//        public override void SetCommandToAdapter(IDataAdapter dataAdapter, IDbCommand command)
//        {
//            ((MySqlDataAdapter)dataAdapter).SelectCommand = (MySqlCommand)command;
//        }
//        /// <summary>
//        /// if mysql return MySqlParameter[] pars
//        /// if sqlerver return SqlParameter[] pars ...
//        /// </summary>
//        /// <param name="parameters"></param>
//        /// <returns></returns>
//        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
//        {
//            if (parameters == null || parameters.Length == 0) return null;
//            SqlParameter[] result = new SqlParameter[parameters.Length];
//            int i = 0;
//            foreach (var paramter in parameters)
//            {
//                if (paramter.Value == null) paramter.Value = DBNull.Value;
//                var p = new SqlParameter();
//                p.ParameterName = paramter.ParameterName;
//                p.UdtTypeName = paramter.UdtTypeName;
//                p.Size = paramter.Size;
//                p.Value = paramter.Value;
//                p.DbType = paramter.DbType;
//                p.Direction = paramter.Direction;
//                result[i] =p;
//                ++i;
//            }
//            return result;
//        }
//    }
//}
