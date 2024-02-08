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
                CustomerId = 1,
                Name = "name",
                Address = new UnitAddressadfafa()
                {
                    City = "city",
                    Street = "street",
                    ZipCode = "zipCode"
                }

            }).ExecuteCommand();
            var data = db.Queryable<UnitCustomeradfafas>().First();
            if (data.Name != "name" || data.Address.City != "city")
            {
                throw new Exception("unit error");
            }
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

            data = db.Queryable<UnitCustomeradfafas>().First();
            if (data.Name != "name2" || data.Address.City != "city2")
            {
                throw new Exception("unit error");
            }

            var list = db.Queryable<UnitCustomeradfafas>().ToList();

    

            db.Insertable(new List<UnitCustomeradfafas>(){
                new UnitCustomeradfafas()
                {
                    CustomerId = 3,
                    Name = "name3",
                    Address = new UnitAddressadfafa()
                    {
                        City = "city3",
                        Street = "street3",
                        ZipCode = "zipCode3"
                    }

                 },
                new UnitCustomeradfafas()
                {
                    CustomerId = 4,
                    Name = "name4",
                    Address = new UnitAddressadfafa()
                    {
                        City = "city4",
                        Street = "street4",
                        ZipCode = "zipCode4"
                    }

                 }
            }).ExecuteCommand();

            var list2 = db.Queryable<UnitCustomeradfafas>()
                .Where(it => it.Address.City == "city2")
                .Select(it => new
                {
                    Street = it.Address.Street
                }).ToList();

            if (list2.Single().Street != "street2")
            {
                throw new Exception("unit error");
            }

            if (db.Queryable<UnitCustomeradfafas>().Count() != 3) 
            {
                throw new Exception("unit error");
            }
            var list3=db.Queryable<UnitCustomeradfafas>().ToList();
            data = list3.Last();
            if (data.Name != "name4" || data.Address.City != "city4")
            {
                throw new Exception("unit error");
            }
        }
    }
    public class UnitAddressadfafa
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

    }
    public class UnitCustomeradfafas
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int CustomerId { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsOwnsOne = true)]
        public UnitAddressadfafa Address { get; set; }
    }
}
