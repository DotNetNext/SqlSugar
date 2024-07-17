using SqlSugar.DbConvert;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OrmTest
{
    internal class UnitValueObject
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
           
            db.CodeFirst.InitTables<UnitafasMyEntity>();
            db.Insertable(new UnitafasMyEntity()
            {
                 Id=Guid.NewGuid(),
                  P0="a",
                   P1=1,
                    MyVO=new MyVO() { 
                     GuidP1=Guid.NewGuid(),
                      GuidP2=Guid.NewGuid(),
                       VoId=Guid.NewGuid()
                    }
            }).ExecuteCommand();
            var list5 = db.Queryable<UnitafasMyEntity>()
                .Select(e => new
                {
                    e.Id,
                    e.MyVO.GuidP1,
                    e.MyVO.GuidP2
                })
                .ToListAsync().GetAwaiter().GetResult();

            var list6 = db.Queryable<UnitafasMyEntity>() 
              .ToListAsync().GetAwaiter().GetResult();
        }
    }
    public class UnitafasMyEntity
    {
        public Guid Id { get; set; }

        public string P0 { get; set; }

        public int P1 { get; set; }

        [SugarColumn(IsOwnsOne = true)]
        public MyVO MyVO { get; set; }
    }

    public class MyVO
    {
        public Guid VoId { get; set; }

        public Guid GuidP1 { get; set; }

        public Guid GuidP2 { get; set; }
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
