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
            DbType = DbType.PostgreSQL,
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
            ULock.Init();
            UInsert3.Init();
            UnitSubToList.Init();
            UnitByteArray.Init();
            Unit001.Init();
            UnitPgSplit.Init();
            UJsonFunc.Init();
            UnitTestReturnPkList.Init();
            UCustom07.Init();
            UCustom016.Init();
            UCustom08.Init();
            UCustom012.Init();
            UCustom014.Init();
            UCustom015.Init();
            UCustom011.Init();
            UCustom01.Init();
            Save();
            CodeFirst();
            Updateable();
            Json();
            Ado();
            Queryable();
            QueryableAsync();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
