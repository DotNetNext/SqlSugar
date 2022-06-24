using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom024
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var data=db.EntityMaintenance.GetEntityInfoNoCache(typeof(UnitNewP));
            db.CodeFirst.InitTables<UnitNewP>();
            db.Insertable(new UnitNewP() { Id = Guid.NewGuid() }).ExecuteCommand();
            var list=db.Queryable<UnitNewP>().Take(2).ToList();
            db.Updateable(list).ExecuteCommand();
            db.Deleteable(list).ExecuteCommand();
        }
       
    }

    public class UnitNewP: UnitBaseP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public new Guid Id { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public string Name { get; set; }    
    }
    public class UnitBaseP 
    {
        public int Id { get; set; }
    }
}
