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
            db.DbMaintenance.TruncateTable<Student>();
            db.Storageable(new Student() { Name = "a", SchoolId = "1", CreateDateTime = DateTime.Now })
                .ExecuteCommand();
            var datas=db.Queryable<Student>().ToList();
            if (datas.Count != 1) Cases.ThrowUnitError();
            datas.First().Name = "aaa";
            db.Storageable(datas).ExecuteCommand();
            var datas2 = db.Queryable<Student>().ToList();
            if (datas2.Count != 1) Cases.ThrowUnitError();
            if (datas2.First().Name != "aaa") Cases.ThrowUnitError();
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
