using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;
namespace NewTest.Dao
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class SugarDao
    {
        //禁止实例化
        private SugarDao()
        {

        }
        public static string ConnectionString
        {
            get
            {
                string reval = "server=.;uid=sa;pwd=sasa;database=SqlSugarTest"; //这里可以动态根据cookies或session实现多库切换
                return reval;
            }
        }
        public static SqlSugarClient GetInstance()
        {
            return new SqlSugarClient(ConnectionString);
        }
    }
}