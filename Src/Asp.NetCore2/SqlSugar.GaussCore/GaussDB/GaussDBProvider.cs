using Dm;
using Npgsql;
using NpgsqlTypes;
using OpenGauss.NET;
using OpenGauss.NET.Types;
using SqlSugar.GaussDBCore;
using SqlSugar.GaussDBCore.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GaussDB
{
    public class GaussDBProvider : PostgreSQLProvider
    {
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    try
                    {
                        var npgsqlConnectionString = base.Context.CurrentConnectionConfig.ConnectionString;
                        base._DbConnection = new OpenGaussConnection(npgsqlConnectionString);
                    }
                    catch (Exception ex)
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

        //public override void BeginTran(string transactionName)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void BeginTran(IsolationLevel iso, string transactionName)
        //{
        //    throw new NotImplementedException();
        //}

        public override IDataAdapter GetAdapter()
        {
            return new GaussDBDataAdapter();
        }

        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            OpenGaussCommand sqlCommand = new OpenGaussCommand(sql, (OpenGaussConnection)this.Connection);
            sqlCommand.CommandType = this.CommandType;
            sqlCommand.CommandTimeout = this.CommandTimeOut;
            if (this.Transaction != null)
            {
                sqlCommand.Transaction = (OpenGaussTransaction)this.Transaction;
            }
            if (parameters.HasValue())
            {
                IDataParameter[] ipars = ToIDbDataParameter(parameters);
                sqlCommand.Parameters.AddRange((OpenGaussParameter[])ipars);
            }
            CheckConnection();
            return sqlCommand;
        }

        public override void SetCommandToAdapter(IDataAdapter adapter, DbCommand command)
        {
            ((GaussDBDataAdapter)adapter).SelectCommand = (OpenGaussCommand)command;
        }

        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return null;
            OpenGaussParameter[] result = new OpenGaussParameter[parameters.Length];
            int index = 0;
            var isVarchar = this.Context.IsVarchar();
            foreach (var parameter in parameters)
            {
                if (parameter.DbType == System.Data.DbType.Int64 && parameter.Value?.Equals("Result%") == true)
                {
                    parameter.DbType = System.Data.DbType.AnsiString;
                }
                UNumber(parameter);
                if (parameter.Value == null) parameter.Value = DBNull.Value;
                if (parameter.Value is System.Data.SqlTypes.SqlDateTime && parameter.DbType == System.Data.DbType.AnsiString)
                {
                    parameter.DbType = System.Data.DbType.DateTime;
                    parameter.Value = DBNull.Value;
                }
                var sqlParameter = new OpenGaussParameter();
                sqlParameter.ParameterName = parameter.ParameterName;
                sqlParameter.Size = parameter.Size;
                sqlParameter.Value = parameter.Value;
                sqlParameter.DbType = parameter.DbType;
                sqlParameter.Direction = parameter.Direction;
                if (parameter.IsJson)
                {
                    sqlParameter.OpenGaussDbType = OpenGaussDbType.Json;
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
                if (parameter.CustomDbType != null && parameter.CustomDbType is OpenGaussDbType)
                {
                    sqlParameter.OpenGaussDbType = ((OpenGaussDbType)parameter.CustomDbType);
                }
            }
            return result;
        }

        private static void Array(SugarParameter parameter, OpenGaussParameter sqlParameter)
        {
            //    sqlParameter.Value = this.Context.Utilities.SerializeObject(sqlParameter.Value);
            var type = sqlParameter.Value.GetType();
            if (ArrayMapping.ContainsKey(type))
            {
                sqlParameter.OpenGaussDbType = ArrayMapping[type] | OpenGaussDbType.Array;
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

        private static void DbNullParametrerArray(SugarParameter parameter, OpenGaussParameter sqlParameter)
        {
            if (parameter.DbType.IsIn(System.Data.DbType.Int32))
            {
                sqlParameter.OpenGaussDbType = OpenGaussDbType.Integer | OpenGaussDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int16))
            {
                sqlParameter.OpenGaussDbType = OpenGaussDbType.Smallint | OpenGaussDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Int64))
            {
                sqlParameter.OpenGaussDbType = OpenGaussDbType.Bigint | OpenGaussDbType.Array;
            }
            else if (parameter.DbType.IsIn(System.Data.DbType.Guid))
            {
                sqlParameter.OpenGaussDbType = OpenGaussDbType.Uuid | OpenGaussDbType.Array;
            }
            else
            {
                sqlParameter.OpenGaussDbType = OpenGaussDbType.Text | OpenGaussDbType.Array;
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

        static readonly Dictionary<Type, OpenGaussDbType> ArrayMapping = new Dictionary<Type, OpenGaussDbType>()
        {
            { typeof(int[]),OpenGaussDbType.Integer},
            { typeof(short[]),OpenGaussDbType.Smallint},
            { typeof(long[]),OpenGaussDbType.Bigint},
            { typeof(decimal[]),OpenGaussDbType.Numeric},
            { typeof(char[]),OpenGaussDbType.Text},
            { typeof(byte[]),OpenGaussDbType.Bytea},
            { typeof(bool[]),OpenGaussDbType.Boolean},
            {typeof(DateTime[]),OpenGaussDbType.Date},
            {typeof(float[]),OpenGaussDbType.Real},
            {typeof(Guid[]),OpenGaussDbType.Uuid},


            { typeof(int?[]),OpenGaussDbType.Integer},
            { typeof(short?[]),OpenGaussDbType.Smallint},
            { typeof(long?[]),OpenGaussDbType.Bigint},
            { typeof(decimal?[]),OpenGaussDbType.Numeric},
            { typeof(char?[]),OpenGaussDbType.Text},
            { typeof(byte?[]),OpenGaussDbType.Bytea},
            { typeof(bool?[]),OpenGaussDbType.Boolean},
            {typeof(DateTime?[]),OpenGaussDbType.Date},
            {typeof(Guid?[]),OpenGaussDbType.Uuid},


             { typeof(string[]), OpenGaussDbType.Text},
             {typeof(float?[]),OpenGaussDbType.Real},
        };
    }
}
