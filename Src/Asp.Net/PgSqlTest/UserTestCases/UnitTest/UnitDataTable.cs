using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitDataTable
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadfadsfa>();
            var list=new List<Unitadfadsfa>() { new Unitadfadsfa() { Id=Guid.NewGuid(), Name="a" },
            new Unitadfadsfa() { Id=Guid.NewGuid(),Name="a" }};
            var dt = db.Utilities.ListToDataTable(list);
            dt.TableName = "Unitadfadsfa";
            var x=db.Storageable(dt).WhereColumns("id").ToStorage();
            x.AsUpdateable.ExecuteCommand();
            x.AsInsertable.ExecuteCommand();
        }
    }
    public class Unitadfadsfa 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
