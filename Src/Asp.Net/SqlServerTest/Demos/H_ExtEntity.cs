using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    /// <summary>
    /// mapping  ef attribute 
    /// </summary>
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
                         if (property.Name == "xxx") {// by name ignore column
                             column.IsIgnore = true;
                         }
                         var attributes = property.GetCustomAttributes(true);//get all attributes 
                           
                         if (attributes.Any(it => it is KeyAttribute))// by attribute set primarykey
                         { 
                             column.IsPrimarykey = true;
                         }
                     },
                     EntityNameService = (type,entity) => {
                         var attributes = type.GetCustomAttributes(true); 
                         if (attributes.Any(it => it is TableAttribute))
                         { 
                             entity.DbTableName = (attributes.First(it => it is TableAttribute)as TableAttribute).Name;
                         }
                     }
                 }
            });
     
            var sql=db.Queryable<StudentTest>().ToList();
            var sql2 = db.Insertable<StudentTest>(new StudentTest()).ExecuteCommand();
        }

    }

    [Table(Name ="student")]//default 
    public class StudentTest {
        
        [Key]
        public string Id { get; set; }
        public string xxx { get; set; }
        public string Name { get; set; }
    }
}
