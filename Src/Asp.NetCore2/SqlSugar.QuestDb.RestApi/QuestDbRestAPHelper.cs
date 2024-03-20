using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using CsvHelper;

namespace SqlSugar 
{
    internal class QuestDbRestAPHelper
    { 

        /// <summary>
        /// 逐行读取，包含空行
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<string> SplitByLine(string text)
        {
            List<string> lines = new List<string>();
            byte[] array = Encoding.UTF8.GetBytes(text);
            using (MemoryStream stream = new MemoryStream(array))
            {
                using (var sr = new StreamReader(stream))
                {
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    }
                }
            }

            return lines;
        }


    }
}
