using OrmTest.Demo;
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
            GetInstance().CodeFirst.InitTables(typeof(MainTable), typeof(SubTable));
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
}).OrderBy((r) => r.RoomNumber, type: OrderByType.Desc).ToPageList(1, 2);
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
