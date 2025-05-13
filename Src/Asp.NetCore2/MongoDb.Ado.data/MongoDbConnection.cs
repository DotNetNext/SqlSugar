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
        private static readonly Dictionary<string, MongoClient> _clientCache = new Dictionary<string, MongoClient>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _lock = new object();

        private string _originalConnectionString;
        private IMongoDatabase _database;
        private string _databaseName;
        private ConnectionState _state = ConnectionState.Closed;

        public override string Database => _databaseName;
        public override string DataSource => _client?.Settings?.Server?.ToString() ?? "";
        public override string ServerVersion => "MongoDB_" + (_client?.Cluster?.Description?.ClusterId.ToString() ?? "Unknown");
        public override ConnectionState State => _state;

        public override string ConnectionString { get => _originalConnectionString; set => _originalConnectionString = value; }
        public string[] ObjectIds { get; internal set; }

        private MongoClient _client;

        public MongoDbConnection(string connectionString)
        {
            _originalConnectionString = connectionString;
            ParseAndConnect(connectionString);
        }

        private void ParseAndConnect(string connStr)
        {
            string mongoConnStr;
            if (connStr.TrimStart().StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase))
            {
                mongoConnStr = connStr;
            }
            else
            {
                string queryParams = string.Empty;  // 用来存储查询参数部分

                // 如果连接字符串以 "mongodb://" 开头
                if (connStr.TrimStart().StartsWith("mongodb://", StringComparison.OrdinalIgnoreCase))
                {
                    mongoConnStr = connStr;

                    // 提取查询参数
                    var uri = new Uri(mongoConnStr);
                    var query = uri.Query;
                    if (!string.IsNullOrEmpty(query))
                    {
                        queryParams = query;
                    }
                }
                else
                {
                    // 解析以 PostgreSQL 风格的连接字符串
                    var dict = ParsePgStyleConnectionString(connStr);
                    var host = dict.GetValueOrDefault("Host", "localhost");
                    var port = dict.GetValueOrDefault("Port", "27017");
                    _databaseName = dict.GetValueOrDefault("Database", "");
                    var username = dict.GetValueOrDefault("Username", "");
                    var password = dict.GetValueOrDefault("Password", "");

                    mongoConnStr = string.IsNullOrEmpty(username)
                        ? $"mongodb://{host}:{port}/{_databaseName}"
                        : $"mongodb://{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}@{host}:{port}/{_databaseName}";

                    // 提取查询参数（如果有）
                    if (dict.ContainsKey("ReplicaSet"))
                    {
                        queryParams += $"?replicaSet={dict["ReplicaSet"]}";
                    }

                    if (dict.ContainsKey("AuthSource"))
                    {
                        if (!string.IsNullOrEmpty(queryParams))
                        {
                            queryParams += "&";
                        }
                        queryParams += $"authSource={dict["AuthSource"]}";
                    }
                }
                mongoConnStr = mongoConnStr + queryParams;
                _client = GetOrCreateClient(mongoConnStr);

                if (_databaseName == null)
                {
                    var mongoUrl = new MongoUrl(mongoConnStr);
                    _databaseName = mongoUrl.DatabaseName ?? "test";
                }

                _database = _client.GetDatabase(_databaseName);
            }
        }

        private static MongoClient GetOrCreateClient(string connectionString)
        {
            if (_clientCache.TryGetValue(connectionString, out var client))
                return client;

            lock (_lock)
            {
                if (_clientCache.TryGetValue(connectionString, out client))
                    return client;

                client = new MongoClient(connectionString);
                _clientCache[connectionString] = client;
                return client;
            }
        }

        private Dictionary<string, string> ParsePgStyleConnectionString(string connStr)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connStr };
            return builder.Cast<KeyValuePair<string, object>>()
                          .ToDictionary(kv => kv.Key, kv => kv.Value.ToString(), StringComparer.OrdinalIgnoreCase);
        }

        public override void Open()
        {
            _state = ConnectionState.Open;
        }

        public override void Close()
        {
            _state = ConnectionState.Closed;
            // 注意：MongoClient 不需要 Dispose，它内部自己管理连接池！
            // 所以这里不用处理 _client.Dispose()，否则会出大问题
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
            return null;
        }

        public IMongoDatabase GetDatabase() => _database;
        public MongoClient GetClient() => _client;

        public override string ToString() => _originalConnectionString;
    }

}
