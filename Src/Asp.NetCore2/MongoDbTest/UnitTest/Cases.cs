using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MongoDbTest
{
    public class Cases
    {
        public static void Init() 
        {
            QuerySingle.Init();
            QueryWhere.Init();
            QuerySelect.Init();
            QuerySelect2.Init();
            QueryJson.Init();
            QueryLeftJoin.Init();
            QueryLeftJoin2.Init();
            QueryJsonArray.Init();
            Insert.Init();
            Insert2.Init();
            Update.Init();
            Delete.Init();
            InsertOrUpdate.Init();
            Unitdafasdys.Init();
            LongPrimaryKey.Init();
        } 
        public static void ThrowUnitError()
        {
            throw new Exception("unit error");
        }
    }
}
