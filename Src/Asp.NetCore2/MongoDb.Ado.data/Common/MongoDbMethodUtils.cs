using MongoDb.Ado.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Ado.data
{
    public class MongoDbMethodUtils
    {
        public static MongoParsedCommand ParseMongoCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("command is null or empty");

            // 去掉前后空格与结尾分号
            command = command.Trim().TrimEnd(';');

            // 确保以 db. 开头
            if (!command.StartsWith("db."))
                throw new FormatException("Invalid command format. Must start with db.");

            int dotIndex = command.IndexOf('.', 3); // 找 collection 之后的点
            int parenIndex = command.IndexOf('(', dotIndex);
            int braceIndex = command.IndexOf('{', parenIndex);

            if (dotIndex == -1 || parenIndex == -1 || braceIndex == -1)
                throw new FormatException("Command format not recognized.");

            string collectionNameRaw = command.Substring(3, dotIndex - 3).Trim();
            string collectionName = collectionNameRaw;

            // 处理 getCollection("xxx") 格式
            if (collectionNameRaw.StartsWith("getCollection(", StringComparison.OrdinalIgnoreCase))
            {
                int start = collectionNameRaw.IndexOf('(');
                int end = collectionNameRaw.LastIndexOf(')');
                if (start != -1 && end != -1 && end > start)
                {
                    string inner = collectionNameRaw.Substring(start + 1, end - start - 1).Trim();
                    // 去掉引号
                    if ((inner.StartsWith("\"") && inner.EndsWith("\"")) || (inner.StartsWith("'") && inner.EndsWith("'")))
                    {
                        inner = inner.Substring(1, inner.Length - 2);
                    }
                    collectionName = inner;
                }
            }

            string operation = command.Substring(dotIndex + 1, parenIndex - dotIndex - 1).Trim();

            string jsonPart = command.Substring(parenIndex + 1).Trim();
            if (jsonPart.EndsWith(")"))
                jsonPart = jsonPart.Substring(0, jsonPart.Length - 1);

            return new MongoParsedCommand
            {
                CollectionName = collectionName,
                Operation = operation,
                Json = jsonPart
            };
        }

        public static void ValidateOperation(string operation)
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
