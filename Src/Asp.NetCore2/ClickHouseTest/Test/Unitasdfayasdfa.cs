using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickHouseTest
{
    public class Unitasdfayasdfa
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

            db.CodeFirst.InitTables<UnitUint>();
            db.Insertable(new UnitUint() { id = 11 }).ExecuteCommand();
            var lit=db.Queryable<UnitUint>().ToList();
        }
        public class UnitUint 
        {
            [SqlSugar.SugarColumn(ColumnDataType = "uint32")]
            public uint id { get; set; }
        }
    }
}
