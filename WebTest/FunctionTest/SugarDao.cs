using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace WebTest.FunctionTest
{
    /// <summary>
    /// SqlSugar数据访问
    /// </summary>
    public class SugarDao : SqlSugar.SqlSugarClient
    {
        //可以根据自已习惯重载多个
        public SugarDao(string connStr = @"Server=.;uid=sa;pwd=sasa;database=SqlSugarTest")
            : base(connStr)
        {

        }

    }
}