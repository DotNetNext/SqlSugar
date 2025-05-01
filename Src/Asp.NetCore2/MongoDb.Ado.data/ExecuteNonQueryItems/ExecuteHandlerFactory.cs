using MongoDb.Ado.data;
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
                { "find", new FindHandler() }
            };
    }
}
