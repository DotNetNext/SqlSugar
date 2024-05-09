using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace SqlSugar
{
    public partial class PostgreSQLProvider : AdoProvider
    {
        public PostgreSQLProvider() 
        {

            if (StaticConfig.AppContext_ConvertInfinityDateTime == false)
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            }
         
        }
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var npgsqlConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new NpgsqlConnection(npgsqlConnectionString);
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

        public override void BeginTran(string transactionName)
        {
            base.BeginTran();
        }
        /// <summary>
        /// Only SqlServer
        /// </summary>
        /// <param name="iso"></param>
        /// <param name="transactionName"></param>
        public override void BeginTran(IsolationLevel iso, string transactionName)
        {
            base.BeginTran(iso);
        }
        public override IDataAdapter GetAdapter()
        {
            return new NpgsqlDataAdapter();
        }
        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            NpgsqlCommand sqlCommand = new NpgsqlCommand(sql, (NpgsqlConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (NpgsqlTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((NpgsqlParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }
        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((NpgsqlDataAdapter)dataAdapter).SelectCommand = (NpgsqlCommand)command;
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
            NpgsqlParameter[] result = new NpgsqlParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                UNumber(parameter);
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if (parameter.Value is System.Data.SqlTypes.SqlDateTime && parameter.DbType == System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                var sqlParameter = new NpgsqlParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                if (parameter.IsJson)
                {
                    sqlParameter.NpgsqlDbType = NpgsqlDbType.Json;
                }
                if (parameter.IsArray)
                {
                    Array(parameter, sqlParameter);
                }
                if (sqlParameter.Direction == 0)
                {
                    sqlParameter.Direction = ParameterDirection.Input;
                }
                result[index] = sqlParameter;
                if (sqlParameter.Direction.IsIn(ParameterDirection.Output, ParameterDirection.InputOutput, ParameterDirection.ReturnValue))
                {
                    if (this.OutputParameters == null) this.OutputParameters = new List<IDataParameter>();
                    this.OutputParameters.RemoveAll(it => it.ParameterName == sqlParameter.ParameterName);
                    this.OutputParameters.Add(sqlParameter);
                }
                if (isVarchar && sqlParameter.DbType == System.Data.DbType.String)
                {
                    sqlParameter.DbType = System.Data.DbType.AnsiString;
                }
                else if (sqlParameter.Value is DateTime && sqlParameter.DbType == System.Data.DbType.AnsiString)
                {
                    sqlParameter.DbType = System.Data.DbType.DateTime;
                }
                ++index;
                if (parameter.CustomDbType != null&& parameter.CustomDbType is NpgsqlDbType)
                {
                    sqlParameter.NpgsqlDbType =((NpgsqlDbType)parameter.CustomDbType);
                }
            }
            return result;
        }

        //private static void ConvertUNumber(SugarParameter parameter)
        //{
        //    if (parameter.DbType == System.Data.DbType.UInt32)
        //    {
        //        parameter.DbType = System.Data.DbType.Int32;
        //    }
        //    else if (parameter.DbType == System.Data.DbType.UInt64)
        //    {
        //        parameter.DbType = System.Data.DbType.UInt64;
        //    }
        //}

        private static void Array(SugarParameter parameter, NpgsqlParameter sqlParameter)
        {
            //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
            var type = sqlParameter.Value.GetType();
            if (ArrayMapping.ContainsKey(type))
            {
                sqlParameter.NpgsqlDbType = ArrayMapping[type] | NpgsqlDbType.Array;
            }
            else if (type == DBNull.Value.GetType())
            {
                DbNullParametrerArray(parameter, sqlParameter);

            }
            else
            {
                Check.Exception(true, sqlParameter.Value.GetType().Name + " No Support");
            }
        }

        private static void DbNullParametrerArray(SugarParameter parameter, NpgsqlParameter sqlParameter)
        {
            if (parameter.DbType.IsIn(System.Data.DbType.Int32))
            {
                sqlParameter.NpgsqlDbType = NpgsqlDbType.Integer | NpgsqlDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int16))
            {
                sqlParameter.NpgsqlDbType = NpgsqlDbType.Smallint | NpgsqlDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int64))
            {
                sqlParameter.NpgsqlDbType = NpgsqlDbType.Bigint | NpgsqlDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Guid))
            {
                sqlParameter.NpgsqlDbType = NpgsqlDbType.Uuid | NpgsqlDbType.Array;
            }
            else
            {
                sqlParameter.NpgsqlDbType = NpgsqlDbType.Text | NpgsqlDbType.Array;
            }
        }

        private static void UNumber(SugarParameter parameter)
        {
            if (parameter.DbType == System.Data.DbType.UInt16)
            {
                parameter.DbType = System.Data.DbType.Int16;
                parameter.Value = Convert.ToInt16(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt32)
            {
                parameter.DbType = System.Data.DbType.Int32;
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.DbType == System.Data.DbType.UInt64)
            {
                parameter.DbType = System.Data.DbType.Int64;
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
            if (parameter.Value is uint)
            {
                parameter.Value = Convert.ToInt32(parameter.Value);
            }
            else if (parameter.Value is ulong)
            {
                parameter.Value = Convert.ToInt64(parameter.Value);
            }
        }
        public override Action<SqlSugarException> ErrorEvent => it =>
        {
            if (base.ErrorEvent != null)
            {
                base.ErrorEvent(it);
            }
            if (it.Message != null && it.Message.StartsWith("42883: function uuid_generate_v4() does not exist"))
            {
                Check.ExceptionEasy(it.Message, $"使用uuid_generate_v4()函数需要创建 CREATE EXTENSION IF NOT EXISTS pgcrypto;CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\" ");
            }
        };

        static readonly Dictionary<Type, NpgsqlDbType> ArrayMapping = new Dictionary<Type, NpgsqlDbType>()
        {
            { typeof(int[]),NpgsqlDbType.Integer},
            { typeof(short[]),NpgsqlDbType.Smallint},
            { typeof(long[]),NpgsqlDbType.Bigint},
            { typeof(decimal[]),NpgsqlDbType.Numeric},
            { typeof(char[]),NpgsqlDbType.Text},
            { typeof(byte[]),NpgsqlDbType.Bytea},
            { typeof(bool[]),NpgsqlDbType.Boolean},
            {typeof(DateTime[]),NpgsqlDbType.Date},
            {typeof(float[]),NpgsqlDbType.Real},
            {typeof(Guid[]),NpgsqlDbType.Uuid},


            { typeof(int?[]),NpgsqlDbType.Integer},
            { typeof(short?[]),NpgsqlDbType.Smallint},
            { typeof(long?[]),NpgsqlDbType.Bigint},
            { typeof(decimal?[]),NpgsqlDbType.Numeric},
            { typeof(char?[]),NpgsqlDbType.Text},
            { typeof(byte?[]),NpgsqlDbType.Bytea},
            { typeof(bool?[]),NpgsqlDbType.Boolean},
            {typeof(DateTime?[]),NpgsqlDbType.Date},
            {typeof(Guid?[]),NpgsqlDbType.Uuid},


             { typeof(string[]), NpgsqlDbType.Text},
             {typeof(float?[]),NpgsqlDbType.Real},
        };
    }
}
