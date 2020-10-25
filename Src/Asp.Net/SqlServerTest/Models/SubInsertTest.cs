using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class SubInsertTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public SubInsertTestItem SubInsertTestItem { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public SubInsertTestItem1 SubInsertTestItem1 { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<SubInsertTestItem2> SubInsertTestItem2 { get; set; }
    }

    public class SubInsertTestItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Name { get; set; }
    }
    public class SubInsertTestItem1
    {
        public string a { get; set; }
    }
    public class SubInsertTestItem2
    {
        public int OrderId { get; set; }
        public int  xid { get; set; }
        public string a { get; set; }
    }
}
