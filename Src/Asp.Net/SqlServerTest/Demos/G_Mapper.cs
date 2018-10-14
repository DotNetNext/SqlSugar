using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
     public class  Mapper : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();

            //auto fill ViewModelStudent3
            var s11 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
                        .Select<ViewModelStudent3>().ToList();


            var s12 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
            .Select<ViewModelStudent3>().Mapper(it=> {
                it.Name = it.Name==null?"默认值":it.Name;
                it.CreateTime = DateTime.Now.Date;
                it.Id = it.Id*1000;
            }).ToList();
        }
    }
}
