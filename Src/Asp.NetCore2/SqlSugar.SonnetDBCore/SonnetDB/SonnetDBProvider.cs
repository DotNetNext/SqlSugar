using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using SonnetDB.Data;

namespace SqlSugar.SonnetDB
{
    public sealed class SonnetDBProvider : AdoProvider
    {
        private bool captureReaderOutputs;
        private (SugarParameter Target, IDataParameter Source)[] readerOutputs = [];

        public override IDataReader GetDataReader(string sql, params SugarParameter[] parameters)
        {
            bool procedure = CommandType == CommandType.StoredProcedure;
            if (!procedure) return base.GetDataReader(sql, parameters);
            captureReaderOutputs = true;
            readerOutputs = [];
            try
            {
                var reader = (DbDataReader)base.GetDataReader(sql, parameters);
                return new SonnetDBOutputDataReader(reader, readerOutputs);
            }
            finally
            {
                captureReaderOutputs = false;
                readerOutputs = [];
            }
        }

        public override async Task<IDataReader> GetDataReaderAsync(string sql, params SugarParameter[] parameters)
        {
            bool procedure = CommandType == CommandType.StoredProcedure;
            if (!procedure) return await base.GetDataReaderAsync(sql, parameters).ConfigureAwait(false);
            captureReaderOutputs = true;
            readerOutputs = [];
            try
            {
                var reader = (DbDataReader)await base.GetDataReaderAsync(sql, parameters).ConfigureAwait(false);
                return new SonnetDBOutputDataReader(reader, readerOutputs);
            }
            finally
            {
                captureReaderOutputs = false;
                readerOutputs = [];
            }
        }
        public override IDbConnection Connection
        {
            get => _DbConnection ??= new SndbConnection(Context.CurrentConnectionConfig.ConnectionString);
            set => _DbConnection = value;
        }

        public override void BeginTran(string transactionName) => base.BeginTran();

        public override void BeginTran(IsolationLevel iso, string transactionName) => base.BeginTran(iso);

        public override IDataAdapter GetAdapter() => new SonnetDBDataAdapter();

        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            var command = new SndbCommand(sql, (SndbConnection)Connection)
            {
                CommandType = CommandType,
                CommandTimeout = CommandTimeOut,
                Transaction = (SndbTransaction?)Transaction
            };
            if (parameters != null && parameters.Length > 0)
            {
                var native = (SndbParameter[])ToIDbDataParameter(parameters);
                command.Parameters.AddRange(native);
                if (captureReaderOutputs)
                    readerOutputs = parameters.Select((parameter, index) => (Target: parameter, Source: (IDataParameter)native[index]))
                        .Where(pair => pair.Source.Direction != ParameterDirection.Input).ToArray();
            }
            CheckConnection();
            return command;
        }

        public override void SetCommandToAdapter(IDataAdapter dataAdapter, DbCommand command)
        {
            ((SonnetDBDataAdapter)dataAdapter).SelectCommand = (SndbCommand)command;
        }

        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                return null!;
            }

            var result = new SndbParameter[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var parameter = parameters[index];
                var value = parameter.Value ?? DBNull.Value;
                var dbType = parameter.DbType;
                if (dbType == System.Data.DbType.Guid)
                {
                    dbType = System.Data.DbType.String;
                    if (value != DBNull.Value) value = value.ToString()!;
                }
                if (value is DateOnly dateOnly) value = dateOnly.ToDateTime(TimeOnly.MinValue);

                var native = new SndbParameter(parameter.ParameterName, value)
                {
                    DbType = dbType,
                    Direction = parameter.Direction == 0 ? ParameterDirection.Input : parameter.Direction,
                    Size = parameter.Size
                };
                result[index] = native;
                if (native.Direction != ParameterDirection.Input)
                {
                    OutputParameters ??= new List<IDataParameter>();
                    OutputParameters.RemoveAll(it => it.ParameterName == native.ParameterName);
                    OutputParameters.Add(native);
                }
            }
            return result;
        }
    }
}
