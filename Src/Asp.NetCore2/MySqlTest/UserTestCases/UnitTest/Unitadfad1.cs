using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfad1
    {
        public static void Init()
        {
            var db =NewUnitTest.Db; 
            db.CodeFirst.InitTables(typeof(TestA));
            db.DbMaintenance.TruncateTable<TestA>();
            db.Insertable(new List<TestA>() { new TestA() { Id = 1, Count = 1.212f }, new TestA() { Id = 1, Count = 1.212f } }).ExecuteReturnSnowflakeId    ();
            var list = db.Queryable<TestA>().ToList();
            db.Updateable(list).ExecuteCommand();
            var list2 = db.Queryable<TestA>().ToList();
            if (list2.First().Count != 1.212f)
            {
                throw new Exception("error");
            }
            db.Updateable(list)
                .PublicSetColumns(it=>it.Count,"-").ExecuteCommand();
            var list3 = db.Queryable<TestA>().ToList();
            if (list3.First().Count != 0)
            {
                throw new Exception("error");
            }
        }   
    }
    [SugarTable("TestA111")]
    public class TestA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 容量或数量
        /// </summary>
        [SugarColumn(ColumnDescription = "容量或数量",Length =10,DecimalDigits =6)]
        public float Count { get; set; } = 0;
    }
}
