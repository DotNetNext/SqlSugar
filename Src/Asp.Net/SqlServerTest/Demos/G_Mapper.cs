using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Mapper : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();

            //auto fill ViewModelStudent3
            var s11 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
                        .Select<ViewModelStudent3>().ToList();


            var s12 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select<ViewModelStudent3>()

                .Mapper((it, cache) =>
                {

                    var allSchools = cache.GetListByPrimaryKeys<School>(vmodel => vmodel.SchoolId);//in（ViewModelStudent3[0].SchoolId , ViewModelStudent3[1].SchoolId...）

                    //Equal to the following writing.
                    //var allSchools2= cache.Get(list =>
                    // {
                    //     var ids=list.Select(i => it.SchoolId).ToList(); 
                    //     return db.Queryable<School>().In(ids).ToList();
                    //});Complex writing metho



                    it.School = allSchools.FirstOrDefault(i => i.Id == it.SchoolId);//one to one

                    it.Schools = allSchools.Where(i => i.Id == it.SchoolId).ToList();//one to many

                    it.Name = it.Name == null ? "null" : it.Name;

                }).ToList();


            var s13 = db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id).Select<ViewModelStudent3>()

                   .Mapper((it, cache) =>
                   {
                       it.Schools = db.Queryable<School>().Where(i => i.Id == it.SchoolId).ToList();
                   }).ToList();


        }
    }
}
