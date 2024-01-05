#region using
using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace MySqlTest
{
    public class _a9_ToPivot
    {

        public static void Init()
        {

            var db = DbHelper.GetNewDb();
            db.CodeFirst.SetStringDefaultLength(200).InitTables(typeof(RowToColumn));
            Clean();
            InitData();
            Test5();
            Test6();
        }
        public static void InitData()
        {
            var db = DbHelper.GetNewDb();
            var ls = new List<RowToColumn>();
            // 创建 Stopwatch 对象并开始计时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 1; i <= 100 * 10000; i++)
            {
                ls.Add(new RowToColumn()
                {
                    Id = Guid.NewGuid(),
                    Code = "A" + i % 1000,
                    Date = new DateTime(2023, i % 11 + 1, 1),
                    Date2 = new DateTime(2023, i % 11 + 1, i % 28 + 1, i % 24, i % 60, i % 60),
                    Val = i % 10,
                    T1 = i % 3 == 0 ? null : "T0",
                    T2 = i % 3 == 1 ? null : 3
                }); ;
                if (i % 100000 == 0)
                {
                    db.Fastest<RowToColumn>().PageSize(100000).BulkCopy(ls);
                    ls.Clear();
                }
            }

            Task.Run(() =>
            {
            });
            // 结束计时并获取经过的时间
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            Console.WriteLine("T0:" + elapsedTime);
        }
        public static void Clean()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<RowToColumn>();
        }

        public static void Test5()
        {
            var db = DbHelper.GetNewDb();
            // 创建 Stopwatch 对象并开始计时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var s = new DateTime(2023, 2, 1);
            var e = new DateTime(2023, 6, 1).AddMilliseconds(-1);
            var ls = db.Queryable<RowToColumn>()
                .Where(x => x.Date >= s && x.Date <= e)
                .GroupBy(x => new { x.Code, x.Date, x.Val, x.T1, x.T2 })
                .Select(x => new { x.Code, x.Date, x.T1, x.T2, Val = SqlFunc.AggregateSum(x.Val) })//求和可以自动处理空值
                .ToPivotTable(it => it.Code, it => new { it.Date, it.T1, it.T2 }, it => it.Any() ? it.Sum(x => x.Val) : 0);

            // 结束计时并获取经过的时间
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            Console.WriteLine("T5:" + elapsedTime);
        }
        public static void Test6()
        {
            var db = DbHelper.GetNewDb();
            // 创建 Stopwatch 对象并开始计时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var s = new DateTime(2023, 2, 1);
            var e = new DateTime(2023, 6, 1).AddMilliseconds(-1);
            var ls = db.Queryable<RowToColumn>()
               .Where(x => x.Date >= s && x.Date <= e)
               .GroupBy(x => new { x.Code, x.Date, x.Val, x.T1, x.T2 })
               .Select(x => new { x.Code, x.Date, x.T1, x.T2, Val = SqlFunc.AggregateSum(x.Val) })//求和可以自动处理空值
               .ToPivotList(it => it.Code, it => new { it.Date, it.T1, it.T2 }, it => it.Any() ? it.Sum(x => x.Val) : 0);

            // 结束计时并获取经过的时间
            stopwatch.Stop();
            TimeSpan elapsedTime = stopwatch.Elapsed;

            Console.WriteLine("T000:" + elapsedTime);
        }
    }

    [SugarTable("row_2_column")]
    public class RowToColumn
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Date2 { get; set; }
        public string Code { get; set; }
        public decimal? Val { get; set; }
        [SugarColumn(ColumnName = "t_1", IsNullable = true)]
        public string T1 { get; set; }
        [SugarColumn(ColumnName = "t_2", IsNullable = true)]
        public int? T2 { get; set; }
    }
}
