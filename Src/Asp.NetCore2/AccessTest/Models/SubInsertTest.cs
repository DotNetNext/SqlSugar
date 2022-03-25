using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class RootTable0
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public TwoItem TwoItem { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public TwoItem2 TwoItem2 { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<TwoItem3> TwoItem3 { get; set; }
    }
    public class TwoItem
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int RootId { get; set; }
        public string Name { get; set; }
    }
    public class TwoItem2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public int RootId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public List<ThreeItem2> ThreeItem2 { get; set; }
    }
    public class TwoItem3
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string  Name { get; set; }
        public string Desc { get; set; }
    }
    public class ThreeItem2
    {
       [SqlSugar.SugarColumn(IsPrimaryKey = true)]

        public int  Id { get; set; }
        public string Name { get; set; }
        public string TwoItem2Id { get; set; }
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
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
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
        [SqlSugar.SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<Province1> Provinces { get; set; }
    }

    public class Province1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<City1> citys { get; set; }
    }

    public class City1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
        public int Id { get; set; }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
    }
}
