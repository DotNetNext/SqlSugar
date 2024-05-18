using OrmTest;
using SqlSugar;
using SqlSugar.OceanBaseForOracle;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace OceanBaseForOracle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("#### MasterSlave Start ####");

            //程序启动时加上只要执行一次
            InstanceFactory.CustomAssemblies =
                     new System.Reflection.Assembly[] { typeof(OceanBaseForOracleProvider).Assembly };


            //OceanBase Oracle 模式用这个 DbType.OceanBaseForOracle
            //OceanBase MySql 模式用DbType.MySql不要用这个
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.OceanBaseForOracle,//Oracle 模式用这个 ，如果是MySql 模式用DbType.MySql
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
            };

            Console.WriteLine(db.Ado.IsValidConnection());
            if (db.DbMaintenance.IsAnyTable("OrderTest", false))
            {
                //创建表
                db.DbMaintenance.DropTable<OrderTest>();
                //测试修改表
                db.CodeFirst.InitTables<OrderTest>();
                db.CodeFirst.InitTables<OrderTest>();
            }

            db.Insertable(new OrderTest() { Id = 109, Name = "abc", CustomId = 1, CreateTime = DateTime.Now })
            .ExecuteReturnSnowflakeId();

            db.Deleteable<OrderTest>().Where(m => m.Id == 109).ExecuteCommand();

            db.Updateable<OrderTest>().SetColumns(m => new OrderTest
            {
                Name = "我是修改"
            }).Where(m => m.Id == 2).ExecuteCommand();

            var pageList = db.Queryable<OrderTest>().OrderBy(m => m.Id).ToOffsetPage(1, 3);
            Console.WriteLine("#### MasterSlave End ####");
        }


    }
}