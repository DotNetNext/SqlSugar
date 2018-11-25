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
            var t41 = db.Deleteable<Student>().In(new int[] { 1, 2 }.Select(it=>it)).ExecuteCommand();
            var t42 = db.Deleteable<Student>().In(new int[] { 1, 2 }.AsEnumerable()).ExecuteCommand();


            //by exp key array
            var t44 = db.Deleteable<Student>().In(it=>it.SchoolId,new int[] { 1, 2 }).ExecuteCommand();
            var t441 = db.Deleteable<Student>().In(it => it.SchoolId,new int[] { 1, 2 }.Select(it => it)).ExecuteCommand();
            var t442 = db.Deleteable<Student>().In(it => it.SchoolId,new int[] { 1, 2 }.AsEnumerable()).ExecuteCommand();
            var t443 = db.Deleteable<Student>().In(it => it.SchoolId, new int[] { 1, 2 }.ToList()).ExecuteCommand();

            //by expression   id>1 and id==1
            var t5 = db.Deleteable<Student>().Where(it => it.Id > 1).Where(it => it.Id == 1).ExecuteCommand();

            var t6 = db.Deleteable<Student>().AS("student").Where(it => it.Id > 1).Where(it => it.Id == 1).ExecuteCommandAsync();
            t6.Wait();
        }
    }
}
