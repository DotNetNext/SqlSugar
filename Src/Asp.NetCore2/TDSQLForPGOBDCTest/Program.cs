using SqlSugar;
using SqlSugar.TDSQLForPGODBC;

namespace TDSQLForPGOBDCTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("#### MasterSlave Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.TDSQLForPGODBC,//Oracle 模式用这个 ，如果是MySql 模式用DbType.MySql
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings()
                {
                    PgSqlIsAutoToLower = false,//增删查改支持驼峰表
                    PgSqlIsAutoToLowerCodeFirst = false, // 建表建驼峰表。5.1.3.30 
                }
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
            };
            var list = db.Queryable<TSYS_USER>().ToList();

            var page1 = db.Queryable<TSYS_USER>().ToOffsetPage(1, 30);

            db.Insertable<TSYS_USER>(new TSYS_USER
            {
                U_ID = "DGSSqlsugar",
                U_NAME = "杜国舜SqlSugar适配",
                U_EMAIL = "dd2@"
            }).ExecuteCommand();

            db.Updateable<TSYS_USER>(new TSYS_USER
            {
                U_ID = "DGSSqlsugar",
                U_NAME = "杜国舜SqlSugar适配，修改后",
                U_EMAIL = "dd2@"
            }).ExecuteCommand();

            db.Deleteable<TSYS_USER>().Where(m => m.U_ID == "DGSSqlsugar").ExecuteCommand();

        }
    }
}
