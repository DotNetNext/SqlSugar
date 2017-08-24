using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
namespace SyntacticSugar
{

    public class PerformanceTest
    {
        private DateTime _beginTime;
        private DateTime _endTime;
        private ParamsModel _params;
        private List<PerformanceTestChartModel> _CharSource = new List<PerformanceTestChartModel>();
        /// <summary>
        ///设置执行次数(默认:1)
        /// </summary>
        public void SetCount(int count)
        {
            _params.RunCount = count;
        }
        /// <summary>
        /// 设置线程模式(默认:false)
        /// </summary>
        /// <param name="isMul">true为多线程</param>
        public void SetIsMultithread(bool isMul)
        {
            _params.IsMultithread = isMul;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PerformanceTest()
        {
            _params = new ParamsModel()
            {
                RunCount = 1
            };
        }

        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="action"></param>
        public void Execute(Action<int> action, Action<string> rollBack, string name = null)
        {
            List<Thread> arr = new List<Thread>();
            _beginTime = DateTime.Now;
            for (int i = 0; i < _params.RunCount; i++)
            {
                if (_params.IsMultithread)
                {
                    var thread = new Thread(new System.Threading.ThreadStart(() =>
                    {
                        action(i);
                    }));
                    thread.Start();
                    arr.Add(thread);
                }
                else
                {
                    action(i);
                }
            }
            if (_params.IsMultithread)
            {
                foreach (Thread t in arr)
                {
                    while (t.IsAlive)
                    {
                        Thread.Sleep(10);
                    }
                }

            }

            _CharSource.Add(new PerformanceTestChartModel() { Name = name, Time = GetTime(), CPU = GetCurrentProcessSize() });
            rollBack(string.Format("总共执行时间：{0}秒", GetTime()));
        }

        private double GetTime()
        {
            _endTime = DateTime.Now;
            double totalTime = ((_endTime - _beginTime).TotalMilliseconds / 1000.0);
            return totalTime;
        }

        public List<PerformanceTestChartModel> GetChartSource()
        {
            return _CharSource;
        }
        private Double GetCurrentProcessSize()
        {
            Process processes = Process.GetCurrentProcess();
            var processesSize = (Double)(processes.WorkingSet64);
            return processesSize / (1024 * 1024);
        }

        private class ParamsModel
        {
            public int RunCount { get; set; }
            public bool IsMultithread { get; set; }
        }
        public class PerformanceTestChartModel
        {
            public string Name { get; set; }
            public double Time { get; set; }
            public double CPU { get; set; }
        }
    }


}