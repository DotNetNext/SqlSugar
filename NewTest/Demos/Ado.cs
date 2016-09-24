using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;

namespace NewTest.Demos
{
    //完美兼容SqlHelper的所有功能
    public class Ado:IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Ado.Init");
            using (var db = SugarDao.GetInstance())
            {
                var r1 = db.GetDataTable("select * from student");
                var r2 = db.GetSingle<Student>("select top 1 * from student");
                var r3 = db.GetScalar("select  count(1) from student");
                var r4 = db.GetReader("select  count(1) from student");
                r4.Dispose();
                var r5 = db.GetString("select  top 1 name from student");
                var r6 = db.ExecuteCommand("select 1");


                //参数支持
                var p1 = db.GetDataTable("select * from student where id=@id", new {id=1 });
                var p2 = db.GetDataTable("select * from student where id=@id", new Dictionary<string, object>() { { "id", "1" } });//目前只支持 Dictionary<string, object>和Dictionary<string, string>
                var p3 = db.GetDataTable("select * from student where id=@id", new SqlParameter("@id",1) );

            }
        }
    }
}
