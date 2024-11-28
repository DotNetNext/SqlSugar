using SqlSugar;
using System.Threading.Tasks;
using System;
using OrmTest;

namespace Sqlquestion1
{
    internal class Unitdfa1212
    {
       public  static void Init()
        {
            var db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = SqlSugar.DbType.ClickHouse,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (sql, pars) => {
                var log = $"【{DateTime.Now}——执行SQL】\r\n{UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars)}\r\n";
                Console.WriteLine(log);
            };

            //建表 
            db.CodeFirst.InitTables<Test001>();
            //清空表
            db.DbMaintenance.TruncateTable<Test001>();

            var mode = new Test001() {  CreateUserId=null,CreatedOnUtc = DateTime.UtcNow };
            //mode.CreateUserId ??= 0;

            //插入测试数据
            var result =   db.Insertable(mode).ExecuteCommand();//用例代码




            //   Console.WriteLine(result);
            Console.WriteLine("用例跑完"); 
        }


        //建类
        [SugarTable("Test001111")]
        [SqlSugar.ClickHouse.CKTable(@"engine = MergeTree PARTITION BY toYYYYMM(CreatedOnUtc)
        ORDER BY(toYYYYMM(CreatedOnUtc))
        SETTINGS index_granularity = 8192;")]
        public class Test001
        {
            /// <summary>
            /// 创建时间
            /// </summary>
            [SugarColumn(ColumnDescription = "创建时间")]
            public DateTime CreatedOnUtc { get; set; }

            /// <summary>
            /// 创建者Id
            /// </summary>
            [SugarColumn(ColumnDescription = "创建者Id", IsNullable = true)]
            public virtual long? CreateUserId { get; set; }
        }
    }
}
