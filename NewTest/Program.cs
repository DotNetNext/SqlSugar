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
            string switchOn = "createclass";
            IDemos demo = null;
            switch (switchOn)
            {

                case "select": demo = new Select(); break;
                case "ado": demo = new Ado(); break;
                case "createclass": demo = new CreateClass(); break;
                default: Console.WriteLine("switchOn的值错误，请输入正确的 case"); break;

            }
            //执行DEMO
            demo.Init();

            //更多例子请查看API
            //http://www.cnblogs.com/sunkaixuan/p/5654695.html
            Console.WriteLine("执行成功请关闭窗口");
            Console.ReadKey();
        }
    }
}
