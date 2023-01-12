using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitOneToOneN
    {
        public static void Init() 
        {
            //创建数据库对象
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.Access,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars));//输出sql,查看执行sql 性能无影响
            };


            //建表 

            if (!db.DbMaintenance.IsAnyTable("StudentA", false))

            {

                db.CodeFirst.InitTables<StudentA>();

            }

            if (!db.DbMaintenance.IsAnyTable("SchoolA", false))

            {

                db.CodeFirst.InitTables<SchoolA>();

            }

            if (!db.DbMaintenance.IsAnyTable("RoomA", false))

            {

                db.CodeFirst.InitTables<RoomA>();

            }
            db.Deleteable<StudentA>().ExecuteCommand();
            db.Deleteable<SchoolA>().ExecuteCommand();
            db.Insertable(new StudentA() { SchoolNo = 100, StudentId = 0 }).ExecuteCommand();
            db.Insertable(new SchoolA() { SchoolId = 100, RoomId = 1 }).ExecuteCommand();

            //用例代码 

            var result = db.Queryable<StudentA>()

               .Includes(x => x.School)

               .ToList();

            if (result.First().School==null) 
            {
                throw new Exception("unit error");
            }
        }

        public class StudentA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int StudentId { get; set; }

            public int SchoolNo { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(SchoolNo) )]

            public SchoolA School { get; set; }



        }

        public class SchoolA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int SchoolId { get; set; }

            public int SchoolNo { get; set; }

            public int RoomId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(RoomId))]

            public RoomA Room { get; set; }

        }

        public class RoomA

        {

            [SugarColumn(IsPrimaryKey = true)]

            public int RoomId { get; set; }

        }
    }
}
