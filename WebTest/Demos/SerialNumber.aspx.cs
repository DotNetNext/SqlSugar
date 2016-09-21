using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;

namespace WebTest.Demos
{
    /// <summary>
    /// 全局配置流水号,插入就不需要定任何逻辑
    /// </summary>
    public partial class SerialNumber : System.Web.UI.Page
    {
     
            protected void Page_Load(object sender, EventArgs e)
            {
                using (SqlSugarClient db = SugarDaoSerNum.GetInstance())//开启数据库连接
                {
                    var obj =Convert.ToInt32( db.Insert<Student>(new Student() { }));

                    var name = db.Queryable<Student>().Single(it => it.id == obj).name;
   

                    var obj2 = Convert.ToInt32( db.Insert<School>(new School() { }));

                    var name2=db.Queryable<School>().Single(it => it.id == obj2).name;
                }
            }
  
        /// <summary>
        /// 扩展SqlSugarClient
        /// </summary>
        public class SugarDaoSerNum
        {
            //禁止实例化
            private SugarDaoSerNum()
            {

            }
            /// <summary>
            /// 页面所需要设置流水号的对象
            /// </summary>
            private static List<PubModel.SerialNumber> _nums = new List<PubModel.SerialNumber>(){
              new PubModel.SerialNumber(){TableName="Student", FieldName="name", GetNumFunc=()=>{ //GetNumFunc在没有事中使用
                  return "stud-"+DateTime.Now.ToString("yyyy-MM-dd");
              }},
                new PubModel.SerialNumber(){TableName="School", FieldName="name",  GetNumFuncWithDb=db=>{ //事务中请使用GetNumFuncWithDb保证同一个DB对象,不然会出现死锁
                  return "ch-"+DateTime.Now.ToString("syyyy-MM-dd");
              }}
            };

            public static SqlSugarClient GetInstance()
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings[@"sqlConn"].ToString(); //这里可以动态根据cookies或session实现多库切换
                var reval = new SqlSugarClient(connection);
                //设置流水号
                reval.SetSerialNumber(_nums);
                return reval;
            }
        }
    }
}