﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
       public static  SqlSugarClient Db=> new SqlSugarClient(new ConnectionConfig()
        {
            DbType = DbType.Sqlite,
            ConnectionString = Config.ConnectionString,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            AopEvents = new AopEvents
            {
                OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                }
            }
        });

        public static void RestData()
        {
            Db.DbMaintenance.TruncateTable<Order>();
            Db.DbMaintenance.TruncateTable<OrderItem>();
        }
        public static void Init()
        {
            UinitCustomConvert.Init();
            UnitNavUpdatee12.Init();
            UnitFilterdafa.Init();
            UInsert3.Init();
            USaveable.Init();
            UnitSubToList.Init();
            UnitByteArray.Init();
            Unit01.Init();
            UnitNavInsertadfa1.Init();
            UnitNavInsertIssue.Init();
            UnitInsertNavN.Init();
            UNavTest.Init();
            UnitTestReturnPkList.Init();
            UCustom01.Init();
            UCustom011.Init();
            UBulkCopy.Init();
            Bulk();
            Insert();
            CodeFirst();
            Updateable();
            Json();
            Ado();
            Queryable();
            QueryableAsync();
            Thread();
            Thread2();
            Thread3();
        }
    }
}
