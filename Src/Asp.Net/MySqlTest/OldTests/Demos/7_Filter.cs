using OrmTest.Demo;
using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Filter : DemoBase
    {
        public static void Init()
        {


            //gobal filter
            var db = GetInstance1();

            var sql = db.Queryable<Student>().OrderBy(it=>it.Id).ToSql();
            //SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  WHERE  isDelete=0 

            var sql2 = db.Queryable<Student, School>((f, s) => new object[] { JoinType.Left, f.SchoolId == s.Id }).ToSql();
            //SELECT[f].[ID],[f].[SchoolId],[f].[Name],[f].[CreateTime]
            //FROM[STudent] f Left JOIN `School` s ON([f].[SchoolId] = [s].[Id])   WHERE f.isDelete=0 


            //Specify name filter 
            var sql3 = db.Queryable<Student>().Filter("query1").ToSql();
            //SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  WHERE  WHERE  id>@id  AND  isDelete=0 


            //Specify key filter  and disabled global filter
            string key = "query1";
            var sql4 = db.Queryable<Student>().Filter(key,true).ToSql();
            //SELECT [ID],[SchoolId],[Name],[CreateTime] FROM [STudent]  WHERE  WHERE  id>@id  

        }

        public static SqlSugarClient GetInstance1()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.MySql, IsAutoCloseConnection = true });
            db.QueryFilter
             .Add(new SqlFilterItem()
             {
                 FilterValue = filterDb =>
                 {
                     return new SqlFilterResult() { Sql = " isDelete=0" };
                 },
                 IsJoinQuery = false 
             }).Add(new SqlFilterItem()
             {
                 FilterValue = filterDb =>
                 {
                     return new SqlFilterResult() { Sql = " f.isDelete=0" };
                 },
                 IsJoinQuery = true
             })
            .Add(new SqlFilterItem()
            {
                FilterName = "query1",
                FilterValue = filterDb =>
                {
                    return new SqlFilterResult() { Sql = " id>@id", Parameters = new { id = 1 } };
                },
                IsJoinQuery = false
            });
            return db;
        }
    }

}
