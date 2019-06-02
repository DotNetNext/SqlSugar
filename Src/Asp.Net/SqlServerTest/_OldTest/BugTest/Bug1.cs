using Models;
using OrmTest.Demo;
using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.BugTest
{
    public class Bug1
    {
        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                InitKeyType = InitKeyType.Attribute,
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return db;
        }
        public void Init()
        {
            var communityId = "";
            var buildId = "";
            var unitId = "";
            var keyword = "";
            GetInstance().CodeFirst.InitTables(typeof(MainTable), typeof(SubTable), typeof(Brand), typeof(VendorAndBrand));
            GetInstance().Queryable<MainTable>().Where(u =>
            (u.CommunityID == communityId || SqlFunc.IsNullOrEmpty(communityId)) &&
            (SqlFunc.Contains(u.BuildID, buildId) || SqlFunc.IsNullOrEmpty(buildId)) &&
            (SqlFunc.Contains(u.UnitID, unitId) || SqlFunc.IsNullOrEmpty(unitId)) &&
            (SqlFunc.Contains(u.RoomNumber, keyword) || SqlFunc.Contains(u.RoomerName, keyword) ||
            SqlFunc.Contains(u.HousePlace, keyword) || SqlFunc.Contains(u.UnitName, keyword) ||
            SqlFunc.Contains(u.BuildName, keyword) || SqlFunc.IsNullOrEmpty(keyword)))
            .GroupBy(ru => new { ru.RoomNumber, ru.RoomID })
            .Select(ru => new
            {
            RoomNumber = SqlFunc.AggregateMax(ru.RoomNumber),
            CountRoomer = SqlFunc.AggregateCount(ru.RoomerName),
            RoomID = SqlFunc.AggregateMax(ru.RoomID),
            Owner = SqlFunc.Subqueryable<SubTable>().Where(r => r.RoomID == ru.RoomID && SqlFunc.Equals(r.RoomUserType, "业主") && SqlFunc.Equals(r.RoomUserType, "业主")).Select(s => s.RoomerName)
            }).OrderBy((r) => r.RoomNumber, type: OrderByType.Desc).ToPageListAsync(1, 2).Wait();

            GetInstance().Updateable<Student>().UpdateColumns(it =>
            new Student()
            {
                Name = "a".ToString(),
                CreateTime = DateTime.Now.AddDays(-1)

            }
            ).Where(it => it.Id == 1).ExecuteCommand();


            var list = GetInstance().Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id&&st.CreateTime==DateTime.Now.AddDays(-1)
            })
           .Where(st => st.Name == "jack").ToList();


            GetInstance().Updateable<BugStudent>().Where(it => true).UpdateColumns(it => new BugStudent() { Float = 11 }).ExecuteCommand();
            var reslut = GetInstance().Queryable<BugStudent>().ToList();

            var list2 = GetInstance().Queryable<Brand, VendorAndBrand>((b, vb) => new object[] {
                JoinType.Left,b.Id == vb.BrandId
            })
          .Where((b) => b.BrandType == 1).Select((b) => b).ToList();


            var list3 = GetInstance().Queryable<Brand, VendorAndBrand>((b, vb) =>
               b.Id == vb.BrandId)
.          Where((b) => b.BrandType == 1).Select((b) => b).ToList();


            var query = GetInstance().Queryable<Student>().Select(o => o);
                        
            var result = query.ToList();

        }


        [SugarTable("student")]
        public class BugStudent
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string name { get; set; }
            public float? Float { get; set; }

        }
        public class MainTable
        {
            public string CommunityID { get; internal set; }
            public string BuildID { get; internal set; }
            public string UnitID { get; internal set; }
            public string RoomNumber { get; internal set; }
            public string HousePlace { get; internal set; }
            public string BuildName { get; internal set; }
            public string RoomID { get; internal set; }
            public string UnitName { get; internal set; }
            public string RoomerName { get; internal set; }
        }
        public class SubTable
        {
            public string RoomID { get; internal set; }
            public string RoomUserType { get; internal set; }
            public string RoomerName { get; internal set; }
        }
    }
}
