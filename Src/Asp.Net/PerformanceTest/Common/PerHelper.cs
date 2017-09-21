using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace PerformanceTest
{
    /// <summary>
    /// 性能测试类,用于循环执行代码并统计时间
    /// </summary>
    public class PerHelper
    {
        public static void Execute(int count, string title, Action fun)
        {
            SyntacticSugar.PerformanceTest ptef = new SyntacticSugar.PerformanceTest();
            ptef.SetCount(count);//执行count次
            ptef.Execute(
                        i =>
                        {
                            fun();

                        },
                        res =>
                        {
                            Console.WriteLine(string.Format("执行{0}次，{1}{2}", count, title, res));
                        });

        }

    }
}
