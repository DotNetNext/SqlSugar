using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitSplitTask
    {
        public static void   Init()
        {
            var client = NewUnitTest.Db; 
            Console.WriteLine("Hello, World!");
            List<Task> tasks = new List<Task>()
            {
              CreateTask(client.CopyNew()),
              CreateTask(client.CopyNew()),
              CreateTask(client.CopyNew())
            };

             Task.WhenAll(tasks).GetAwaiter().GetResult();
            client.Deleteable(new SpitDemoModel()).SplitTable().ExecuteCommand();
        }


        private static Task CreateTask(ISqlSugarClient client)
        {
            return Task.Run(() => {
                client.Insertable(new SpitDemoModel()).SplitTable().ExecuteCommand();
            });
        }
    }

    [SplitTable(SplitType.Day)]
    [SugarTable("SpitDemo_{year}{month}{day}")]
    public class SpitDemoModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [SplitField]
        public DateTime CreateTime { get; set; } = DateTime.Now.AddDays(1);

    }
}
 
