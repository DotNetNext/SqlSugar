using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SqlSugar;
using OrmTest.Models;
using System.Data.SqlClient;

namespace OrmTest
{

    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<Student, bool>> exp = it => it.Name!="";
            ExpressionContext expContext = new ExpressionContext(exp, ResolveExpressType.Single);
           // var x = expContext.GetFiledName();
            var xx = expContext.ToString();


            var b = DateTime.Now;
            var config = new AttrbuitesCofnig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = "server=.;uid=sa;pwd=sasa;database=SqlSugar4XTest",
                EntityNamespace = "OrmTest.Models",
                IsAutoCloseConnection = false
            };
            SqlSugarClient db = new SqlSugarClient(config);
            for (int i = 0; i < 1; i++)
            {
                db.Database.IsEnableLogEvent = true;
                db.Database.LogEventStarting = (sql, par) =>
                {
                    Console.WriteLine(sql + " " + par);
                };


                var tb = db.Database.DbMaintenance.GetTableInfoList();
                var tc = db.Database.DbMaintenance.GetColumnInfosByTableName("student");


                //var list = db.Queryable<Student>()
                //    .JoinTable<School>((st,sc)=>st.SchoolId==sc.Id)
                //    .Where("id=2").Where(it => it.Name == "id").Where(it => it.Id > 0).ToList();

                //var list2 = db.Queryable<Student>().Where(it => it.Name == "id").Where(it => it.Id > 0).ToList();
                //var xx = db.SqlQuery<Student>("select * from Student");
                //var cl = db.Database.DbMaintenance.GetColumnInfosByTableName("student");

            }
            var e = DateTime.Now;
            var t = (e - b).TotalMilliseconds;
        }
    }
}
