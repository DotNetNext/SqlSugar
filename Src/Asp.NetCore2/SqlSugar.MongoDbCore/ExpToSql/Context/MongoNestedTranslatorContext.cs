using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDbCore
{
    public class MongoNestedTranslatorContext
    {
        public ResolveExpressType resolveType { get; internal set; }
        public SqlSugarProvider context { get; internal set; }
    }
}
