using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;
namespace SqlSugarDemo
{

    [SugarTable("user_name_simple")]
    public class user_name_simple
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

        public int id { set; get; }
        public string name { set; get; }
        public DateTime create_time { set; get; }
    }

 
}
