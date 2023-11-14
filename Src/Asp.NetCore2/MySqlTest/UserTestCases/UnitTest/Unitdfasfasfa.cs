using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitdfasfasfa
    {
        public static void Init() 
        {
            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConfigId = "Default",
                ConnectionString =Config.ConnectionString
            };

            ConnectionConfig connectionConfig1 = new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConfigId = "Default1",
                ConnectionString =  Config.ConnectionString
            };

            var db = new SqlSugarClient(new List<ConnectionConfig> { connectionConfig, connectionConfig1 },
                               db =>
                               {
                                   //调试SQL事件，可以删掉 (要放在执行方法之前)
                                   db.Aop.OnLogExecuting = (sql, pars) =>
                                   {
                                       //Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
                                   };
                               });

            var Db = db.GetConnection("Default");
            Console.WriteLine($"Db 的MappingTables 属性为Null： {Db.MappingTables == null}");

            var Db1 = db.GetConnection("Default1");
            Console.WriteLine($"Db1 的MappingTables 属性为Null： {Db1.MappingTables == null}");

            var type = typeof(TestTable);
            Db.CodeFirst.InitTables(type);
            Db1.CodeFirst.InitTables(type);
            Console.WriteLine("Hello, World!");
        }
        [SugarTable("Test_Table_{year}{month}{day}")]
        [SplitTable(SplitType.Month)]
        public class TestTable
        {
            [SugarColumn(ColumnDescription = "创建时间", IsPrimaryKey = true)]
            public long Id { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [SugarColumn(ColumnDescription = "创建时间", CreateTableFieldSort = 993, IsOnlyIgnoreUpdate = true)]
            [SplitField]
            public DateTime CreationTime { get; set; }
        }
    }
}
