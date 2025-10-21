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
            QueryJson2.Init();
            QueryJson3.Init();
            QueryJson4.Init();
            QueryJson5.Init();
            QueryJson6.Init();
            QueryJson7.Init();
            QueryJson8.Init();
            UnitJsonAndNew.Init();
            QueryLeftJoin.Init();
            QueryLeftJoin2.Init();
            QueryLeftJoin3.Init();
            QueryJsonArray.Init();
            QueryJsonArray2.Init();
            QueryJsonArray3.Init();
            Insert.Init();
            Insert2.Init();
            Insert3.Init();
            Update.Init();
            Delete.Init();
            InsertOrUpdate.Init();
            Unitdafasdys.Init();
            Enum.Init();
            Enum2.Init();
            UnitDateTimeFunc.Init();
            UnitDateOnly.Init();
            //主键不是ObjectId类型用例 
            //The primary key is not an ObjectId type use case
            LongPrimaryKey.Init();
            GuidPrimaryKey.Init();
            StringPrimaryKey.Init();
        } 
        public static void ThrowUnitError()
        {
            throw new Exception("unit error");
        }
    }
}
