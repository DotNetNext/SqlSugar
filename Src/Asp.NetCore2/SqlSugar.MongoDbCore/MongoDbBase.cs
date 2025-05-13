using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb
{
    public class MongoDbBase
    {
        [SugarColumn(IsPrimaryKey =true,IsOnlyIgnoreInsert =true,ColumnName ="_id")]
        public string Id { get; set; }
    }
}
