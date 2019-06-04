using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DemoF_Utilities
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Utilities Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });


            List<int> ids = Enumerable.Range(1, 100).ToList();
            db.Utilities.PageEach(ids, 10, list =>
            {
                Console.WriteLine(string.Join("," ,list));   
            });

            var list2= db.Utilities.DataTableToList<Order>(db.Ado.GetDataTable("select * from `order`"));

            //more https://github.com/sunkaixuan/SqlSugar/wiki/f.Utilities
            Console.WriteLine("#### Utilities End ####");
        }
    }
}
