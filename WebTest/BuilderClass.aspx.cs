using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;

namespace Demo
{
    /// <summary>
    /// 用于创建实体的DEMO,可以自已实现
    /// </summary>
    public partial class BuilderClass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                db.ClassGenerating.CreateClassFiles(db, txtPath.Text, txtNS.Text);
                //还有其它方法我这边只是最简单的
                //db.ClassGenerating.CreateClassFilesByTableNames  
                //db.ClassGenerating....
            }
        }

        protected void btnCreateClassCode_Click(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                txtResult.Value = db.ClassGenerating.SqlToClass(db, txtSql.Text, txtClassName.Text);

            }
        }




    }
}