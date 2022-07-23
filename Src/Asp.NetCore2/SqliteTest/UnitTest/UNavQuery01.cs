using SqlSugar;
using SqlugarDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UNavQuery01
    {
        public static void Init() 
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"DataSource=..\TestDB.sqlite",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
                ,AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });

            bool bDataHaveInited = false;

            if (!bDataHaveInited)
            {
                #region 创建表和初始化模拟数据
                db.Context.DbMaintenance.CreateDatabase();
                db.CodeFirst.InitTables<CustomerMainInfomation>();
                db.CodeFirst.InitTables<CustomerTranslatorBindingRelationship>();
                db.CodeFirst.InitTables<ERPTranslatorMainInfomation>();


                {
                    CustomerMainInfomation pm = new CustomerMainInfomation()
                    {
                        Id = 1,
                        RecordID = "DFBAFACE-5E9D-4BC5-84E4-5451C404D5CD",
                        CustomerName = "客户名字",
                        CustomerNo = "11090032"
                    };

                    db.Insertable(pm).ExecuteCommand();
                }
                long pdiId = 21;
                {
                    CustomerTranslatorBindingRelationship pdi = new CustomerTranslatorBindingRelationship()
                    {
                        Id = 1,
                        // Name = "PDI1 Name",
                        RecordID = "A224B2AA-927B-4071-8271-31FA32867DBE",
                        ParentID = "DFBAFACE-5E9D-4BC5-84E4-5451C404D5CD",
                        TranslatorID = "1405023",
                        Status = "有效"
                    };
                    db.Insertable(pdi).ExecuteCommand();
                }
                long rsId1 = 31;
                {
                    ERPTranslatorMainInfomation rr = new ERPTranslatorMainInfomation()
                    {
                        Id = 1,
                        //Name = "RR1 Name",
                        RecordID = "AA8E30BA-A6BE-44F9-A4CE-CAEDFCA6E3A8",
                        TranslatorID = "1405023"
                    };
                    db.Insertable(rr).ExecuteCommand();
                }
                #endregion
            }
            string strCustomerNo = "11090032";
            string strTranslatorID = "1405023";
            #region 不加过滤条件，正常
            {
                var vTranslatoreList =
                    db.Queryable<ERPTranslatorMainInfomation>()
                    .Includes(s => s.CustomerBindingRelationships.Where(s1 => s1.Status == "有效"
                        //&& (s1.CustomerInfo.CustomerNo == strCustomerNo)
                        ).ToList(),
                        s => s.CustomerInfo)
                    .Where(s => s.TranslatorID == strTranslatorID)
                    .ToList()
                    ;
            }
            #endregion

            #region 加过滤条件，报错
            {
                var list = db.Queryable<CustomerTranslatorBindingRelationship>()
       .Where(x => x.CustomerInfo.CustomerNo == "")
       .ToList();


                var vTranslatoreList =
                    db.Queryable<ERPTranslatorMainInfomation>()
                    .Includes(s => s.CustomerBindingRelationships.Where(s1 => s1.Status == "有效"
                        && (s1.CustomerInfo.CustomerNo == strCustomerNo)
                        ).ToList(),
                        s => s.CustomerInfo)
                    .Where(s => s.TranslatorID == strTranslatorID)
                    .ToList()
                    ;
            }
            #endregion
        }
    }
}
