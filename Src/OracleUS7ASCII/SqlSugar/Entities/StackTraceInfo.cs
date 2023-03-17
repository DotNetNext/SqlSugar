using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class StackTraceInfo
    {
        public string FirstFileName { get { return this.MyStackTraceList.First().FileName; } }
        public string FirstMethodName { get { return this.MyStackTraceList.First().MethodName; } }
        public int FirstLine { get { return this.MyStackTraceList.First().Line; } }

        public List<StackTraceInfoItem> MyStackTraceList { get; set; }
        public List<StackTraceInfoItem> SugarStackTraceList { get; set; }
    }
    public class StackTraceInfoItem
    {
        public string FileName { get; set; }
        public string MethodName { get; set; }
        public int Line { get; set; }
    }
}
