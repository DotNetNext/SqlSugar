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
        public override void BeginTran()
        {
            
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
            ((SqlSugar.TDengineCore.TDengineDataAdapter)dataAdapter).SelectCommand = (TDengineCommand)command;
        }
        public static bool _IsIsNanosecond { get; set; }
        public static bool _IsMicrosecond { get; set; }
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
                if (parameter.Value is bool) 
                {
                    parameter.Value = parameter.Value?.ToString()?.ToLower();
                }
                var sqlParameter = new TDengineParameter(parameter.ParameterName,parameter.Value,parameter.DbType,0);
                if (parameter.CustomDbType?.Equals(System.Data.DbType.DateTime2) == true|| (parameter.Value is DateTime&&_IsMicrosecond))
                {
                    sqlParameter.IsMicrosecond = true;
                }
                else if (parameter.CustomDbType?.Equals(typeof(Date19)) == true|| (parameter.Value is DateTime && _IsIsNanosecond))
                {
                    sqlParameter.IsNanosecond = true;
                }
                else if (parameter.Value is DateTime&&this.Context.CurrentConnectionConfig.ConnectionString.Contains("config_")) 
                {
                    _IsIsNanosecond=sqlParameter.IsNanosecond = this.Context.CurrentConnectionConfig.ConnectionString.Contains("config_ns");
                    _IsMicrosecond = sqlParameter.IsMicrosecond = this.Context.CurrentConnectionConfig.ConnectionString.Contains("config_us");
                }
                result[i]=sqlParameter;
                i++;
            }
            return result;
        }
         
    }
}
