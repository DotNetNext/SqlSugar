using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;
namespace NewTest.Demos
{
    //别名表的功能
    public class MsterSlave : IDemos
    {

        public void Init()
        {
            SqlSugarClient db = new SqlSugarClient("server=.;uid=sa;pwd=sasa;database=SqlSugarTest", "server=localhost;uid=sa;pwd=sasa;database=SqlSugarTest");

            var l1= db.Queryable<Student>().ToList();//走从
            var d1= db.Queryable<Student>().ToDataTable();//走从
            var t3 = db.Queryable<Student>().ToList();//走从

            db.Insert<Student>(new Student() { name="主" });

            db.BeginTran();
            var l2 = db.Queryable<Student>().ToList();//走主
            db.CommitTran();

            var l3 = db.Queryable<Student>().ToList();//走从
            db.Dispose();
        }
    }
}
