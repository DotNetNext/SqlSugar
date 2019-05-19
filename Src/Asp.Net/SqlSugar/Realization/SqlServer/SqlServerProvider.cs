using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class SqlServerProvider : AdoProvider
    {
        public SqlServerProvider() { }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        base._DbConnection = new SqlConnection(base.Context.CurrentConnectionConfig.ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        Check.Exception(true, ErrorMessage.ConnnectionOpen, ex.Message);
                    }
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
            CheckConnection();
            base.Transaction = ((SqlConnection)this.Connection).BeginTransaction(transactionName);
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            CheckConnection();
            base.Transaction = ((SqlConnection)this.Connection).BeginTransaction(iso, transactionName);
        }
        public override IDataAdapter GetAdapter()
        {
            return new SqlDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(sql, (SqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (SqlTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                SqlParameter[] ipars = GetSqlParameter(parameters);
                sqlCommand.Parameters.AddRange(ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((SqlDataAdapter)dataAdapter).SelectCommand = (SqlCommand)command;
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
            SqlParameter[] result = new SqlParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput,ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                ++index;
            }
            return result;
        }
        /// <summary>
        /// if mysql return MySqlParameter[] pars
        /// if sqlerver return SqlParameter[] pars ...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlParameter[] GetSqlParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            SqlParameter[] result = new SqlParameter[parameters.Length];
            int index = 0;
            foreach (var parameter in parameters)
            {
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                var sqlParameter = new SqlParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.UdtTypeName = parameter.UdtTypeName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                if (sqlParameter.Value!=null&& sqlParameter.Value != DBNull.Value && sqlParameter.DbType == System.Data.DbType.DateTime)
                {
                    var date = Convert.ToDateTime(sqlParameter.Value);
                    if (date==DateTime.MinValue)
                    {
                        sqlParameter.Value = Convert.ToDateTime("1753/01/01");
                    }
                }
                sqlParameter.Direction = parameter.Direction;
                result[index] = sqlParameter;
                if (parameter.TypeName.HasValue()) {
                    sqlParameter.TypeName = parameter.TypeName;
                    sqlParameter.SqlDbType = SqlDbType.Structured;
                    sqlParameter.DbType = System.Data.DbType.Object;
                }
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                ++index;
            }
            return result;
        }
    }
}
