using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UTran2
    {
        public static void Init()
        {

            SqlSugarScope db = new SqlSugarScope(new List<ConnectionConfig>()
        {
            new ConnectionConfig(){ ConfigId="1", DbType=DbType.SqlServer, ConnectionString=Config.ConnectionString,InitKeyType=InitKeyType.Attribute,IsAutoCloseConnection=true },
            new ConnectionConfig(){ ConfigId="2", DbType=DbType.SqlServer, ConnectionString=Config.ConnectionString2 ,InitKeyType=InitKeyType.Attribute ,IsAutoCloseConnection=true}
        });
            var count = 0;
            var insertCount = 0;
            try
            {
                db.BeginTran();
                count=db.GetConnection(1).Queryable<Order>().Count();
                db.GetConnection(1).Insertable(new Order() { Name = "a", Price = 1, CreateTime = DateTime.Now, CustomId = 1 }).ExecuteCommand();
                insertCount = db.GetConnection(1).Queryable<Order>().Count();
                db.GetConnection(2).Insertable(new ORDER() { Name = "a", Price = 1, CreateTime = DateTime.Now, CustomId = 1 }).ExecuteCommand();

                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
            }
            var currCount = db.GetConnection(1).Queryable<Order>().Count();
            if (count != currCount) 
            {
                throw new Exception("unit error");
            }
            if (insertCount == count)
            {
                throw new Exception("unit error");
            }
        }
    }
    public class ORDER
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        public decimal Price { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsNullable = true)]
        public int CustomId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<OrderItem> Items { get; set; }
    }
}
