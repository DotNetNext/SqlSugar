using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.BugTest
{
    public class Bug2
    {
        public  SqlSugarClient DB
        {
            get {
                SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
                {
                    InitKeyType = InitKeyType.Attribute,
                    ConnectionString = Config.ConnectionString,
                    DbType = DbType.SqlServer,
                    IsAutoCloseConnection = true
                });
                return db;
            }
        }
        public void Init() {
           var x2= DB.Queryable<School>().Where(x => x.Id == SqlFunc.Subqueryable<School>().Where(y =>  y.Id == SqlFunc.Subqueryable<Student>().Where(yy => y.Id == x.Id).Select(yy => yy.Id)).Select(y => y.Id)).ToSql();
            if (!x2.Key.Contains("STudent")) {
               // throw new Exception("bug2 error");
            }
        }
    }
       

 
 
}