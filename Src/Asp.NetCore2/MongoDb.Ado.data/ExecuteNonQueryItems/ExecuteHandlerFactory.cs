using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.data
{
    public class ExecuteHandlerFactory
    {
        public readonly static Dictionary<string, IMongoOperationHandler> Items = new Dictionary<string, IMongoOperationHandler>(StringComparer.OrdinalIgnoreCase)
            {
                { "insert", new InsertHandler() },
                { "insertmany", new InsertManyHandler() },
                { "update", new UpdateHandler() },
                { "updatemany", new UpdateManyHandler() },
                { "delete", new DeleteHandler() },
                { "deletemany", new DeleteManyHandler() },
                { "find", new NonFindHandler() }
            };


        public static int Handler(string operation, string json, IMongoCollection<BsonDocument> collection)
        {
            MongoDbMethodUtils.ValidateOperation(operation);
            var handlers = ExecuteHandlerFactory.Items;

            if (!handlers.TryGetValue(operation, out var handler))
                throw new NotSupportedException($"不支持的操作类型: {operation}");
            handler.operation = operation;
            return handler.Handle(collection, json);
        }

    }
}
