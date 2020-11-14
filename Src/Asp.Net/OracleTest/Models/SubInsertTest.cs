using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class SubInsertTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,OracleSequenceName ="Seq_id")]
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
        [SqlSugar.SugarColumn(IsPrimaryKey = true, OracleSequenceName = "Seq_id")]
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
    public class Country
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<Province> Provinces { get; set; }
    }

    public class Province
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<City> citys { get; set; }
    }

    public class City
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
    }


    public class Country1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true,   OracleSequenceName = "seq_id")]
        public int Id { get; set; }
        public string Name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<Province1> Provinces { get; set; }
    }

    public class Province1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, OracleSequenceName = "seq_id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<City1> citys { get; set; }
    }

    public class City1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, OracleSequenceName ="seq_id")]
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
    }
}
