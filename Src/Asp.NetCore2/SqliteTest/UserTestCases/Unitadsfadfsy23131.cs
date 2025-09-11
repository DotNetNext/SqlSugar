using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadsfadfsy23131
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            SugarParameter [] ps = null;
            db.Aop.OnLogExecuting = (sql, p) =>
            {

                Console.WriteLine(UtilMethods.GetNativeSql(sql, p));
                ps = p;
            };
            db.CodeFirst.InitTables<Unitdafadfa>();
            db.Storageable(new Unitdafadfa() {  no2="", no1="",xx=""})
                .WhereColumns(it => new { it.no1, it.no2 })
                .ExecuteCommand();
            if (ps.Select(it => it.DbType+"").Distinct().Count() != 3) 
            {
                throw new Exception("unit error");
            }
        }
        public class Unitdafadfa 
        {
            [SqlSugar.SugarColumn(SqlParameterDbType =System.Data.DbType.AnsiStringFixedLength)]
            public string no1 { get; set; }
            [SqlSugar.SugarColumn(SqlParameterDbType = System.Data.DbType.StringFixedLength)]
            public string no2 { get; set; }
            public string xx { get; set; }
        }
    }
}
