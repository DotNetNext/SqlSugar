using SqlSugar.DbConvert;
using SqlSugar;
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
            db.Aop.DataExecuting = (s, y) =>
            {
            };
            db.CodeFirst.InitTables<UnitCustomeradfafas1>();
            db.DbMaintenance.TruncateTable<UnitCustomeradfafas1>();
            db.Insertable(new UnitCustomeradfafas1()
            {
                CustomerId = 1,
                Name = "name",
                Address = new UnitAddressadfafa2221()
                {
                    City = "city",
                    Street = "street",
                    ZipCode = "zipCode",
                    DbType = DbType.MySql
                }

            }).ExecuteCommand();
            var data = db.Queryable<UnitCustomeradfafas1>().First();
            if (data.Name != "name" || data.Address.City != "city")
            {
                throw new Exception("unit error");
            }
            db.Updateable(new UnitCustomeradfafas1()
            {
                CustomerId = 1,
                Name = "name2",
                Address = new UnitAddressadfafa2221()
                {
                    City = "city2",
                    Street = "street2",
                    ZipCode = "zipCode2",
                     DbType=DbType.MySql
                }

            }).ExecuteCommand();

            data = db.Queryable<UnitCustomeradfafas1>().First();
            if (data.Name != "name2" || data.Address.City != "city2")
            {
                throw new Exception("unit error");
            }

            var list = db.Queryable<UnitCustomeradfafas1>().ToList();

    

            db.Insertable(new List<UnitCustomeradfafas1>(){
                new UnitCustomeradfafas1()
                {
                    CustomerId = 3,
                    Name = "name3",
                    Address = new UnitAddressadfafa2221()
                    {
                        City = "city3",
                        Street = "street3",
                        ZipCode = "zipCode3",
                            DbType=DbType.MySql
                    }

                 },
                new UnitCustomeradfafas1()
                {
                    CustomerId = 4,
                    Name = "name4",
                    Address = new UnitAddressadfafa2221()
                    {
                        City = "city4",
                        Street = "street4",
                        ZipCode = "zipCode4",
                            DbType=DbType.MySql
                    }

                 }
            }).ExecuteCommand();

            var list2 = db.Queryable<UnitCustomeradfafas1>()
                .Where(it => it.Address.City == "city2")
                .Select(it => new
                {
                    Street = it.Address.Street
                }).ToList();

            if (list2.Single().Street != "street2")
            {
                throw new Exception("unit error");
            }

            if (db.Queryable<UnitCustomeradfafas1>().Count() != 3) 
            {
                throw new Exception("unit error");
            }
            var list3=db.Queryable<UnitCustomeradfafas1>().ToList();
            data = list3.Last();
            if (data.Name != "name4" || data.Address.City != "city4")
            {
                throw new Exception("unit error");
            }
        }
    }
    public class UnitAddressadfafa2221
    {
        public string Street { get; set; }
        public string City { get; set; }
     
        public string ZipCode { get; set; }
        [SugarColumn(ColumnDataType ="VARCHAR(20)",SqlParameterDbType = typeof(EnumToStringConvert))]//CommonPropertyConvertORM自带的 
        public DbType DbType { get; set; }

    }
    public class UnitCustomeradfafas1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int CustomerId { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsOwnsOne = true)]
        public UnitAddressadfafa2221 Address { get; set; }
  
    }
}
