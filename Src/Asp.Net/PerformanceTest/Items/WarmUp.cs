using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SqlSugar;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
namespace PerformanceTest.Items
{
    public class WarmUp
    {
        public WarmUp()
        {
            Console.WriteLine("开启预热");
          
            Console.WriteLine("预热完毕");
            Console.WriteLine("----------------比赛开始-------------------");
        }
    }
}
