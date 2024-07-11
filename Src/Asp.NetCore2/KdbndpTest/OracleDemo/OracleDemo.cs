using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmTest;
namespace KdbndpTest.OracleDemo
{
    internal class OracleDemo
    {
        public static void Init() 
        {

            Demo0_SqlSugarClient.Init();
            Demo1_Queryable.Init();
            Demo2_Updateable.Init();
            Demo3_Insertable.Init();
            Demo4_Deleteable.Init();
            Demo5_SqlQueryable.Init();
            Demo6_Queue.Init();
            Demo7_Ado.Init();
            Demo8_Saveable.Init();
            Demo9_EntityMain.Init();
            DemoA_DbMain.Init();
            DemoB_Aop.Init();
            DemoC_GobalFilter.Init();
            DemoD_DbFirst.Init(); ;
            DemoE_CodeFirst.Init();
            DemoF_Utilities.Init();
            DemoG_SimpleClient.Init();

            //Rest Data
            NewUnitTest.RestData();
        }
    }
}
