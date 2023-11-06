using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void DeleteTest() 
        {
            Db.CodeFirst.InitTables<UnitDel12311>();
            var list = new List<UnitDel12311>() {
              new UnitDel12311(){
                  Date1=DateTime.Now,
                   ID=Guid.NewGuid(),Name="a"
              },
                new UnitDel12311(){
                  Date1=DateTime.Now,
                   ID=Guid.NewGuid(),Name="a"
              }
            };
            var db = Db;
            //db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings()
            //{
            //     DisableNvarchar=true
            //};
            db.Insertable(list).ExecuteCommand();
            var r= db.Deleteable(list).ExecuteCommand();
            if (r == 0) 
            {
                throw new Exception("unit test delete");
            }
        }
        public class UnitDel12311 
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public Guid ID { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public DateTime Date1 { get; set; }
            public string Name { get; set; }
        }
    }
}
