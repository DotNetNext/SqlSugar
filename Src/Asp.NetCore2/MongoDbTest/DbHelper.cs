using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class DbHelper
    {
        public static string ConnectionString= "mongodb://mongouser:Huangxin%40123@localhost:27018/SqlSugarDb?replicaSet=cmgo-7d07e4w1_0&authSource=admin";
        public static string SqlSugarConnectionString = "host=localhost;Port=27018;Database=SqlSugarDb;Username=mongouser;Password=Huangxin@123;replicaSet=cmgo-7d07e4w1_0&authSource=admin";
    }
}
