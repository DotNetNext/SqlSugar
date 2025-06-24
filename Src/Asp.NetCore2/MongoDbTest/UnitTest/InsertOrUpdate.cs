using MongoDb.Ado.data;
using MongoDB.Driver;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest 
{
    internal class InsertOrUpdate
    {
        internal static void Init()
        {
            var db = DBHelper.DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>(); 
            db.Storageable(new Student() { Name = "a", SchoolId = "1", CreateDateTime = DateTime.Now })
                .ExecuteCommand();
        }
        [SqlSugar.SugarTable("UnitStudent1zzsds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            public DateTime CreateDateTime { get; set; }
        }
    }
}
