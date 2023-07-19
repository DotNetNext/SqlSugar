using OrmTest;
using SqlSugar;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace OceanBaseForOracle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("#### MasterSlave Start ####");
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
                Console.WriteLine(db.Ado.Connection.ConnectionString);
            };
            Console.WriteLine("Master:");
            db.Insertable(new Order() { Id=109,Name = "abc", CustomId = 1, CreateTime = DateTime.Now }).ExecuteCommand();
            db.Deleteable<Order>().Where(m => m.Id == 109).ExecuteCommand();
            db.Updateable<Order>().SetColumns(m => new Order
            {
                Name = "我是修改"
            }).Where(m => m.Id == 2).ExecuteCommand();
            Console.WriteLine("Slave:");
            //var s = db.Queryable<Order>().First();
            //var list = db.Queryable<Order>().Select(m => new Order
            //{
            //    Id = m.Id,
            //    CreateTime = m.CreateTime,
            //    CustomId = m.CustomId,
            //    Idname = SqlFunc.Subqueryable<Order>().Where(s => s.Id == 2).Select(s => s.Name),
            //    Name = m.Name,
            //    Price = m.Price,
            //}).ToList();
            //var grouplist = db.Queryable<Order>().OrderByDescending(m=>m.Id).GroupBy(m=>new {m.Id,m.Name}).SelectMergeTable(m => new Order
            //{
            //    Id = m.Id,
            //    Name = m.Name,
            //    CreateTime= SqlFunc.AggregateMin(m.CreateTime),
            //    Price= SqlFunc.AggregateSum(m.Price),
            //}).OrderBy(m=>m.Id).Where(m=>m.Id==1).ToList();
            //var orderlist = db.Queryable<Order>().OrderBy(m => new { m.Id, m.Name }).ToList();
            var pageList = db.Queryable<Order>().OrderBy(m=>m.Id).ToOffsetPage(1, 3);
            Console.WriteLine("#### MasterSlave End ####");
        }
    }
}