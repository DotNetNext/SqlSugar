
using System;
using System.Data;
using System.Data.Common;
using System.Net.Http.Headers;
using TDengine.Driver;
using TDengine.Driver.Client;
namespace SqlSugar.TDengineAdo
{
    public class TDengineConnection : DbConnection
    {
        private readonly string host;
        private readonly short port;
        private readonly string username;
        private readonly string password;
        private string dbname;
        internal ITDengineClient connection;

        public TDengineConnection(string connectionString)
        { 
            connection = DbDriver.Open(new ConnectionStringBuilder(connectionString));
            var builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (builder.TryGetValue("Host", out object hostValue))
            {
                host = Convert.ToString(hostValue);
            }
            if (builder.TryGetValue("Port", out object portValue))
            {
                port = Convert.ToInt16(portValue);
            }
            if (builder.TryGetValue("Username", out object usernameValue))
            {
                username = Convert.ToString(usernameValue);
            }
            if (builder.TryGetValue("Password", out object passwordValue))
            {
                password = Convert.ToString(passwordValue);
            }
            if (builder.TryGetValue("Database", out object dbnameValue))
            {
                dbname = Convert.ToString(dbnameValue);
            }
        }
        public TDengineConnection(string host, short port, string username, string password, string dbname)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
            this.dbname = dbname;
            connection = DbDriver.Open(new ConnectionStringBuilder(ConnectionString));
        }

        public override string ConnectionString
        {
            get { return $"Host={host};Port={port};Username={username};Password={password};Database={dbname}"; }
            set { throw new NotSupportedException(); }
        }

        public override string Database => dbname;

        public override string DataSource => host;

        public override string ServerVersion => "Unknown"; // You may provide the actual version if available.

        public override ConnectionState State => connection==null? ConnectionState.Closed : ConnectionState.Open;

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException("BeginDbTransaction");
        }

        public override void Close()
        {
            if (connection != null)
            {

                connection.Dispose();
                connection = null;
            }
        } 
        public override void Open()
        {
            if (connection == null)
            {
                connection = DbDriver.Open(new ConnectionStringBuilder(ConnectionString)); 
            }
            else if(connection == null)
            {
                connection = DbDriver.Open(new ConnectionStringBuilder(ConnectionString));
            }
            if (!string.IsNullOrEmpty(Database)) 
            {
                connection.Exec("use " + Database); 
            }
        }

        protected override DbCommand CreateDbCommand()
        {
            return new TDengineCommand();
        }
        protected DbCommand CreateDbCommand(string commandText)
        {
            return new TDengineCommand(commandText, this);
        }

        protected override void Dispose(bool disposing)
        {
            Close();
        }
 

        public override void ChangeDatabase(string databaseName)
        { 
            dbname = databaseName; 
        }
    }
}