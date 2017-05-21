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
            Ado();
        }

        private static void Ado()
        {
            var db = GetInstance();
            var t1= db.Ado.SqlQuery<string>("select 'a'");
            var t2 = db.Ado.GetInt("select 1");
            var t3 = db.Ado.GetDataTable("select 1 as id");
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

            //top 5
            var top5 = db.Queryable<Student>().Take(5).ToList();

            //skip5
            var skip5 = db.Queryable<Student>().Skip(5).ToList();
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


            //Where If
            string name = null;
            string name2 = "sunkaixuan";
            var list2 = db.Queryable<Student>()
                 .WhereIF(!string.IsNullOrEmpty(name), it => it.Name == name)
                 .WhereIF(!string.IsNullOrEmpty(name2), it => it.Name == name2).ToList();
        }
        public static void Join()
        {
            var db = GetInstance();
            //join 
            var list = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).ToList();

            //join return List<dynamic>
            var list2 = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            }).Select<Student,School,ViewModelStudent>((st,sc)=>new ViewModelStudent { Name= st.Name,SchoolId=sc.Id }).ToList();
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
