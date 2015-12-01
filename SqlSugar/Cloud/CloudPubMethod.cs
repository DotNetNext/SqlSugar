using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            foreach (var config in configList)
            {
                for (int i = 0; i < config.Rate; i++)
                {
                    connectionNameList.Add(config.ConnectionName);
                }
            }
            var index = random.Next(0, connectionNameList.Count - 1);
            return connectionNameList[index];
        }
    }
}
