using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace TDengineTest
{
    public partial class ORMTest
    {

        private static void Demo1(SqlSugarClient db)
        {
            //建库
            db.DbMaintenance.CreateDatabase();

            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  St01 (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT, isdelete BOOL, name BINARY(64)) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable02 using St01 tags('California.SanFrancisco',1)");


            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable02 ");


            //插入单条子表
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommand();

            //批量插入子表
            db.Insertable(GetInsertDatas()).ExecuteCommand();


            //查询子表(主表字段也能查出来)
            var list = db.Queryable<MyTable02>().OrderBy(it => it.ts).ToList();
            var list1 = db.Queryable<MyTable02>().OrderBy(it => it.ts)
                .Select(it=>new {
                  date=it.ts.Date,
                  ts=it.ts
                }).ToList();

            //条件查询
            var list2 = db.Queryable<MyTable02>().Where(it => it.name == "测试2").ToList();
            var list22 = db.Queryable<MyTable02>().Where(it => it.voltage == 222).ToList();
            var list222 = db.Queryable<MyTable02>().Where(it => it.phase == 1.2).ToList();
            var list2222 = db.Queryable<MyTable02>().Where(it => it.isdelete == true).ToList();

            //模糊查询
            var list3 = db.Queryable<MyTable02>().Where(it => it.name.Contains("a")).ToList();

            //时间差函数 
            var list31 = db.Queryable<MyTable02>().Select(it =>
            new
            {
                diff = SqlFunc.DateDiff(DateType.Day, it.ts, DateTime.Now),
                time = it.ts
            }).ToList();

            //时间加1天
            var list32 = db.Queryable<MyTable02>().Select(it =>
              new
              {
                  addTime = SqlFunc.DateAdd(it.ts, 1, DateType.Day),
                  oldime = it.ts
              }).ToList();

            //自定义函数:实现时间加1天
            var list33 = db.Queryable<MyTable02>().Select(it =>
              new
              {
                  addTime = SqlFunc.MappingColumn<DateTime>(" `ts`+1d "),
                  oldime = it.ts
              }).ToList();

            //分页
            var Count = 0;
            var list4 = db.Queryable<MyTable02>().Where(it => it.voltage == 111)
                .ToPageList(1, 2, ref Count);

            //删除子表
            var ts = list.First().ts;
            var de = DateTime.Now.AddYears(-1);
            var count = db.Deleteable<MyTable02>().Where(it => it.ts > de).ExecuteCommand();


            //异步
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommandAsync().GetAwaiter().GetResult();

            var list100 = db.Queryable<MyTable02>().ToListAsync().GetAwaiter().GetResult();

            //联表查询:不支持left join只能这样
            var list101 = db.Queryable<MyTable02, MyTable02>((x, y) => x.ts == y.ts)
                .Select((x, y) => new
                {
                    xts = x.ts,
                    yts = y.ts
                }).ToList();
            //联表查询在分页
            var list102 = db.Queryable<MyTable02, MyTable02>((x, y) => x.ts == y.ts)
            .Select((x, y) => new
            {
                xts = x.ts,
                yts = y.ts
            }).ToPageList(1, 2);
        }
    }
}
