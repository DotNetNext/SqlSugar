using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;

namespace WebTest.Demo
{
    /// <summary>
    /// 创建实体文件
    /// </summary>
    public partial class CreateModelFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {

                //根据当前数据库生成所有表的实体类文件 （参数：SqlSugarClient ，文件目录，命名空间）
                db.ClassGenerating.CreateClassFiles(db,Server.MapPath("~/Models"),"Models");
                //根据表名生成实体类文件
                //db.ClassGenerating.CreateClassFilesByTableNames(db, Server.MapPath("~/Models"), "Models" , "student","school");

                //根据表名生成class字符串
                var str = db.ClassGenerating.TableNameToClass(db, "Student");

                //根据SQL语句生成class字符串
                var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from Student", "student");
            }

        }
    }
}