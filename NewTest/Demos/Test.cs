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
    public class MyCost {public static Student2 p = new Student2() { id = 5, name = "张表", isOk = true };}
    //单元测试
    public class Test : IDemos
    {
        public Student2 Getp() {  return new Student2() { id = 4, name = "张表" , isOk=true};}
        public Student2 _p = new Student2() { id = 3, name = "张表", isOk = true };
        public void Init()
        {
            Student2 p = new Student2() { id = 2, name = "张表", isOk = true };
            Console.WriteLine("启动Test.Init");
            using (var db = SugarDao.GetInstance())
            {

                //解析拉姆达各种情况组合

                //基本
                var t1 = db.Queryable<Student>().Where(it => it.id == 1).ToList();
                var t2 = db.Queryable<Student>().Where(it => it.id == p.id).ToList();
                var t3 = db.Queryable<Student>().Where(it => it.id == _p.id).ToList();
                var t4 = db.Queryable<Student>().Where(it => it.id == Getp().id).ToList();
                var t5 = db.Queryable<Student>().Where(it => it.id == MyCost.p.id).ToList();


                //BOOL
                var t11 = db.Queryable<Student2>("Student").Where(it => it.isOk).ToList();
                var t21 = db.Queryable<Student2>("Student").Where(it => it.isOk==MyCost.p.isOk).ToList();
                var t31 = db.Queryable<Student2>("Student").Where(it => !it.isOk).ToList();
                var t41 = db.Queryable<Student2>("Student").Where(it => it.isOk==true).ToList();
                var t51 = db.Queryable<Student2>("Student").Where(it =>it.isOk==false).ToList();
                var t61 = db.Queryable<Student2>("Student").Where(it => it.isOk == _p.isOk).ToList();
                var t71 = db.Queryable<Student2>("Student").Where(it => it.isOk == !p.isOk).ToList();

            }
        }
    }
}
