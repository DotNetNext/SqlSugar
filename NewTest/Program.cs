using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using NewTest.Demos;
namespace NewTest
{
    class Program
    {
        static void Main(string[] args)
        {
 
                //设置执行的DEMO
                string switch_on = "select";
                IDemos demo = null;
                switch (switch_on)
                {

                    case "select": demo = new Select(); break;
                    default: Console.WriteLine("switch_on没有找到"); break;

                }
                //执行DEMO
                demo.Init();

                //更多例子请查看API
                //http://www.cnblogs.com/sunkaixuan/p/5654695.html
                Console.WriteLine("执行成功请关闭窗口 ,更多例子请查看API:http://www.cnblogs.com/sunkaixuan/p/5654695.html");
                Console.ReadKey();
        }
    }
}
