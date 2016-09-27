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
    //插入
    public class Insert : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Inset.Init");
            using (var db = SugarDao.GetInstance())
            {

                db.Insert(GetInsertItem()); //插入一条记录 (有主键也好，没主键也好，有自增列也好都可以插进去)


                db.InsertRange(GetInsertList()); //批量插入 支持（别名表等功能）


                db.SqlBulkCopy(GetInsertList()); //批量插入 适合海量数据插入，要求实体与数据库一致，不支持(别名表、排除列等功能)



                //设置不插入列
                db.DisableInsertColumns = new string[] { "sex" };//sex列将不会插入值
                Student s = new Student()
                {
                    name = "张" + new Random().Next(1, int.MaxValue),
                    sex = "gril"
                };
                var id = db.Insert(s); //插入
                //查询刚插入的sex是否有值
                var sex = db.Queryable<Student>().Single(it => it.id == id.ObjToInt()).sex;//无值
                var name = db.Queryable<Student>().Single(it => it.id == id.ObjToInt()).name;//有值
            }
        }

        private static List<Student> GetInsertList()
        {
            List<Student> list = new List<Student>()
                {
                     new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                },
                 new Student()
                {
                     name="张"+new Random().Next(1,int.MaxValue)
                }
                };
            return list;
        }

        private static Student GetInsertItem()
        {
            Student s = new Student()
            {
                name = "张" + new Random().Next(1, int.MaxValue)
            };
            return s;
        }
    }
}
