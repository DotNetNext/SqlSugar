using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SqlSugar; 
using TDengineDriver; 

namespace TDengineTest
{
    public partial class ORMTest
    {

        public static void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.TDengine,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(UtilMethods.GetNativeSql(sql, p));
                    }
                }
            });

            //建表
            CodeFirst(db);

            //生成实体
            DbFirst(db);

            //简单用例
            Demo1(db);

            //测试用例 （纳秒、微妙、类型）
            UnitTest(db);

            Console.WriteLine("执行完成");
            Console.ReadKey();
        }

    }

}
