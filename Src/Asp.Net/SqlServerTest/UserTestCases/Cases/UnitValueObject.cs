using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitValueObject
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UnitCustomeradfafas>();
            db.DbMaintenance.TruncateTable<UnitCustomeradfafas>();
            db.Insertable(new UnitCustomeradfafas()
            {
                  CustomerId=1,
                  Name="name",
                   Address=new UnitAddressadfafa()
                   {
                        City="city",
                         Street= "street",
                          ZipCode= "zipCode"
                   }
                  
            }).ExecuteCommand();
            db.Updateable(new UnitCustomeradfafas()
            {
                CustomerId = 1,
                Name = "name2",
                Address = new UnitAddressadfafa()
                {
                    City = "city2",
                    Street = "street2",
                    ZipCode = "zipCode2"
                }

            }).ExecuteCommand();
            var list=db.Queryable<UnitCustomeradfafas>().ToList();

            var list2 = db.Queryable<UnitCustomeradfafas>()
                .Where(it=>it.Address.City== "city2")
                .Select(it=>new {
                    Street = it.Address.Street
                }).ToList();

           
        }
    }
    public class UnitAddressadfafa
    {
        public string Street { get;   set; }
        public string City { get;   set; }
        public string ZipCode { get;   set; }
         
    }
    public class UnitCustomeradfafas
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public int CustomerId { get;   set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsOwnsOne =true)]
        public UnitAddressadfafa Address { get; set; } 
    }
}
