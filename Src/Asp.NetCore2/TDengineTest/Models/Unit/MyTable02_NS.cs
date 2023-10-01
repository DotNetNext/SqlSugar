using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
    /// <summary>
    /// 纳秒
    /// </summary>
    [SugarTable("MyTable02")]
    public class MyTable02_NS
    {
        //也可以用InsertServerTime =true 
        [SugarColumn(IsPrimaryKey = true,SqlParameterDbType =typeof(DateTime19))]
        public DateTime ts { get; set; }
        public float current { get; set; }
        public bool isdelete { get; set; }
        public string name { get; set; }
        public int voltage { get; set; }
        public float phase { get; set; }
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public string location { get; set; }
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public int groupId { get; set; }
    }

    /// <summary>
    /// 微秒
    /// </summary>

    [SugarTable("MyTable02")]
    public class MyTable02_US
    {
        //也可以用InsertServerTime =true 
        [SugarColumn(IsPrimaryKey = true, SqlParameterDbType = typeof(DateTime16))]
        public DateTime ts { get; set; }
        public float current { get; set; }
        public bool isdelete { get; set; }
        public string name { get; set; }
        public int voltage { get; set; }
        public float phase { get; set; }
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public string location { get; set; }
        [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
        public int groupId { get; set; }
    }
}
