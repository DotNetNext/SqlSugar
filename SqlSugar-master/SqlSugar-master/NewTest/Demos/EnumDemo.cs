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
    //枚举的支持
    public class EnumDemo:IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动EnumDemo.Init");
            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                var stuList = db.Queryable<Student>().ToList();
                db.Insert<Student>(new Student() { sch_id = SchoolEnum.北大青鸟 });
                db.Update<Student>(new Student() { sch_id = SchoolEnum.it清华, id = 11 });
                var stuList2 = db.Queryable<Student>().Where(it => it.sch_id == SchoolEnum.全智).ToList();
            }
        }
        public class Student
        {

            /// <summary>
            /// 说明:- 
            /// 默认:- 
            /// 可空:False 
            /// </summary>
            public int id { get; set; }

            /// <summary>
            /// 说明:- 
            /// 默认:- 
            /// 可空:True 
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 说明:- 
            /// 默认:- 
            /// 可空:False 
            /// </summary>
            public SchoolEnum sch_id { get; set; }

            /// <summary>
            /// 说明:- 
            /// 默认:- 
            /// 可空:True 
            /// </summary>
            public string sex { get; set; }

            /// <summary>
            /// 说明:- 
            /// 默认:- 
            /// 可空:False 
            /// </summary>
            public bool isOk { get; set; }

        }

        public enum SchoolEnum
        {
            北大青鸟 = 1,
            it清华 = 2,
            全智 = 3
        }
    }



}
