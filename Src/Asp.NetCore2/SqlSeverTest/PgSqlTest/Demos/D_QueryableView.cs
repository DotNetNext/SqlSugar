using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class QueryableView : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            var q1 = db.Queryable<Student, School>((st,sc)=>new object[] {
                JoinType.Left,st.SchoolId==sc.Id
            }).Select((st, sc) => new ViewModelStudent4() { Id=st.Id, Name=st.Name,SchoolName=sc.Name });

            var q2 = db.Queryable<School>();


            var innerJoinList = db.Queryable(q1, q2, (j1, j2) => j1.Id == j2.Id).Select((j1, j2) => j1).ToList();//inner join

            var leftJoinList = db.Queryable(q1, q2,JoinType.Left, (j1, j2) => j1.Id == j2.Id).Select((j1, j2) => j1).ToList();//left join
        }
    }

    public class ViewModelStudent4 { 
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Name { get; set; }
    }
}
