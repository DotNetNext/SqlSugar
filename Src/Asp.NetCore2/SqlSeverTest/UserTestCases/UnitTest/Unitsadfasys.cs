using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitsadfasys
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db; 
            var data=db.Deleteable(
            new UnitDeletedsfsa() { A = "a" }
            ).ToSql(); 
            if (!data.Key.Contains("N'")) throw new Exception("unit error");

            var data2 = db.Deleteable(
             new UnitDeletedsfsa2() { A = "a" }
            ).ToSql();
            if (data2.Key.Contains("N'")) throw new Exception("unit error");
        }
        public class UnitDeletedsfsa 
        {
            [SqlSugar.SugarColumn(SqlParameterDbType =System.Data.DbType.String,IsPrimaryKey =true)]
            public string A { get; set; }
        }
        public class UnitDeletedsfsa2
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public string A { get; set; }
        }
    }
}
