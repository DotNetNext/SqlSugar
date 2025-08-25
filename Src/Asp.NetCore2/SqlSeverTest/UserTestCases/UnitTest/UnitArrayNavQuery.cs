using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitArrayNavQuery
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitPerson01xq123gh1, UnitAddress1231sdfal>();
            db.DbMaintenance.TruncateTable<UnitPerson01xq123gh1, UnitAddress1231sdfal>();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson01xq123gh1
            {
                Name = "John Doe", 
                
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var person2 = new UnitPerson01xq123gh1
            {
                Name = "Tom", 

            };  
            int personId2 = db.Insertable(person2).ExecuteReturnIdentity();

            var person3 = new UnitPerson01xq123gh1
            {
                Name = "Jack",

            };
            int personId3 = db.Insertable(person2).ExecuteReturnIdentity();

            db.Insertable(new UnitAddress1231sdfal()
            {
                 PersonIds=new int[] { personId, personId2 },
                 Street="北京街道1"
            }).ExecuteCommand();

            db.Insertable(new UnitAddress1231sdfal()
            {
                PersonIds = new int[] { personId3 },
                Street = "北京街道2"
            }).ExecuteCommand(); 
            var list=db.Queryable<UnitAddress1231sdfal>().Includes(s => s.Persons).ToList();

            if (list.First().Persons.Count != 2) throw new Exception("unit error");
            if (list.Last().Persons.Count != 1) throw new Exception("unit error");
        }
        public class UnitPerson01xq123gh1
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; } 
        }

        public class UnitAddress1231sdfal
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
            [SqlSugar.SugarColumn(IsJson =true)]
            public int[] PersonIds { get; set; }

            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToManyByArrayList, nameof(PersonIds))]
            public List<UnitPerson01xq123gh1> Persons { get; set; }
        }
    }

}
