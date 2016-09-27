using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;

namespace WebTest.Demos
{
    /// <summary>
    /// 插入海量数据
    /// </summary>
    public partial class InsertBigData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                var list = new List<Student>()
                {
                    new Student(){ isOk=true, name="张三",  sch_id=1, sex="男"},
                    new Student(){ isOk=true, name="sun",  sch_id=1, sex="女"},
                    new Student(){ isOk=true, name="mama",  sch_id=1, sex="gril"}
                };
                db.SqlBulkCopy<Student>(list);
            }
        }
    }
}