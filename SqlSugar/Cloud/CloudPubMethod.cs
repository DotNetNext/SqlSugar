using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{

    internal class CloudPubMethod
    {
        public static Random random = new Random();
        /// <summary>
        /// 根据rate获取随机Connection
        /// </summary>
        /// <param name="configList"></param>
        public static string GetConnection(List<CloudConnectionConfig> configList)
        {
            Check.Exception(configList == null || configList.Count == 0, "CloudPubMethod.GetConnection.configList不能为null并且count>0。");
            List<string> connectionNameList = new List<string>();
            SetConnectionNameList(configList, connectionNameList);
            var index = random.Next(0, connectionNameList.Count - 1);
            return connectionNameList[index];
        }

        /// <summary>
        /// 并行执行任务并且传入索引
        /// </summary>
        /// <param name="method">函数参数i</param>
        /// <param name="tasks">task数组</param>
        /// <param name="i">索引</param>
        public static void TaskFactory<FType>(Func<int, FType> method, Task<FType>[] tasks, int i)
        {
            tasks[i] = Task.Factory.StartNew(() =>
            {
                return method(i);
            }); ;
        }
        private static void SetConnectionNameList(List<CloudConnectionConfig> configList, List<string> connectionNameList)
        {
            var cacheKey = "SetConnectionNameList";
            var cm = CacheManager<List<string>>.GetInstance();
            if (cm.ContainsKey(cacheKey))
            {
                connectionNameList = cm[cacheKey];
            }
            else
            {
                foreach (var config in configList)
                {
                    for (int i = 0; i < config.Rate; i++)
                    {
                        connectionNameList.Add(config.ConnectionName);
                    }
                }
                cm.Add(cacheKey, connectionNameList, cm.Day);
            }
        }
    }
}
