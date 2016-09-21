using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;
using SqlSugar;

namespace WebTest.Demos
{
    public partial class CreateModelFile2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //自定义实体模板引擎模块
            ClassTemplate.Template = "using System;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace $namespace\r\n{\r\n    public class $className\r\n    {\r\n        public $className() \r\n        { \r\n            this.CreateTime = DateTime.Now;\r\n            this.IsRemove = 0;\r\n            this.UpdateTime=DateTime.Now;\r\n            this.$primaryKeyName=Guid.NewGuid().ToString(\"N\").ToUpper();\r\n        }\r\n        $foreach\r\n    }\r\n}\r\n";
            using (var db = SugarDao.GetInstance())
            {
                db.ClassGenerating.CreateClassFiles(db, "e:/TestModel", "Models");
                //生成的Model带构造函数并且赋了默认值 ，如下注释

            }
        }
        //public class InsertTest
        //{
        //    public InsertTest()
        //    {
        //        this.CreateTime = DateTime.Now;
        //        this.IsRemove = 0;
        //        this.UpdateTime = DateTime.Now;
        //        this.id = Guid.NewGuid().ToString("N").ToUpper();
        //    }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:False 
        //    /// </summary>
        //    public int id { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public string v1 { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public string v2 { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public string v3 { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public int? int1 { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public DateTime? d1 { get; set; }

        //    /// <summary>
        //    /// Desc:- 
        //    /// Default:- 
        //    /// Nullable:True 
        //    /// </summary>
        //    public string txt { get; set; }

        //}
    }
}