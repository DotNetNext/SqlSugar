using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDb.Ado.data
{
    public class MongoDbConnection : DbConnection
    {
        private readonly string _originalConnectionString;
        private MongoClient _client;
        private IMongoDatabase _database;
        private ConnectionState _state = ConnectionState.Closed;
        private string _databaseName;

        public override string ConnectionString
        {
            get => _originalConnectionString;
            set => throw new NotSupportedException("Setting ConnectionString after construction is not supported.");
        }

        public override string Database => _databaseName;

        public override string DataSource => _client?.Settings?.Server?.ToString() ?? "";

        public override string ServerVersion => "MongoDB_" + (_client?.Cluster?.Description?.ClusterId.ToString() ?? "Unknown");

        public override ConnectionState State => _state;

        public MongoDbConnection(string connectionString)
        {
            _originalConnectionString = connectionString;
            ParseAndConnect(connectionString);
        }

        private void ParseAndConnect(string connStr)
        {
            if (connStr.TrimStart().StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase))
            {
                var mongoUrl = new MongoUrl(connStr);
                _client = new MongoClient(mongoUrl);
                _databaseName = mongoUrl.DatabaseName ?? "test";
                _database = _client.GetDatabase(_databaseName);
                return;
            }

            var dict = ParsePgStyleConnectionString(connStr);
            var host = dict.GetValueOrDefault("Host", "localhost");
            var port = dict.GetValueOrDefault("Port", "27017");
            _databaseName = dict.GetValueOrDefault("Database", "test");
            var username = dict.GetValueOrDefault("Username", "");
            var password = dict.GetValueOrDefault("Password", "");

            var mongoConnStr = string.IsNullOrEmpty(username)
                ? $"mongodb://{host}:{port}"
                : $"mongodb://{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}@{host}:{port}/{_databaseName}";

            var mongoUrlParsed = new MongoUrl(mongoConnStr);
            _client = new MongoClient(mongoUrlParsed);
            _database = _client.GetDatabase(_databaseName);
        }

        private Dictionary<string, string> ParsePgStyleConnectionString(string connStr)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connStr };
            return builder.Cast<KeyValuePair<string, object>>()
                          .ToDictionary(kv => kv.Key, kv => kv.Value.ToString(), StringComparer.OrdinalIgnoreCase);
        }

        public override void Open()
        {
            // MongoClient 实际上在操作集合时才连接，我们只改变状态
            _state = ConnectionState.Open;
        }

        public override void Close()
        {
            _state = ConnectionState.Closed;
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotSupportedException("MongoDB does not support SQL-style DbCommand.");
        }

        public override void ChangeDatabase(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentException("Database name cannot be null or empty.", nameof(databaseName));

            _database = _client.GetDatabase(databaseName);
            _databaseName = databaseName;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotSupportedException("MongoDB does not support transactions via DbTransaction.");
        }

        public IMongoDatabase GetDatabase() => _database;

        public MongoClient GetClient() => _client;

        public override string ToString() => _originalConnectionString;
    }
}
