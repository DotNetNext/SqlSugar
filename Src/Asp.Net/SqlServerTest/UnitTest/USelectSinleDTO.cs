using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class USelectSinleDTO
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            Dto1(db);
            Dto2(db);
            Dto3(db);
            db.CodeFirst.InitTables<ClassA>();
            db.Insertable(new ClassA() { fId = 1, username = "a" }).ExecuteCommand();
            var list = db.Queryable<ClassA>().Select<ClassDTO>().ToList();
            if (list.First().fId != 1 && list.First().username != "a") throw new Exception("unit error");
            db.Queryable<ClassA>().OrderBy(it => new { it.fId, it.username }).ToList();
            db.DbMaintenance.DropTable<ClassA>();
        }

        private static void Dto1(SqlSugarClient db)
        {
            var sql = db.Queryable<ClassA>().Select<ClassDTO>().ToSql();
            if (sql.Key != "SELECT [f_id] AS [fId] ,[user_name] AS [username] FROM [ClassA] ")
            {
                throw new Exception("unit error");
            }
        }
        private static void Dto2(SqlSugarClient db)
        {
            var sql = db.Queryable<ClassA>().Select<ClassDTO2>().ToSql();
            if (sql.Key != "SELECT [user_name] AS [username] FROM [ClassA] ")
            {
                throw new Exception("unit error");
            }
        }
        private static void Dto3(SqlSugarClient db)
        {
            var sql = db.Queryable<ClassA>().Select<ClassDTO3>().ToSql();
            if (sql.Key != "SELECT [f_id] AS [f_id] ,[user_name] AS [username] FROM [ClassA] ")
            {
                throw new Exception("unit error");
            }
        }


        public class ClassA 
        {
            [SqlSugar.SugarColumn(ColumnName = "f_id")]
            public int fId { get; set; }
            [SqlSugar.SugarColumn(ColumnName = "user_name")]
            public string username { get; set; }
        }
        public class ClassDTO3
        {

            public int f_id { get; set; }

            public string username { get; set; }
        }
        public class ClassDTO2
        {

            public int fId2 { get; set; }

            public string username { get; set; }
        }
        public class ClassDTO
        {
          
            public int fId { get; set; }
         
            public string username { get; set; }
        }
    }
}
