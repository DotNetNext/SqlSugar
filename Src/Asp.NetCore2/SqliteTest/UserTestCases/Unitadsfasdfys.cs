using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{ 

    internal class Unitfasdfasysfs
    {
        public static void Init()
        {

            var db = NewUnitTest.Db;
       
            db.CodeFirst.InitTables<Log, LogType>();

            db.DbMaintenance.TruncateTable<Log, LogType>();

            // 插入10条测试数据
            for (int i = 0; i < 10; i++)
            {
                db.Insertable(new Log
                {
                    Id = i + 1,
                    AddTime = DateTime.Now.AddDays(-i),
                }).SplitTable().ExecuteCommand();
                db.Insertable(new LogType
                {
                    Name = "Type" + i
                }).ExecuteCommand();
            }

            Console.WriteLine("===========================================================");
            try
            {
                var beginTime = DateTime.Now.Date.AddDays(-3);
                var endTime = beginTime.AddDays(5);
                var list = db.Queryable<Log>()
                        .LeftJoin<LogType>((callLog, logType) => callLog.TypeId == logType.Id)
              .Where(callLog => callLog.AddTime >= beginTime && callLog.AddTime <= endTime)
              .SplitTable(beginTime, endTime)
              .Select((callLog) => new LogData()
              {
                  //Id = callLog.TypeId,
                  Data = new LogItem()
                  {
                      Time = callLog.AddTime,
                  }
              }).ToList();
                Console.WriteLine($"查询到数据 {list.Count} 条");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        [SplitTable(SplitType.Month)]//按年分表 （自带分表支持 年、季、月、周、日）
        [SugarTable("Unit_log_{year}{month}{day}")]//生成表名格式 3个变量必须要有
        public class Log
        {
            /// <summary>
            /// Id
            /// </summary>
            [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnName = "type_id")]
            public int TypeId { get; set; }

            [SplitField]
            [SugarColumn(ColumnName = "add_time")]
            public DateTime AddTime { get; set; }
        }


        [SugarTable("UintLogType1")]
        public class LogType
        {
            /// <summary>
            /// Id
            /// </summary>
            [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnName = "name", Length = 20)]
            public string Name { get; set; }
        }

        public class LogData
        {
            public int Id { get; set; }

            public LogItem Data { get; set; }
        }
        public class LogItem
        {
            public int Id { get; set; }


            public DateTime? Time { get; set; }
        }
    }
}
