using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTPLM.RFQ.Model.XRFQ_APP;

namespace TDSQLForOracleODBCTest
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
                DbType = DbType.TDSQLForOracleODBC,//Oracle 模式用这个 ，如果是MySql 模式用DbType.MySql
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
            });
            //var date = DateTime.Now;
            ////查询
            //var list = db.Queryable<TRFQ_QUOTATION>().Where(m => m.ORDER_DATE.Value.Date <= date.Date).ToList();


            //var list2 = db.Queryable<TRFQ_QUOTATION>().Where(m => m.ORDER_DATE.Value.Date <= date.Date).ToOffsetPage(1, 50);
            //var item = new TRFQ_QUOTATION()
            //{
            //    ORDER_DATE = DateTime.Now,
            //    Q_STATUS = 0,
            //};

            //var item2 = new TRFQ_QUOTATION()
            //{
            //    ORDER_DATE = DateTime.Now,
            //    Q_STATUS = 0,
            //};

            //var item3 = new TRFQ_QUOTATION()
            //{
            //    ORDER_DATE = DateTime.Now,
            //    Q_STATUS = 0,
            //};
            //var insertList = new List<TRFQ_QUOTATION>() { item, item2, item3 };
            ////新增
            //var newid = db.Insertable<TRFQ_QUOTATION>(insertList).ExecuteReturnIdentity();
            //item.Q_SYSID = newid;

            ////修改
            //db.Updateable<TRFQ_QUOTATION>().SetColumns(it => new TRFQ_QUOTATION()
            //{
            //    Q_STATUS = 1
            //}).Where(m => m.Q_SYSID == newid).ExecuteCommand();

            //item.Q_STATUS = 2;
            ////修改
            //db.Updateable<TRFQ_QUOTATION>(item).UpdateColumns(["Q_STATUS"]).ExecuteCommand();

            //item.Q_STATUS = 3;
            //List<TRFQ_QUOTATION> updateList = [item];
            //updateList.Add(new TRFQ_QUOTATION
            //{
            //    Q_SYSID = 15739,
            //    Q_STATUS = 4
            //});

            List<TRFQ_COUNTERPARTY> updateList = [];
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 85583,
                PARTYNAME = "北京证券公司",
                PARTYNAME_SHORT = "北京证券公司",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 106742,
                PARTYNAME = "财通基金丰悦稳健6号单一资产管理计划",
                PARTYNAME_SHORT = "财通基金丰悦稳健6号单一",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 132460,
                PARTYNAME = "汇添富全天候策略1号集合资产管理计划",
                PARTYNAME_SHORT = "汇添富全天候策略1号集合",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            updateList.Add(new TRFQ_COUNTERPARTY
            {
                P_SYSID = 83442,
                PARTYNAME = "中国银行股份有限公司",
                PARTYNAME_SHORT = "中国银行",
            });
            //修改
            db.Updateable<TRFQ_COUNTERPARTY>(updateList).UpdateColumns(["PARTYNAME", "PARTYNAME_SHORT"]).ExecuteCommand();
        }
    }
}
