using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace WebTest.Dao
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class SugarDao
    {
        //禁止实例化
        private SugarDao() { 

        }
        public static SqlSugarClient GetInstance()
        {
            string connection = System.Configuration.ConfigurationManager.ConnectionStrings[@"sqlConn"].ToString(); //这里可以动态根据cookies或session实现多库切换
            return new SqlSugarClient(connection);
        }
    }
}