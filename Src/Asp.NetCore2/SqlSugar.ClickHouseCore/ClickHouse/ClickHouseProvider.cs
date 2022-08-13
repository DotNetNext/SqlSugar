using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace SqlSugar.ClickHouse
{
    public partial class ClickHouseProvider : AdoProvider
    {
        public ClickHouseProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var ClickHouseConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new ClickHouseConnection(ClickHouseConnectionString);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
            //No Support
        }
        public override void BeginTran(string transactionName)
        {
            //No Support
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            //No Support
        }
        public override void CommitTran()
        {
            //No Support
        }
        public override void RollbackTran()
        {
            //No Support
        }
        public override IDataAdapter GetAdapter()
        {
            return new ClickHouseDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var connection=(ClickHouseConnection)this.Connection;
            CheckConnection();
            IDataParameter[] ipars = ToIDbDataParameter(parameters);
            ClickHouseCommand sqlCommand =connection.CreateCommand();
            var pars = ToIDbDataParameter(parameters);
            sqlCommand.CommandText = sql;
            sqlCommand.Parameters.AddRange(pars);
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((ClickHouseDataAdapter)dataAdapter).SelectCommand = (ClickHouseCommand)command;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return new ClickHouseDbParameter[] { };
            ClickHouseDbParameter[] result = new ClickHouseDbParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if(parameter.Value is System.Data.SqlTypes.SqlDateTime&&parameter.DbType==System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                var sqlParameter = new ClickHouseDbParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                ++index;
                
            }
            return result;
        }
 
    }
}
