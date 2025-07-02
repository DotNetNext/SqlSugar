using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDb.Ado.Data.Common
{
    public class MongoParsedCommand
    {
        public string CollectionName { get; set; }
        public string Operation { get; set; }
        public string Json { get; set; }
    }
}


