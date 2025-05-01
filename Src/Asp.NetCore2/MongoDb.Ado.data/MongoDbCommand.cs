using System;
using System.Data.Common;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDb.Ado.data
{
    public class MongoDbCommand : DbCommand
    {
        private string _commandText;
        private MongoDbConnection _connection;
        private int _commandTimeout;

        public MongoDbCommand() { }

        public MongoDbCommand(string commandText, MongoDbConnection connection)
        {
            _commandText = commandText;
            _connection = connection;
        }

        public override string CommandText
        {
            get => _commandText;
            set => _commandText = value;
        }

        public override int CommandTimeout
        {
            get => _commandTimeout;
            set => _commandTimeout = value;
        }

        public override CommandType CommandType { get; set; } = CommandType.Text;

        protected override DbConnection DbConnection
        {
            get => _connection;
            set => _connection = (MongoDbConnection)value;
        }

        protected override DbParameterCollection DbParameterCollection => throw new NotSupportedException("暂不支持参数。");

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        public override void Cancel() { }

        public override int ExecuteNonQuery()
        {
            var (operation, collectionName, json) = ParseCommand(_commandText);
            var collection = GetCollection(collectionName);

            if (operation == "insert")
            {
                var doc = BsonDocument.Parse(json);
                collection.InsertOne(doc);
                return 1;
            }

            throw new NotSupportedException("只支持 insert 操作。");
        }

        public override object ExecuteScalar()
        {
            var (operation, collectionName, json) = ParseCommand(_commandText);
            var collection = GetCollection(collectionName);

            if (operation == "find")
            {
                var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);
                var document = collection.Find(filter).FirstOrDefault();
                return document;
            }

            throw new NotSupportedException("只支持 find 操作。");
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var (operation, collectionName, json) = ParseCommand(_commandText);
            var collection = GetCollection(collectionName);

            if (operation == "find")
            {
                var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);
                //var filter = new BsonDocument { { "age", new BsonDocument { { "$gt", 25 } } } };
                var cursor = collection.Find(filter).ToCursor();
                return new MongoDbDataReader(cursor);
            }

            throw new NotSupportedException("只支持 find 操作。");
        }

        public override void Prepare() { }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }

        private IMongoCollection<BsonDocument> GetCollection(string name)
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
                throw new InvalidOperationException("连接尚未打开。");

            return _connection.GetDatabase().GetCollection<BsonDocument>(name);
        }

        // 示例：find users {age:{$gt:20}} 或 insert users {"name":"Tom"}
        private (string op, string collection, string json) ParseCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd))
                throw new InvalidOperationException("CommandText 不能为空。");

            var parts = cmd.Trim().Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new InvalidOperationException("命令格式错误，应为：操作 集合名 JSON过滤");

            string op = parts[0].ToLowerInvariant();
            string collection = parts[1];
            string json = parts.Length >= 3 ? parts[2] : "";

            return (op, collection, json);
        }
    }
}




