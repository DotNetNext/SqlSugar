using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;

namespace NewTest.Demos
{
    //单元测试
    public class Test : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Test.Init");
            using (var db = SugarDao.GetInstance())
            {
               


            }
        }
    }
}
