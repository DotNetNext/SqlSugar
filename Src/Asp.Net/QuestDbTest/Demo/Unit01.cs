using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class Unit01
    {
        public static void Init()
        {
            var client = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "host=localhost;port=8812;username=admin;password=quest;database=qdb;ServerCompatibilityMode=NoTypeLoading;",
                DbType = DbType.QuestDB,
                LanguageType = LanguageType.Chinese,
                IsAutoCloseConnection = true
            });

            client.CodeFirst.InitTables<TestUserEntity>();

            TestUserEntity[] users = new TestUserEntity[]
            {
                new TestUserEntity(){Name="zhangsan",cate="a" },
                new TestUserEntity(){Name="lisi",cate="b" }
            };

            client.Queryable<TestUserEntity>().Where(u => u.cate.Equals("a")).ToList();

            client.Queryable<TestUserEntity>().Where(u => u.cate.Equals(users[0].cate)).ToList();

            client.CodeFirst.InitTables<UnitPage>();
            if (!client.Queryable<UnitPage>().Any())
            {
                client.Insertable(new UnitPage() { id = 1 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 2 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 3 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 4 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 4 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 6 }).ExecuteCommand();
                client.Insertable(new UnitPage() { id = 7 }).ExecuteCommand();
            }
            var first = client.Queryable<UnitPage>().Take(1).ToList();
            var first2 = client.Queryable<UnitPage>().First();
            var pagr1 = client.Queryable<UnitPage>().ToPageList(1, 2);
            var pagr2 = client.Queryable<UnitPage>().ToPageList(2, 2);
            var pagr3 = client.Queryable<UnitPage>().ToPageList(0, 2);
        }
        public class UnitPage 
        {
            public int id { get; set; }
        }

        [SugarTable("TestUser")]
        public class TestUserEntity
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public string cate { get; set; }
        }
    }
}
