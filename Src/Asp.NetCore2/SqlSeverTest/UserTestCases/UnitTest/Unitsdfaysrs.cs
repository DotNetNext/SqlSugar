using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsdfaysrs
    {
        public static void Init()
        {
            var _db = NewUnitTest.Db;
            _db.CodeFirst.InitTables<Drivers, Vehicle, DriverCard>();
            _db.DbMaintenance.TruncateTable<Drivers, Vehicle, DriverCard>();

            // 添加一条测试数据到 Drivers
            var testDriver = new Drivers
            {
                TenantId = 1,
                DriverName = "测试司机"
            };
            _db.Insertable(testDriver).ExecuteReturnEntity();

            // 添加一条测试数据到 Vehicle
            var testVehicle = new Vehicle
            {
                TenantId = 1,
                VehNum = "粤A12345",
                VehColor = 1
            };
            _db.Insertable(testVehicle).ExecuteReturnEntity();

            // 添加一条测试数据到 DriverCard
            var testDriverCard = new DriverCard
            {
                Id = testDriver.Id, // 关联 Drivers 主键
                CardName = "测试卡"
            };
            _db.Insertable(testDriverCard).ExecuteCommand();

            var list = _db.Queryable<Drivers>()
                .LeftJoin<DriverCard>((d, dc) => d.Id == dc.Id)
                .LeftJoin<Vehicle>((d, dc, v) => d.TenantId == v.TenantId)
                .Select((d, dc, v) => new DriverCardOutput
                {
                    VehNum = v.VehNum,
                    DriverCardInfo = dc,
                }, true)
                .ToListAsync().GetAwaiter().GetResult();

            _db.Queryable<Drivers>()
                  .Select(s => new
                  {
                      name=s.Id==0?"":SqlFunc.Subqueryable<Drivers>().Where(z=>z.DriverName.Equals(s.DriverName)).Select(z=>z.DriverName)
                  }).ToList();
        }
        [SugarTable("T_DriverCard")]

        public class DriverCard

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int Id { get; set; }




            [SugarColumn(ColumnName = "DC_CompanyName", Length = 50)]

            public string? CardName { get; set; }

        }



        [SugarTable("T_Drivers")]

        public class Drivers

        {

            [SugarColumn(ColumnName = "D_Id", IsPrimaryKey = true, IsIdentity = true)]

            public int Id { get; set; }



            [SugarColumn(ColumnName = "TenantId", IsNullable = true)]

            public int TenantId { get; set; }




            [SugarColumn(ColumnName = "DriverName", Length = 50)]

            public string DriverName { get; set; }




        }



        [SugarTable("T_Vehicle")]

        public class Vehicle

        {

            [SugarColumn(ColumnName = "V_Id", IsPrimaryKey = true, IsIdentity = true)]

            public int VehId { get; set; }



            [SugarColumn(ColumnName = "TenantId")]

            public long TenantId { get; set; }



            [SugarColumn(ColumnName = "VehNum")]

            public string VehNum { get; set; }




            [SugarColumn(ColumnName = "VehColor ")]

            public int VehColor { get; set; }




        }



        public class DriverCardOutput

        {

            public string VehNum { get; set; }



            public DriverCard DriverCardInfo { get; set; }



        }
    }
}
