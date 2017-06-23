using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Demo
{
    public class Delete:DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            //by entity
            var t1 = db.Deleteable<Student>().Where(new Student() { Id = 1 }).ExecuteCommand();

            //use lock
            var t2 = db.Deleteable<Student>().With(SqlWith.RowLock).ExecuteCommand();


            //by primary key
            var t3 = db.Deleteable<Student>().In(1).ExecuteCommand();

            //by primary key array
            var t4 = db.Deleteable<Student>().In(new int[] { 1, 2 }).ExecuteCommand();

            //by expression
            var t5 = db.Deleteable<Student>().Where(it => it.Id == 1).ExecuteCommand();
        }
    }
}
