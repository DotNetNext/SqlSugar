using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PkDapper.Demos;

namespace PkDapper
{
    class Program
    {
     
        /// <summary>
        /// SqlSugar与Dapper的性能比较
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            WarmUp wu = new WarmUp();//预热处理

             //设置执行的DEMO
            string switchOn = "6";
            IDemos demo = null;
            switch (switchOn)
            {
                //查询所有
                case "1": demo = new SelectBigData(); break;

                //查询单条
                case "2": demo = new SelectSingle(); break;

                //比拼海量数据更新
                case "3": demo = new UpdateList(); break;

                //比拼海量数据插入
                case "4": demo = new InsertList(); break;

                //比拼批量删除
                case "5": demo = new DeleteArray(); break;

                //分页
                case "6": demo = new Page(); break;

                //比拼普通插入
                case "7": demo = new InsertItem(); break;

                //比拼普通更新
                case "8": demo = new UpdateItem(); break;
                default: Console.WriteLine("switchOn的值错误，请输入正确的 case"); break;
            }
            demo.Init();
            Console.WriteLine("执行成功请关闭窗口");
            Console.ReadKey();
        }
    }
}
