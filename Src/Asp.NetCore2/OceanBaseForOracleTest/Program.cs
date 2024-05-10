using OrmTest;
using SqlSugar;
using SqlSugar.OceanBaseForOracle;
using System.Xml.Linq;
using xTPLM.RFQ.Model.XRFQ_APP;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;

namespace OceanBaseForOracle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("#### MasterSlave Start ####");

            //程序启动时加上只要执行一次
            InstanceFactory.CustomAssemblies =
                     new System.Reflection.Assembly[] { typeof(OceanBaseForOracleProvider).Assembly };


            //OceanBase Oracle 模式用这个 DbType.OceanBaseForOracle
            //OceanBase MySql 模式用DbType.MySql不要用这个
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.OceanBaseForOracle,//Oracle 模式用这个 ，如果是MySql 模式用DbType.MySql
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(db.Ado.Connection.ConnectionString);
            };
            Console.WriteLine("Master:");
            //db.Queryable<TRFQ_INSTRUCTIONS>().Where(m => m.REMARK==""&& m.PRICE_TYPE == 1 && m.PRICE >= 100 && m.PRICE_UPPER >= 100).ToList() ;
            //db.Insertable<Dto>(new Dto { }).ExecuteCommand();
            db.Insertable<TRFQ_INSTRUCTIONS>(new TRFQ_INSTRUCTIONS
            {
                I_NAME = "测试新增",
                END_TIME = DateTime.Now.Date,
                REMARK = "我是备注",
                CREATE_BY = 1,
                UPDATE_TIME = DateTime.Now,
                UPDATE_BY = 1,
                STATUS = 0,
                I_CODE = "090005",
                A_TYPE = "SPT_BD",
                M_TYPE = "X_CNBD",
                ORDER_MONEY = 500,
                I_TYPE = 1,
                PRICE_TYPE = 3,
                YTM = 0,
                YTM_OE = 0,
                PRICE = 100,
                NETPRICE = 0,
                TRADE_TYPE = "10",
                PARTY_ID = 1000,
                SECU_ACCID = "bss",
                ORDER_DATE = DateTime.Now,
                SET_DAYS = 0,
                SOURCE_TYPE = "xRFQ"
            }).ExecuteCommand();
            Console.WriteLine("#### MasterSlave End ####");
        }
    }

    public class Dto
    {
        public string PatID1 { get; set; }

        public string PatID3 { get; set; }

        public string PatID { get; set; }
    }
}