using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Demo9_EntityMain
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### EntityMain Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Oscar,
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
            var entityInfo = db.EntityMaintenance.GetEntityInfo<Order>();
            foreach (var column in entityInfo.Columns)
            {
                Console.WriteLine(column.DbColumnName);
            }

            var dbColumnsName = db.EntityMaintenance.GetDbColumnName<EntityMapper>("Name");

            var dbTableName = db.EntityMaintenance.GetTableName<EntityMapper>();

            //more https://github.com/sunkaixuan/SqlSugar/wiki/9.EntityMain
            Console.WriteLine("#### EntityMain End ####");
        }
    }
}
