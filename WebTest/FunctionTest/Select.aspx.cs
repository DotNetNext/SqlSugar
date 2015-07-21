using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
namespace WebTest.FunctionTest
{
    public partial class Select : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int sid = 1;
            var school= GetSingleSchool(sid);
            var studentList=  GetStudent("孙","男",1,10,"id asc,name");
        }

        public School GetSingleSchool(int id)
        {
            using (SugarDao db = new SugarDao())
            {
                return db.Queryable<School>().Single(c => c.id == id);
            }
        }

        /// <summary>
        /// 根据条件查询并且分页
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static List<Student> GetStudent(string name, string sex,int pageIndex, int pageSize, string orderFileds)
        {
            using (SugarDao db = new SugarDao())
            {
                var qable = db.Queryable<Student>();
                if (!string.IsNullOrEmpty(name))
                {
                    qable = qable.Where(it => it.name.Contains(name));
                }
                if (!string.IsNullOrEmpty(sex))
                {
                    qable = qable.Where(it => it.sex == sex);
                }
                if (!string.IsNullOrEmpty(orderFileds))//无需担心注入
                {
                    qable = qable.OrderBy(orderFileds);
                }
                return qable.ToPageList(pageIndex,pageSize);
            }
        }
    }
}