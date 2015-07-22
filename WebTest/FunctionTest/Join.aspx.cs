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
    public partial class Join : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                var list = db.Sqlable().Form("student", "s").Join("school", "l", "s.sch_id", "l.id", JoinType.INNER).SelectToList<Student>("*");

            }
        }
    }
}