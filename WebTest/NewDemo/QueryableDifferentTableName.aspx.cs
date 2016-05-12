using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
using WebTest.Dao;

namespace WebTest.NewDemo
{
    public partial class QueryableDifferentTableName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())//开启数据库连接
            {
                var list = db.Queryable<Student>().ToList();
                //如果类名与数据库名不一致可以写在这样
                var list2 = db.Queryable<Student2>("Student").ToList();
                var list2Count = db.Queryable<Student2>("Student").Count();
            }

            using (SqlSugarClient db = SugarDaoMappingTable.GetInstance())//开启数据库连接
            {
                //同样可以将Student2映射成Student
                var list = db.Queryable<Student2>().ToList();
            }
        }
    }


    /// <summary>
    /// 扩展SqlSugarClient
    /// </summary>
    public class SugarDaoMappingTable
    {
        //禁止实例化
        private SugarDaoMappingTable()
        {

        }
        private static List<KeyValue> _mappingTables = new List<KeyValue>()
        {
            new KeyValue(){ Key="Student2", Value="Student"},
            new KeyValue(){ Key="Student3", Value="Student"}
        };
        public static SqlSugarClient GetInstance()
        {
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings[@"sqlConn"].ToString(); //这里可以动态根据cookies或session实现多库切换
            var reval= new SqlSugarClient(connection);
            reval.SetMappingTables(_mappingTables);
            return  reval;
        }
    }
}