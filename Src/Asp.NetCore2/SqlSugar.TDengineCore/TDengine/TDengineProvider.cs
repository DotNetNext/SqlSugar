using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar.TDengineAdo;
namespace SqlSugar.TDengine
{
    public partial class TDengineProvider : AdoProvider
    {
        public TDengineProvider() 
        {
            
         
        }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    { 
                        var TDengineConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new TDengineConnection(TDengineConnectionString);
                    }
                    catch (Exception)
                    {
                        throw;

                    }
                }
                return base._DbConnection;
            }
            set
            {
                base._DbConnection = value;
            }
        }

        public override void BeginTran(string transactionName)
        {
          
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
        
        }
        public override IDataAdapter GetAdapter()
        {
            return new SqlSugar.TDengineCore.TDengineDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            TDengineCommand sqlCommand = new TDengineCommand(sql, (TDengineConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            //if (this.Transaction != null)
            //{
            //    sqlCommand.Transaction = (TDengineTransaction)this.Transaction;
            //}
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((TDengineParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((TDengineDataAdapter)dataAdapter).SelectCommand = (TDengineCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            TDengineParameter[] result = new TDengineParameter[parameters.Length];
            int i = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                 
                var sqlParameter = new TDengineParameter(parameter.ParameterName,parameter.Value,parameter.DbType,0);
                result[0]=sqlParameter;
                i++;
            }
            return result;
        }
         
    }
}
