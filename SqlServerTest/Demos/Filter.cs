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

            var sql = db.Queryable<Student>().ToSql();

            var sql2 = db.Queryable<Student, School>((f, s) => new object[] { JoinType.Left, f.SchoolId == s.Id }).ToSql();

            //Specify name filter 
            var sql3 = db.Queryable<Student>().Filter("query1").ToSql();

            //Specify key filter  and disabled global filter
            string key = "query1";
            var sql4 = db.Queryable<Student>().Filter(key,true).ToSql();

        }

        public static SqlSugarClient GetInstance1()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
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
