using SqlSugar.MongoDb;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dm.util;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace MongoDbTest
{
    public class Insert3
    {
        private static SqlSugarClient db = DbHelper.GetNewDb();

        internal static void Init()
        {
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();

            var data2 = new Student() { Age = 1, Name = "11", SchoolId = "111", its = new List<Chid>() { new Chid() { Name = "xxx" } } };
            db.Insertable(data2).ExecuteCommandIdentityIntoEntityAsync().GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(data2.Id)) Cases.ThrowUnitError();

            var data3 = db.Queryable<Student>().Where(it => it.Id == data2.Id).First();

            if (data3.RootTime!.Value.ToString("yyyy-MM-dd HH") != new Student().RootTime!.Value!.ToString("yyyy-MM-dd HH")) Cases.ThrowUnitError();
            if (data3.its.First().RetailCrTime!.Value.ToString("yyyy-MM-dd HH") != new Chid().RetailCrTime!.Value.ToString("yyyy-MM-dd HH")) Cases.ThrowUnitError();

            db.Updateable(data3).ExecuteCommand();
            var data5 = db.Queryable<Student>().Where(it => it.Id == data2.Id).First();

            if (data5.RootTime!.Value.ToString("yyyy-MM-dd HH") != new Student().RootTime!.Value.ToString("yyyy-MM-dd HH")) Cases.ThrowUnitError();
            if (data5.its.First().RetailCrTime!.Value.ToString("yyyy-MM-dd HH") != new Chid().RetailCrTime!.Value.ToString("yyyy-MM-dd HH")) Cases.ThrowUnitError();
        }

        public static DbResult<bool> UseTran(Action action)
        {
            try
            {
                var result = db.Ado.UseTran(() => action());
                return result;
            }
            catch (Exception ex)
            {
                db.Ado.RollbackTran();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [SqlSugar.SugarTable("UnitStudent1ddsfhssds3z1")]
        public class Student : MongoDbBase
        {
            public string Name { get; set; }

            public string SchoolId { get; set; }

            public int Age { get; set; }

            [SqlSugar.SugarColumn(IsJson = true)]
            public List<Chid> its { get; set; } = new List<Chid>();

            public DateTime? RootTime { get; set; } = DateTime.Now;
        }

        public class Chid
        {
            public string Name { get; set; }

            public DateTime? RetailCrTime { get; set; } = DateTime.Now;
        }
    }
}
