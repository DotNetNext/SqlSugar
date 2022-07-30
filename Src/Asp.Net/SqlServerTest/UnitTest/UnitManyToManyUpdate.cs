using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlSugar;
using static OrmTest.Program;
namespace OrmTest
{
    public class UnitManyToManyUpdate
    {
        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                 Console.WriteLine(sql);
            };
            //建表 
            db.CodeFirst.InitTables<SchoolA111, SchoolAndRoomA111, RoomA111>();
            db.DbMaintenance.TruncateTable<SchoolA111, SchoolAndRoomA111, RoomA111>();
            //插入数据
            db.Insertable(new SchoolA111 { SchoolId = 1, SchoolName = "清华" }).ExecuteCommand();
            db.Insertable(new SchoolAndRoomA111 { SchoolId = 1, RoomId = 1, Weight = 1 }).ExecuteCommand();
            db.Insertable(new SchoolAndRoomA111 { SchoolId = 1, RoomId = 2, Weight = 1 }).ExecuteCommand();
            db.Insertable(new RoomA111 { RoomId = 1, RoomName = "甲号房" }).ExecuteCommand();
            db.Insertable(new RoomA111 { RoomId = 2, RoomName = "乙号房" }).ExecuteCommand();
            //用例代码
            //查关系表数据
            var data = db.Queryable<SchoolAndRoomA111>()
                .Includes(it => it.School)
                .Includes(it => it.Room)
                .First();
            //修改A表或B表数据
            data.Room.RoomName = "丙号房";
            //执行导航更新
            //报错Sql (Update关系表中的AB表主键)：
            //   UPDATE [SchoolAndRoomA]  SET [RoomId]=1  WHERE  SchoolId = 1
            var result = db.UpdateNav(data)
              .Include(it => it.School)
              .Include(it => it.Room)
                .ExecuteCommand();
         
        }
        //建类
        public class SchoolA111
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            public string SchoolName { get; set; }
        }
        public class SchoolAndRoomA111
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
            [SugarColumn(IsPrimaryKey = true)]
            public int RoomId { get; set; }
            public int? Weight { get; set; }
            [SugarColumn(IsNullable = true)]
            [Navigate(NavigateType.OneToOne, nameof(SchoolId))]
            public SchoolA111 School { get; set; }
            [SugarColumn(IsNullable = true)]
            [Navigate(NavigateType.OneToOne, nameof(RoomId))]
            public RoomA111 Room { get; set; }
        }
        public class RoomA111
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int RoomId { get; set; }
            public string RoomName { get; set; }
        }
    }
}