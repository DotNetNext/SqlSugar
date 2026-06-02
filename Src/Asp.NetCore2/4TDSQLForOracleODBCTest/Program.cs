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
            var date = DateTime.Now;
            //查询
            var list = db.Queryable<TRFQ_QUOTATION>().Where(m => m.ORDER_DATE.Value.Date <= date.Date).ToList();


            var list2 = db.Queryable<TRFQ_QUOTATION>().Where(m => m.ORDER_DATE.Value.Date <= date.Date).ToOffsetPage(1, 50);
            var item = new TRFQ_QUOTATION()
            {
                ORDER_DATE = DateTime.Now,
                Q_STATUS = 0,
            };

            var item2 = new TRFQ_QUOTATION()
            {
                ORDER_DATE = DateTime.Now,
                Q_STATUS = 0,
            };

            var item3 = new TRFQ_QUOTATION()
            {
                ORDER_DATE = DateTime.Now,
                Q_STATUS = 0,
            };
            var insertList = new List<TRFQ_QUOTATION>() { item, item2, item3 };
            //新增
            var newid = db.Insertable<TRFQ_QUOTATION>(insertList).ExecuteReturnIdentity();
            item.Q_SYSID = newid;

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
            ////修改
            //db.Updateable<TRFQ_QUOTATION>(updateList).UpdateColumns(["Q_STATUS"]).ExecuteCommand();
        }
    }
}
