using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MongoDb.Ado.data
{
    public class HandlerContext
    {
        public string[] ids { get; set; }
        public DbConnection Connection { get;  set; }
        public MongoDbConnection MongoDbConnection { get { return Connection as MongoDbConnection; } }
        public IClientSessionHandle ServerSession { get { return this.MongoDbConnection?.iClientSessionHandle; } }
        public bool IsAnyServerSession { get { return ServerSession != null; } }
    }
}
