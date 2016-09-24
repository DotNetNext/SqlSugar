using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;

namespace NewTest.Demos
{
    //设置ToJson的日期格式
    public class SerializerDateFormat : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动SerializerDateFormat.Init");
            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                db.SerializerDateFormat = "yyyy-mm/dd";
                var jsonStr = db.Queryable<InsertTest>().OrderBy("id").Take(1).ToJson();
                var jsonStr2 = db.Sqlable().From<InsertTest>("t").SelectToJson(" top 1 *");
                var jsonStr3 = db.SqlQueryJson("select top 1 * from InsertTest");
            }
        }
    }
}
