using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F9.DataEntity.Entity;
using SqlSugar;
namespace  OrmTest
{
    public class UBulkCopy
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.CreateDatabase();
            db.Aop.OnLogExecuting = null;
            db.CodeFirst.InitTables<UBulkCopydsafad1>();
            var list = new List<UBulkCopydsafad1>();
           db.DbMaintenance.TruncateTable<UBulkCopydsafad1>();
            for (int i = 0; i < 100000; i++)
            {
                list.Add(new UBulkCopydsafad1() { name = "a", name1 = "x" + i, name2 = "a" });
            }
            //  db.BeginTran();
            // db.Insertable(list).ExecuteCommand();
            var list2 = db.Queryable<UBulkCopydsafad1>().ToList();
            db.Fastest<UBulkCopydsafad1>().BulkCopy(list);
            var list3 = db.Queryable<UBulkCopydsafad1>().ToList();
            var row= db.Fastest<UBulkCopydsafad1>().BulkUpdate(list3);
            //db.CommitTran();
        }
        public class UBulkCopydsafad1
        {
            [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
            public int id { get; set; }
            public string  name { get; set; }
            public string name1 { get; set; }
            public string name2 { get; set; }
            public int id2 { get; set; }
            public int id3 { get; set; }
        }
    }
}
