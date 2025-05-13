using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickHouseTest
{
    public class Unitsdfsasss
    {
        public static void Init()
        {
            var db = new SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = SqlSugar.DbType.ClickHouse,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuting = (sql, pars) => {
                var log = $"【{DateTime.Now}——执行SQL】\r\n{UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars)}\r\n";
                Console.WriteLine(log);
            };
 

            db.CodeFirst.InitTables<UnitUintxx1111>();
            db.Insertable(new   List<UnitUintxx1111>(){new UnitUintxx1111() { Wash_RGJson = new { id = 1 } } }).ExecuteCommand();
            var list= db.Queryable<UnitUintxx1111>().ToList();
        }
        public class UnitUintxx1111
        { 
            //public JObject _Wash_RGJson;

            /// <summary>

            /// 具体清洗规则

            /// </summary>

            [SugarColumn(ColumnDescription = "具体清洗规则", IsJson =true )]

            public object Wash_RGJson { get; set; }
        }
    }
}
