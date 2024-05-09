using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest.Test
{
    public class BugTest5

    {

        [SugarTable("User")]

        public class User

        {

            [SugarColumn(IsNullable = false, ColumnDataType = "uuid", IsPrimaryKey = true)]

            public Guid Id { get; set; }



            /// <summary>

            /// 部门名称

            /// </summary>

            [SugarColumn(IsNullable = false)]

            public string Name { get; set; }



            /// <summary>

            /// 用户部门

            /// </summary>

            [SugarColumn(IsNullable = false, IsJson = true, ColumnDataType = "json")]

            public List<Department> UserDpt { get; set; }

        }



        [SugarTable("User2")]

        public class User2

        {

            [SugarColumn(IsNullable = false, ColumnDataType = "uuid", IsPrimaryKey = true)]

            public Guid Id { get; set; }



            [SugarColumn(IsNullable = false, ColumnDataType = "uuid")]

            public Guid UserId { get; set; }

        }



        public class Department

        {

            public Guid Id { get; set; }

            public string Name { get; set; }

        }



        public static void Init()

        {

            SqlSugarClient db = new SqlSugarClient(

             new ConnectionConfig()

             {

                 ConnectionString = Config.ConnectionString,

                 DbType = DbType.PostgreSQL,//设置数据库类型

                 IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放

                 InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息

             });

            db.Aop.OnLogExecuted = (s,p) =>//SQL报错

            {

                Console.WriteLine(s);

            };

            //db.DbMaintenance.CreateDatabase();

            db.CodeFirst.InitTables(typeof(User), typeof(User2));

            List<Department> departments = new List<Department>();

            departments.Add(new Department { Id = Guid.NewGuid(), Name = "研发部" });

            departments.Add(new Department { Id = Guid.NewGuid(), Name = "市场部" });

            Guid userId = Guid.NewGuid();

            User user = new User

            {

                Id = userId,

                Name = "张三",

                UserDpt = departments

            };

            db.Insertable(user).ExecuteCommand();

            User2 user2 = new User2();

            user2.Id = Guid.NewGuid();

            user2.UserId = userId;



            db.Insertable(user2).ExecuteCommand();

            var data = db.Queryable<User, User2>((a, b) => new object[] { JoinType.Inner, a.Id == b.UserId })

             .Where((a, b) => a.Id.ToString() == userId.ToString())

                .Select((a, b) => new

                {

                    User = a ,
                    de=a.UserDpt

                }).ToList();

            Console.ReadKey();

        }

    }
}
