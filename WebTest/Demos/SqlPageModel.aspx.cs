using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
using WebTest.Dao;
namespace WebTest.Demos
{
    public partial class SqlPageModel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())
            {
                db.PageModel = PageModel.Offset;
                var list = db.Queryable<Student>().OrderBy("id").Skip(0).Take(2).ToList();
                var list1 = db.Sqlable().From<Student>("t").SelectToPageList<Student>("*","id",1,2);


                List<School> dataPageList = db.Sqlable()
                .From("school", "s")
                .Join("student", "st", "st.id", "s.id", JoinType.INNER)
                .Join("student", "st2", "st2.id", "st.id", JoinType.LEFT)
                .Where("s.id>100 and s.id<100")
                .SelectToPageList<School>("st.*", "s.id", 1, 10);

                db.PageModel = PageModel.RowNumber;
                var list2 = db.Queryable<Student>().OrderBy("id").Skip(0).Take(2).ToList();
                var list22 = db.Sqlable().From<Student>("t").SelectToPageList<Student>("*", "id", 1, 2);
            }
        }
    }
}