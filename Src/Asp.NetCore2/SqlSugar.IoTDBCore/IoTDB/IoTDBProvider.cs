using Apache.IoTDB.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SqlSugar.IoTDBCore;

namespace SqlSugar.IoTDB
{
    public class IoTDBProvider : AdoProvider
    {
        public override IDbConnection Connection
        {
            get
            {
                if (base._DbConnection == null)
                {
                    var options = IoTDBConnectionOptions.Parse(Context.CurrentConnectionConfig.ConnectionString);
                    base._DbConnection = new IoTDBConnection(options.ProviderConnectionString);
                }
                return base._DbConnection;
            }
            set { base._DbConnection = value; }
        }

        public override IDataAdapter GetAdapter() => new IoTDBDataAdapter();

        public override DbCommand GetCommand(string sql, SugarParameter[] parameters)
        {
            if (this.CommandType != CommandType.Text)
                throw new NotSupportedException("IoTDB supports text SQL commands only.");
            if (parameters != null && parameters.Any(it =>
                it.Direction == ParameterDirection.Output ||
                it.Direction == ParameterDirection.InputOutput ||
                it.Direction == ParameterDirection.ReturnValue))
                throw new NotSupportedException("IoTDB does not support output parameters.");
            var values = parameters?.Select(it => new KeyValuePair<string, object>(it.ParameterName, it.Value));
            var command = new IoTDBDbCommand(IoTDBParameterBinder.Bind(sql, values), (IoTDBConnection)Connection)
            {
                CommandType = this.CommandType,
                CommandTimeout = this.CommandTimeOut
            };
            CheckConnection();
            return command;
        }

        public override void SetCommandToAdapter(IDataAdapter adapter, DbCommand command)
        {
            ((IoTDBDataAdapter)adapter).SelectCommand = command;
        }

        public override IDataParameter[] ToIDbDataParameter(params SugarParameter[] parameters)
        {
            return Array.Empty<IDataParameter>();
        }

        public override void BeginTran(string transactionName) => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");
        public override void BeginTran(IsolationLevel iso, string transactionName) => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");
        public override void BeginTran() => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");
        public override void BeginTran(IsolationLevel iso) => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");
        public override System.Threading.Tasks.Task BeginTranAsync() => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");
        public override System.Threading.Tasks.Task BeginTranAsync(IsolationLevel iso) => throw new NotSupportedException("IoTDB tree model does not provide atomic ADO.NET transactions.");

        internal IoTDBConnectionOptions Options => IoTDBConnectionOptions.Parse(Context.CurrentConnectionConfig.ConnectionString);
    }
}
