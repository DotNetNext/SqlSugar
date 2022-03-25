using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class DemoL_Snowflake
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### DemoL_Snowflake ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
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
            db.CodeFirst.InitTables<SnowflakeModel>();
            Console.WriteLine(db.Queryable<SnowflakeModel>().Count());
            var id= db.Insertable(new SnowflakeModel()
            {
                Name="哈哈"
            }).ExecuteReturnSnowflakeId();
            var ids = db.Insertable(db.Queryable<SnowflakeModel>().Take(10).ToList()).ExecuteReturnSnowflakeIdList();
            Console.WriteLine(db.Queryable<SnowflakeModel>().Count());
        }
    }
    public class SnowflakeModel 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }

        public string Name{get;set; }
    }
}
