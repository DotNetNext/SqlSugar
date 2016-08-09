using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;

namespace WebTest.Demos
{
    public partial class EnumType : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                var stuList= db.Queryable<Student>().ToList();
                db.Insert<Student>(new Student() { sch_id = SchoolEnum.蓝翔2 });
                db.Update<Student>(new Student() { sch_id = SchoolEnum.蓝翔2, id = 1 });
                var stuList2 = db.Queryable<Student>().Where(it => it.sch_id == SchoolEnum.蓝翔1).ToList();
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
            public string isOk { get; set; }

        }

        public enum SchoolEnum
        {
            蓝翔1 = 1,
            蓝翔2 = 2
        }
    }
}