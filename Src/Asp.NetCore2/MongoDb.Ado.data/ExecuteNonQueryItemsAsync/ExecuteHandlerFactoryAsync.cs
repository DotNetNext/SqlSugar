using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class ExecuteHandlerFactoryAsync
    {
        public readonly static Dictionary<string, IMongoOperationHandlerAsync> Items = new Dictionary<string, IMongoOperationHandlerAsync>(StringComparer.OrdinalIgnoreCase)
            {
                { "insert", new InsertHandlerAsync() },
                { "insertmany", new InsertManyHandlerAsync() },
                { "update", new UpdateHandlerAsync() },
                { "updatemany", new UpdateManyHandlerAsync() },
                { "delete", new DeleteHandlerAsync() },
                { "deletemany", new DeleteManyHandlerAsync() },
                { "find", new NonFindHandlerAsync() }
            };


        public static Task<int> HandlerAsync(string operation, string json, IMongoCollection<BsonDocument> collection)
        {
            MongoDbMethodUtils.ValidateOperation(operation);
            var handlers = ExecuteHandlerFactoryAsync.Items;

            if (!handlers.TryGetValue(operation, out var handler))
                throw new NotSupportedException($"不支持的操作类型: {operation}");
            handler.operation = operation;
            return handler.HandleAsync(collection, json);
        }

    }
}
