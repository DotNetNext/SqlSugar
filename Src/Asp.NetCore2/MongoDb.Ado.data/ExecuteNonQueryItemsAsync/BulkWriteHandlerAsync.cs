using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MongoDb.Ado.data 
{
    public class BulkWriteHandlerAsync : IMongoOperationHandlerAsync
    {
        public HandlerContext context { get; set; }
        public CancellationToken token { get; set; }
        public string operation { get; set; }
        public async Task<int> HandleAsync(IMongoCollection<BsonDocument> collection, string json)
        {
            var documents = ParseJsonArray(json);
            var bulkOps = new List<WriteModel<BsonDocument>>();
            // 新增逻辑：如果只有一个文档且update只包含$set
            if (documents.Count == 1)
            {
                var doc = documents[0];
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;
                if (IsHandlePipelineUpdate(update))
                {
                    return await HandlePipelineUpdate(collection, filter, update);
                }
            }
            foreach (var doc in documents)
            {
                var filter = doc["filter"].AsBsonDocument;
                var update = doc["update"].AsBsonDocument;

                var op = new UpdateManyModel<BsonDocument>(filter, update);
                bulkOps.Add(op);
            }
            if (bulkOps.Count == 0) return 0;
            if (context.IsAnyServerSession)
            {
                var result = await collection.BulkWriteAsync(context.ServerSession,bulkOps);
                return (int)result.ModifiedCount;
            }
            else
            {
                var result = await collection.BulkWriteAsync(bulkOps);
                return (int)result.ModifiedCount;
            }
        }

        private List<BsonDocument> ParseJsonArray(string json)
        {
            if (json.TrimStart().StartsWith("["))
                return BsonSerializer.Deserialize<List<BsonDocument>>(json);
            return new List<BsonDocument> { BsonDocument.Parse(json) };
        }

        private async Task<int> HandlePipelineUpdate(IMongoCollection<BsonDocument> collection, BsonDocument filter, BsonDocument update)
        {
            // 构造pipeline update
            // 构造pipeline update，不写死，循环现有的$set值
            var setDoc = update["$set"].AsBsonDocument;
            var setPipelineDoc = new BsonDocument();
            foreach (var element in setDoc.Elements)
            {
                // 检查值是否为BsonDocument且包含操作符（如$add），否则直接赋值
                if (element.Value.IsBsonDocument && element.Value.AsBsonDocument.GetElement(0).Name.StartsWith("$"))
                {
                    setPipelineDoc[element.Name] = element.Value;
                }
                else
                {
                    setPipelineDoc[element.Name] = element.Value;
                }
            }
            var updatePipeline = new[]
            {
                        new BsonDocument("$set", setPipelineDoc)
                    };
            var pipelineUpdate = new PipelineUpdateDefinition<BsonDocument>(updatePipeline);

            if (context.IsAnyServerSession)
            {
                var result = await collection.UpdateManyAsync(context.ServerSession,filter, pipelineUpdate);
                return (int)result.ModifiedCount;
            }
            else
            {
                var result = await collection.UpdateManyAsync(filter, pipelineUpdate);
                return (int)result.ModifiedCount;
            }
        }

        private static bool IsHandlePipelineUpdate(BsonDocument update)
        {
            return update.ElementCount == 1 && update.Contains("$set");
        }
    }

}
