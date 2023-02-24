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
            DbType = DbType.MySql,
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
            Unitadsfasf1.Init();
            UnitSubToList001.Init();
            UInsert3.Init();
            UnitSubToList.Init();
            UCustom20.Init();
            UCustom07.Init();
            UnitTestReturnPkList.Init();
            UnitSameKeyBug.Init();
            UOneManyMany.init();
            UDelete.Init();
            UCustom012.Init();
            UCustom014.Init();
            UCustom015.Init();
            UCustom011.Init();
            UnitCustom01.Init();
            UCustom06.Init();
            Bulk();
            Bulk2();
            Insert();
            Queue();
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
