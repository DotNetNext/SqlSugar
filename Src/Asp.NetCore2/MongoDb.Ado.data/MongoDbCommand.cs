using System;
using System.Data.Common;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

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
                // 处理插入操作
                var doc = BsonDocument.Parse(json);
                collection.InsertOne(doc);
                return 1;  // 返回插入成功的文档数
            }
            if (operation == "insertmany")
            {
                // 处理插入多条记录操作
                var documents = BsonSerializer.Deserialize<List<BsonDocument>>(json); // 假设 json 是包含多个文档的数组
                collection.InsertMany(documents);
                return documents.Count;  // 返回插入成功的文档数
            }
            if (operation == "update")
            {
                // 处理更新操作
                var updateCommand = BsonDocument.Parse(json);
                var filter = updateCommand["filter"].AsBsonDocument;
                var update = updateCommand["update"].AsBsonDocument;
                var options = updateCommand.Contains("options") ? updateCommand["options"].AsBsonDocument : null;

                var updateResult = collection.UpdateOne(filter, update); // 单个更新
                return (int)updateResult.ModifiedCount;  // 返回修改的文档数
            }

            if (operation == "updatemany")
            {
                var totals = 0;
                // 处理插入多条记录操作
                var documents = BsonSerializer.Deserialize<List<BsonDocument>>(json); // 假设 json 是包含多个文档的数组
                foreach (var updateCommand in documents)
                {
                    var filter = updateCommand["filter"].AsBsonDocument;
                    var update = updateCommand["update"].AsBsonDocument;
                    var options = updateCommand.Contains("options") ? updateCommand["options"].AsBsonDocument : null;

                    var updateResult = collection.UpdateMany(filter, update); // 单个更新
                    totals+=(int)updateResult.ModifiedCount;  // 返回修改的文档数
                }
                return documents.Count;  // 返回插入成功的文档数
            }

            if (operation == "delete")
            {
                // 处理删除单个文档操作
                var deleteCommand = BsonDocument.Parse(json);
                var filter = deleteCommand["filter"].AsBsonDocument;

                var deleteResult = collection.DeleteOne(filter); // 单个删除
                return (int)deleteResult.DeletedCount;  // 返回删除的文档数
            }

            if (operation == "deletemany")
            {
                var totals = 0;
                // 处理插入多条记录操作
                var documents = BsonSerializer.Deserialize<List<BsonDocument>>(json); // 假设 json 
                foreach (var updateCommand in documents)
                {
                    var filter = updateCommand["filter"].AsBsonDocument;  
                    var updateResult = collection.DeleteMany(filter); // 单个更新
                    totals += (int)updateResult.DeletedCount;  // 返回修改的文档数
                }
                return documents.Count;  // 返回插入成功的文档数
            }

            if (operation == "find")
            {
                // 处理查询操作，查询并返回 0
                var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);
                var document = collection.Find(filter).FirstOrDefault(); // 查询操作
                return 0;  // 返回 0，表示查询操作已执行，且没有对数据库做更改
            }

            throw new NotSupportedException("不支持此操作类型。");
        }

        public override object ExecuteScalar()
        {
            var (operation, collectionName, json) = ParseCommand(_commandText);
            var collection = GetCollection(collectionName);

            if (operation == "find")
            {
                var filter = string.IsNullOrWhiteSpace(json) ? FilterDefinition<BsonDocument>.Empty : BsonDocument.Parse(json);

                // 设置投影排除 "_id" 字段，确保返回的结果不包含 _id 字段
                var projection = Builders<BsonDocument>.Projection.Exclude("_id");

                // 执行查询并限制返回一条记录
                var document = collection.Find(filter).Project(projection).FirstOrDefault();

                // 如果查询到结果且文档非空，则获取第一个字段的值
                if (document != null && document.Elements.Any())
                {
                    var firstElement = document.Elements.First();  // 获取第一个字段（列）
                    return firstElement.Value;  // 返回该字段的值
                }

                return null; // 如果没有结果或没有字段，返回 null
            }

           return null;
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




