using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public class DbParamInfo
    {
        public string DbParamName { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
    }
}
