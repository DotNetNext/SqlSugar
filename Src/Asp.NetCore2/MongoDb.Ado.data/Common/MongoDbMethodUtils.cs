using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class MongoDbMethodUtils
    { 
        public void ValidateOperation(string operation)
        {
            if (ExecuteHandlerFactory.Items.TryGetValue(operation, out var handler))
            {
                return ;
            }
            if (DbDataReaderFactory.Items.TryGetValue(operation, out var handlerQuery)) 
            {
                return;
            }
            throw new InvalidOperationException($"Operation '{operation}' is not supported.");
        }
    }
}
