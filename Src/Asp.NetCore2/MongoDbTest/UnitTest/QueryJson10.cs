using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    internal class QueryJson10
    {

        public static void Init() 
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<RetailInfo1>();
            var id = ObjectId.GenerateNewId() + "";
            db.Insertable(new RetailInfo1()
            {
                SetBeginDate=DateTime.Now+"",
                SetEndDate=DateTime.Now+"",
                RetailDate = new RetailDateInfo() {  Id=id }

            }).ExecuteCommand();
            var list=db.Queryable<RetailInfo1>().ToList();
            var list2 = db.Queryable<RetailInfo1>().Where(i=>i.RetailDate.Id==id).ToList();
        } 
        public class RetailInfo1
        {
            /// <summary>
            /// 开始日期（字符串或时间戳，需要能转 DateTime）
            /// </summary>
            public string SetBeginDate { get; set; }

            /// <summary>
            /// 结束日期
            /// </summary>
            public string SetEndDate { get; set; }

            /// <summary>
            /// 零售日期，可能是个组合字段
            /// </summary>
            [SqlSugar.SugarColumn(IsJson =true)]
            public RetailDateInfo RetailDate { get; set; }
        } 
        public class RetailDateInfo
        { 
            public string Id { get; set; } 
        }
    }
}
