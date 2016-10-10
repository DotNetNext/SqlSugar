using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkDapper.Demos;

namespace PkDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            WarmUp wu = new WarmUp();//预热处理

             //设置执行的DEMO
            string switchOn = "1";
            IDemos demo = null;
            switch (switchOn)
            {
                //比拼实体转换性能
                case "1": demo = new SelectBigData(); break;
                //比拼细节处理性能
                case "2": demo = new SelectSingle(); break;
                //比拼海量数据更新
                case "3": demo = new UpdateList(); break;
                //比拼海量数据挺入
                case "4": demo = new InsertList(); break;
                //比拼批量删除
                case "5": demo = new DeleteArray(); break;
                default: Console.WriteLine("switchOn的值错误，请输入正确的 case"); break;
            }
            demo.Init();
            Console.WriteLine("执行成功请关闭窗口");
            Console.ReadKey();
        }
    }
}
