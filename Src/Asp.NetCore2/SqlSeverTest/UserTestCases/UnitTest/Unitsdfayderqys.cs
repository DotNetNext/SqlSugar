using NetTaste;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsdfayderqys
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<WarehoseReqDetailEntity, SysItemsEntity>();
            db.DbMaintenance.TruncateTable<WarehoseReqDetailEntity, SysItemsEntity>();
            db.Insertable(new WarehoseReqDetailEntity()
            {
                agrname = "测试农资",
                agrtype = "1",
                agrclass = "化肥",
                factory = "测试工厂",
                intnum = 100,
                warehosereqno = "REQ001",
                strno = ""
            }).ExecuteReturnEntity();

            db.Insertable(new SysItemsEntity()
            {
                fid = "1",
                fencode = "ITEM001",
                fparentid = "1",
                ffullname = "",
                fistree = true,
                flayers = 1
                // 其他需要的测试字段
            }).ExecuteReturnEntity();


            var list = db.Queryable<WarehoseReqDetailEntity>().Includes(x => x.SysItems)
               // .GroupBy(x => new { x.agrname, x.agrtype, x.agrclass, x.factory })
                .Select(x => new {  x.factory, x.SysItems })
                .ToList();

            var list2 = db.Queryable<WarehoseReqDetailEntity>().Includes(x => x.SysItems)
              // .GroupBy(x => new { x.agrname, x.agrtype, x.agrclass, x.factory })
             .Select(x => new  DTO{ factory=x.factory, SysItems=x.SysItems })
             .ToList();

            if (list.First().SysItems.fencode != "ITEM001") throw new Exception("unit error");
            if (list2.First().SysItems.fencode != "ITEM001") throw new Exception("unit error");
        }
        [SugarTable("WarehoseReqDetail")]
        public class WarehoseReqDetailEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string strno { get; set; }
            public string agrname { get; set; }
            public string agrtype { get; set; }
            public string factory { get; set; }
            public string agrclass { get; set; }
            public Int32 intnum { get; set; }
            public string warehosereqno { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(agrtype), nameof(SysItemsEntity.fid))]
            public SysItemsEntity SysItems { get; set; }

        }
        [SugarTable("SysItems")]
        public class SysItemsEntity
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string fid { get; set; }
            public string fparentid { get; set; }
            public string fencode { get; set; }
            public string ffullname { get; set; }
            public Boolean fistree { get; set; }
            public Int32 flayers { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(fparentid), nameof(SysItemsEntity.fid))]
            public SysItemsEntity Parent { get; set; }

        } 
        public class DTO
        {
            public string factory { get; set; }

            public Unitsdfayderqys.SysItemsEntity SysItems { get; set; }
        }
    } 
}
