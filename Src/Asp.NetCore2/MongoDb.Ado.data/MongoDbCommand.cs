using System;
using System.Data.Common;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data; 
namespace MongoDb.Ado.data
{

    public class MongoDbCommand : DbCommand
    {
        private string _commandText;
        private MongoDbConnection _connection;
        private int _commandTimeout;

        public MongoDbCommand()
        {
        }

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

        protected override DbParameterCollection DbParameterCollection => throw new NotSupportedException("MongoDbCommand does not support parameters yet.");

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        public override void Cancel()
        {
            // MongoDB driver does not support canceling a command.
        }

        public override int ExecuteNonQuery()
        {
            throw new NotSupportedException("MongoDbCommand does not support ExecuteNonQuery directly.");
        }

        public override object ExecuteScalar()
        {
            var collection = GetCollection();
            var document = collection.Find(FilterDefinition<BsonDocument>.Empty).FirstOrDefault();
            return document;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var collection = GetCollection();
            var cursor = collection.Find(FilterDefinition<BsonDocument>.Empty).ToCursor();
            return new MongoDbDataReader(cursor);
        }

        public override void Prepare()
        {
            // No preparation needed for MongoDB commands
        }

        private IMongoCollection<BsonDocument> GetCollection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be open.");

            if (string.IsNullOrWhiteSpace(_commandText))
                throw new InvalidOperationException("CommandText must be set to the collection name.");

            return _connection.GetDatabase().GetCollection<BsonDocument>(_commandText);
        }

        protected override DbParameter CreateDbParameter()
        {
            throw new NotImplementedException();
        }
    }

}
