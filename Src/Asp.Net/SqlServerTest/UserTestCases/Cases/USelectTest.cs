using SqlSugar;
using System.Linq;

namespace OrmTest
{
    public class USelectTest
    {
        public class SelectMenuDomain
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public int Pid { get; set; }
            public string Name { get; set; }
        }
        public class SelectMenuResponse
        {
            public int Id { get; set; }
            public int Pid { get; set; }
            public string Name { get; set; }
            /// <summary>
            /// 父类名称
            /// </summary>
            public string PName { get; set; }
        }
        public static void Init()
        {
            var db = NewUnitTest.Db;
            //   db.Aop.OnLogExecuting = null;

            db.CodeFirst.InitTables<SelectMenuDomain>();
            db.DbMaintenance.DropTable<SelectMenuDomain>();
            db.CodeFirst.InitTables<SelectMenuDomain>();

            db.Insertable(new SelectMenuDomain() { Id = 1, Name = "顶级菜单" }).ExecuteCommand();
            db.Insertable(new SelectMenuDomain() { Id = 2, Pid = 1, Name = "子集菜单" }).ExecuteCommand();

            TestAA(db);

        }
        private static void TestAA(SqlSugarClient db)
        {
            var test11 = db.Queryable<SelectMenuDomain>().ToList();
            var test1 = db.Queryable<SelectMenuDomain>()
               .Select(a => new SelectMenuResponse()
               {
                   PName = SqlFunc.Subqueryable<SelectMenuDomain>().Where(d => d.Id == a.Pid).Select(d => d.Name)
               },
               true)
             .ToList();
            if (test1.Last().PName != "顶级菜单") 
            {
                throw new System.Exception("unit error");
            }
            if (test1.Last().Id != 2)
            {
                throw new System.Exception("unit error");
            }
            if (test1.Last().Name!= "子集菜单")
            {
                throw new System.Exception("unit error");
            }
        }
    }
}