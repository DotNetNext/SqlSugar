using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugarSelect
{
    [SqlSugar.SugarTable("UnitTestModel1")]
    public class TestModel1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Titlt { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "ntext", IsJson = true)]
        public Guid[] Ids { get; set; }
    }
    [SqlSugar.SugarTable("UnitTestModel2")]
    public class TestModel2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int Pid { get; set; }
    }
}
