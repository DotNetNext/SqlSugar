using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Query
    {

        public static void Init()
        {
            Easy();
            Page();
            Where();
            Join();
            Funs();
            Select();
        }

        public static void Easy()
        {
            var db = GetInstance();
            var getAll = db.Queryable<Student>().ToList();
            var getByPrimaryKey = db.Queryable<Student>().InSingle(2);
            var getByWhere = db.Queryable<Student>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByFuns = db.Queryable<Student>().Where(it => NBORM.IsNullOrEmpty(it.Name)).ToList();
        }

        public static void Page()
        {
            var db = GetInstance();
            var pageIndex = 1;
            var pageSize = 2;
            var totalCount = 0;
            var page = db.Queryable<Student>().ToPageList(pageIndex, pageSize, ref totalCount);

            var pageJoin = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).ToPageList(pageIndex, pageSize, ref totalCount);
        }
        public static void Where()
        {
            var db = GetInstance();
            //join 
            var list = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .Where<School>(sc => sc.Id == 1)
            .Where<Student>(st => st.Id == 1)
            .Where<Student, School>((st, sc) => st.Id == 1 && sc.Id == 2).ToList();

            //SELECT [st].[Id],[st].[SchoolId],[st].[Name],[st].[CreateTime] FROM [Student] st 
            //Left JOIN School sc ON ( [st].[SchoolId] = [sc].[Id] )   
            //WHERE ( [sc].[Id] = @Id0 )  AND ( [st].[Id] = @Id1 )  AND (( [st].[Id] = @Id2 ) AND ( [sc].[Id] = @Id3 ))
        }
        public static void Join()
        {

        }
        public static void Funs()
        {

        }
        public static void Select()
        {

        }

        public static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new SystemTablesConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            db.Ado.IsEnableLogEvent = true;
            db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.RewritableMethods.SerializeObject(pars));
                Console.WriteLine();
            };
            return db;
        }
    }
}
