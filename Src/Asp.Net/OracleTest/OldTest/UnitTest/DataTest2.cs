using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.UnitTest
{
    public class DataTest2 : UnitTestBase
    {
        public interface IEntity<T> {
            T ID { get; set; } 
        }
        public abstract class Entity<T> : IEntity<T>
        {
           public virtual T ID { get; set; }
        }
        public class MyModel: Entity<int>
        {
            public override int ID { get; set; }
        }
        private DataTest2() { }
        public DataTest2(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            var t1 = db.Queryable<MyModel>().AS("Student").ToList();
        }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            return db;
        }
    }
}
