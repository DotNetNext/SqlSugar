using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitTestConfigQuery
    {
        public static void Init() 
        {

            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<DataDictionary1, DataMain>();
            db.DbMaintenance.TruncateTable<DataDictionary1, DataMain>();
            List<DataDictionary1> datas = new List<DataDictionary1>();
            datas.Add(new DataDictionary1() { Code = "1", Name = "男", Type = "sex" });
            datas.Add(new DataDictionary1() { Code = "2", Name = "女", Type = "sex" });
            datas.Add(new DataDictionary1() { Code = "1", Name = "南通市", Type = "city" });
            datas.Add(new DataDictionary1() { Code = "2", Name = "苏州市", Type = "city" });
            datas.Add(new DataDictionary1() { Code = "1", Name = "江苏省", Type = "province" });
            datas.Add(new DataDictionary1() { Code = "2", Name = "湖南省", Type = "province" });
            db.Insertable(datas).ExecuteCommand();//这样就能把数据插进数据库了
            db.Insertable(new DataMain() { SexCode="1", Province = "2" }).ExecuteCommand();
            db.Insertable(new DataMain() { SexCode = "2", Province="1" }).ExecuteCommand();
            db.Insertable(new DataMain() { SexCode = "3", Province = "1" }).ExecuteCommand();
            var list=db.Queryable<DataMain>()
                .Includes(x => x.SexInfo)
                .Includes(x => x.ProvinceInfo)
                .ToList();
            if (list.First().SexInfo.Name != "男" || list.First().ProvinceInfo.Name != "湖南省"||
                list.Last().SexInfo!=null) 
            {
                throw new Exception("unit error");
            }
            var list2 = db.Queryable<DataMain>()
                .Where(x=>x.SexInfo.Name=="男")
                .ToList();
            if (list2.Count == 0 || list2.First().SexCode != "1") 
            {
                throw new Exception("unit error");
            }
        }
        [SugarTable("UnitDataDictionary1")]
        public class DataDictionary1
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }
        [SugarTable("UnitDataMain")]
        public class DataMain 
        {
            public string SexCode { get; set; }
            [SqlSugar.Navigate(NavigateType.OneToOne,nameof(SexCode),nameof(DataDictionary1.Code),"type='sex'")]
            public DataDictionary1 SexInfo { get; set; }

            public string Province { get; set; }
            [SqlSugar.Navigate(NavigateType.OneToOne, nameof(Province), nameof(DataDictionary1.Code), "type='province'")]
            public DataDictionary1 ProvinceInfo { get; set; }
        }


     
    }
}
