using MongoDb.Ado.data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public  class AdoTest
    {
        public static void Init() 
        {
            //开发中
            var client = new MongoClient("");
            var database = client.GetDatabase("test");
            // 获取当前数据库中的所有集合
            var collections =   database.ListCollections();
      

            MongoDbConnection connection = new MongoDbConnection("");
            connection.Open();
            connection.Close();
        }
    }
}
