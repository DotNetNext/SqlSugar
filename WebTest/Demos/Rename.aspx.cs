using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;
namespace WebTest.Demo
{
    /// <summary>
    /// 实体映射别名功能
    /// </summary>
    public partial class Rename : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())
            {

                //-----单个-------//
                var list = db.Queryable<V_Student>("Student").ToList();//查询的是 select * from student 而我的实体名称为V_Student




                //-----全局多个设置-------//

                //设置 Mapping Table 如果没这方面需求可以传NULL
                List<KeyValue> mappingTableList = new List<KeyValue>(){
                new KeyValue(){ Key="FormAttr", Value="Flow_FormAttr"},
                new KeyValue(){ Key="Student3", Value="Student"}
                };
                db.SetMappingTables(mappingTableList);
            }
        }
    }
}