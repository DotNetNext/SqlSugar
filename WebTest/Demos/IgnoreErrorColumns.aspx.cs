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
    //自动排除非数据库列
    public partial class IgnoreErrorColumns : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())
            {
       
                //设置别名表具体列子看rename.aspx
                db.SetMappingTables(new List<KeyValue>() { new KeyValue() { Key = "MyStudent", Value = "Student" }, });

                //数据库并没有appendField和appendField2二个字段，设置该属性后将插入可以正常使用
                db.IsIgnoreErrorColumns = true;//自动排除非数据库列  update也一样
          
                db.Insert<MyStudent>(new MyStudent() { name = "张三" });
                //将实体MyStudent插入Student表
            }
        }
    }
    /// <summary>
    /// 继承了学生表
    /// </summary>
    public class MyStudent : Student
    {
        /// <summary>
        /// 这两个字段Student表并不存在
        /// </summary>
        public string appendField { get; set; }
        public string appendField2 { get; set; }
    }
}