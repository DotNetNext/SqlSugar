using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar  
{ 
    internal class MyTuple
    {
        public bool isDiscrimator;
        public Dictionary<string, string> discrimatorDict;

        public MyTuple(bool isDiscrimator, Dictionary<string, string> dict)
        {
            this.isDiscrimator = isDiscrimator;
            this.discrimatorDict = dict;
        }
    }
}
