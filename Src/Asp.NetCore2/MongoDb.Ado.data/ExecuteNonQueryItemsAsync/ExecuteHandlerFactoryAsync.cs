using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class ExecuteHandlerFactoryAsync
    {
        public readonly Dictionary<string, IMongoOperationHandlerAsync> Items = new Dictionary<string, IMongoOperationHandlerAsync>(StringComparer.OrdinalIgnoreCase)
            {
                { "insert", new InsertHandlerAsync() },
                { "insertmany", new InsertManyHandlerAsync() },
                { "update", new UpdateHandlerAsync() },
                { "updatemany", new UpdateManyHandlerAsync() },
                { "BulkWrite", new BulkWriteHandlerAsync() },
                { "delete", new DeleteHandlerAsync() },
                { "deletemany", new DeleteManyHandlerAsync() },
                { "find", new NonFindHandlerAsync() }
            };


        public static Task<int> HandlerAsync(string operation, string json, IMongoCollection<BsonDocument> collection,CancellationToken cancellationToken, HandlerContext handlerContext)
        {
            MongoDbMethodUtils.ValidateOperation(operation);
            var handlers = new ExecuteHandlerFactoryAsync().Items;

            if (!handlers.TryGetValue(operation, out var handler))
                throw new NotSupportedException($"不支持的操作类型: {operation}");
            handler.operation = operation;
            handler.token = cancellationToken;
            handler.context = handlerContext;
            return handler.HandleAsync(collection, json);
        }

    }
}
