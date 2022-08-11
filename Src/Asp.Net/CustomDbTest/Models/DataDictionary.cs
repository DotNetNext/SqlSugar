using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class DataDictionary
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Person 
    {
        //数据库字段
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int SexId { get; set; }
        public int CityId { get; set; }
        public int ProviceId { get; set; }

        //非数据库字段
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public string SexName { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string CityName { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string ProviceName { get; set; }
    }
}
