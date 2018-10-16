using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class ExtEntity: DemoBase
    {
        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                 ConfigureExternalServices=new ConfigureExternalServices() {
                     EntityService = (property, column) => {
                         if (property.Name == "xxx") {
                             column.IsIgnore = true;
                         }
                        //property.GetCustomAttributes
                     }
                 }
            });
     
            var sql=db.Queryable<StudentTest>().ToSql();
            var sql2 = db.Insertable<StudentTest>(new StudentTest()).ToSql();
        }

    }

    public class StudentTest {
        
        public string Id { get; set; }
        public string xxx { get; set; }
        public string Name { get; set; }
    }
}
