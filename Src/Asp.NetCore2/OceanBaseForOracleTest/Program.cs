using OrmTest;
using SqlSugar;
using SqlSugar.OceanBaseForOracle;
using System.Xml.Linq;
using xTPLM.RFQ.Common.Enum;
using xTPLM.RFQ.Model.XMANAGER_APP;
using xTPLM.RFQ.Model.XRFQ_APP;

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
            DateTime NextTime = DateTime.Now;
            DateTime endTime = DateTime.Now;
            db.Updateable<TRFQ_SYS_TASK>().SetColumns(m => new TRFQ_SYS_TASK
            {
                NEXT_FIRE_TIME = NextTime,
                PREVIOUS_FIRE_TIME = endTime,
                COUNT = m.COUNT + 1
            }).Where(n => n.ID == 1).ExecuteCommand();
            //db.Queryable<TRFQ_INSTRUCTIONS>().Where(m => m.REMARK==""&& m.PRICE_TYPE == 1 && m.PRICE >= 100 && m.PRICE_UPPER >= 100).ToList() ;
            //db.Insertable<Dto>(new Dto { }).ExecuteCommand();
            //db.Insertable<TRFQ_INSTRUCTIONS>(new TRFQ_INSTRUCTIONS
            //{
            //    A_TYPE= "SPT_BD",
            //    CREATE_BY=1,
            //    END_TIME=DateTime.Now.Date,
            //    I_CODE="090005",
            //    I_NAME= "09附息国债05",
            //    I_TYPE= InstructionsType.Accurate,
            //    M_TYPE= "X_CNBD",
            //    NETPRICE= 100M,
            //    ORDER_DATE= DateTime.Now.Date,
            //    PRICE= 100.3405M,
            //    ORDER_MONEY= 100M,
            //    PRICE_TYPE=xTPLM.RFQ.Common.PriceTypeEnum.NETPRICE,
            //    SECU_ACCID= "bss_in_secu_02",
            //    SET_DAYS=xTPLM.RFQ.Common.SetDays.T0,
            //    SOURCE_TYPE= "xRFQ",
            //    STATUS=InstructionsStatus.Create,
            //    TRADE_TYPE=TradeType.Buy,
            //    YTM= 4.0197M
            //}).ExecuteCommand();
            int Count = 0;
            var list = db.Queryable<TRFQ_INSTRUCTIONS>()
                .LeftJoin<TRFQ_BND>((t, t1) => t.I_CODE == t1.I_CODE && t.A_TYPE == t1.A_TYPE && t.M_TYPE == t1.M_TYPE)
                .Where(t => t.I_CODE != null)
                .SelectMergeTable((t, t1) => new TRFQ_INSTRUCTIONS
                {
                    I_ID = t.I_ID,
                    A_TYPE = t.A_TYPE,
                    CREATE_BY = t.CREATE_BY,
                    END_TIME = t.END_TIME,
                    I_CODE = t.I_CODE,
                    I_NAME = t.I_NAME,
                    IS_CITY_INVESTMENT = t.IS_CITY_INVESTMENT,
                    IS_RATES = t.IS_RATES,
                    M_TYPE = t.M_TYPE,
                    ORDER_MONEY = t.ORDER_MONEY,
                    PARTY_ID = t.PARTY_ID,
                    PERPETUAL = t.PERPETUAL,
                    PRICE_TYPE = t.PRICE_TYPE,
                    REMARK = t.REMARK,
                    STATUS = t.STATUS,
                    TRADE_TYPE = t.TRADE_TYPE,
                    UPDATE_BY = t.UPDATE_BY,
                    UPDATE_TIME = t.UPDATE_TIME,
                    ORDER_DATE_MIN = t.ORDER_DATE_MIN,
                    ORDER_DATE_MAX = t.ORDER_DATE_MAX,
                    SECU_ACCID = t.SECU_ACCID,
                    SYSID_EXT = t.SYSID_EXT,
                    MARKET = t.MARKET,
                    PARTY_NAME = SqlFunc.Subqueryable<TRFQ_COUNTERPARTY>().Where(m => m.P_SYSID == t.PARTY_ID).Select(m => m.PARTYNAME_SHORT),
                    SUBMIT_IRUSER = t.SUBMIT_IRUSER,
                    P_CLASS = t.P_CLASS,
                    LAST_TERM = t.LAST_TERM,
                    LAST_TERM_TYPE = t.LAST_TERM_TYPE,
                    MODIFIED_D = t.MODIFIED_D,
                    SUBMIT_MESSAGE = t.SUBMIT_MESSAGE,
                    SET_DAYS = t.SET_DAYS,
                    ORDER_DATE = t.ORDER_DATE,
                    I_TYPE = t.I_TYPE,
                    I_NO = t.I_NO,
                    PRICE = t.PRICE,
                    NETPRICE = t.NETPRICE,
                    YTM = t.YTM,
                    YTM_OE = t.YTM_OE,
                    PRICE_UPPER = t.PRICE_UPPER,
                    NETPRICE_UPPER = t.NETPRICE_UPPER,
                    YTM_UPPER = t.YTM_UPPER,
                    YTM_OE_UPPER = t.YTM_OE_UPPER,
                    CREATE_BY_NAME = SqlFunc.Subqueryable<TMANAGER_SYS_USER>().AS($"XMANAGER_APP.TMANAGER_SYS_USER").Where(m => m.U_ID == t.CREATE_BY).Select(m => m.U_NICKNAME ?? m.U_NAME),
                    SHG_AGREENUM = t.SHG_AGREENUM,
                    SHG_TRADER_CP = t.SHG_TRADER_CP,
                    SHG_SEATNO_CP = t.SHG_SEATNO_CP,
                    SECU_ACCNAME = SqlFunc.Subqueryable<TRFQ_ACC_SECU>().Where(m => m.ACCID == t.SECU_ACCID).Select(m => m.ACCNAME),
                    ISSUER = t1.ISSUER,
                    B_NAME = t1.B_NAME,
                    SUBMIT_TIME = t.SUBMIT_TIME,
                    SOURCE_TYPE = t.SOURCE_TYPE,
                    CREATE_TIME = t.CREATE_TIME,
                    EXT_TRADE_ID = t.EXT_TRADE_ID,
                    CANCEL_STATUS = t.CANCEL_STATUS,
                    CASH_ACCID = t.CASH_ACCID,
                }).GroupBy(t => new { t.I_CODE, t.A_TYPE, t.M_TYPE, t.B_NAME })
                .SelectMergeTable(t => new InsOrderList
                {
                    I_CODE = t.I_CODE,
                    A_TYPE = t.A_TYPE,
                    M_TYPE = t.M_TYPE,
                    B_NAME = t.B_NAME,
                    MinId = SqlFunc.AggregateMin(t.I_ID)
                }).OrderByDescending(t=>t.MinId).ToOffsetPage(2, 30, ref Count);
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