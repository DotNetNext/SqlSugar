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
            DbType = DbType.SqlServer,
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
            UnitSubToList.Init();
            UJsonsdafa.Init();
            UOneManyMany.init();
            UOneManyMany2.init();
            UOneManyMany3.init();
            UOneManyMany4.init();
            UOneManyMany5.init();
            UNavDynamic111N.Init();
            UCustom019.Init();
            UCustom015.Init();
            UCustom014.Init();
            UCustom012.Init();
            UCustom01.Init();
            UCustom02.Init();
            UCustom03.Init();
            Bulk();
            Filter();
            Insert();
            Enum();
            Tran();
            Queue();
            CodeFirst();
            Updateable();
            Json();
            Ado();
            Queryable();
            Queryable2();
            QueryableAsync();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
