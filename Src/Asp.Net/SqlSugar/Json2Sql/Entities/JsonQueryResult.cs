using System;
using System.Collections.Generic;
using System.Text;

namespace  SqlSugar
{
    public class JsonQueryResult
    {
        public object Data { get; set; }
        public Dictionary<string, string> TableInfo{get;set;}
        public int ToTalRows { get;  set; }
    }
}
