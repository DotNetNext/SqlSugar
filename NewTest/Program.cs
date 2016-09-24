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
            string switchOn = "update";
            IDemos demo = null;
            switch (switchOn)
            {

                //查询
                case "select": demo = new Select(); break;
                //删除
                case "delete": demo = new Delete(); break;
                //插入
                case "insert": demo = new Insert(); break;
                //更新
                case "update": demo = new Update(); break;
                //基层函数的用法
                case "ado": demo = new Ado(); break;
                //创建实体函数
                case "createclass": demo = new CreateClass(); break;
                //枚举支持
                case "enum": demo = new EnumDemo(); break;
                //过滤器
                case "filter": demo = new Filter(); break;
                //过滤器2
                case "filter2": demo = new Filter2(); break;
                //公开含数
                case "pubmethod": demo = new PubMethod(); break;
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
