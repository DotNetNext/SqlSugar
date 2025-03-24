﻿using SqliteTest.UnitTest;
using SqlSugar;
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
            Unitasdfays.Init();
            Unitadfafafa.Init();
            UnitSubGroupadfa.Init();
            UnitAsyncToken.Init();
            UnitSplitadfa.Init();
            UnitWeek.Init();
            Unitafadsa.Init();
            UnitSplitTask.Init();
            UnitBulkMerge.Init();
            UnitBizDelete.Init();
            UnitBulkCopyUpdateaasfa.Init();
            UnitSubToList.Init();
            CrossDatabase02.Init();
            CrossDatabase03.Init();
            ULock.Init();
            UNavQuery01.Init();
            UCustom012.Init();
            Bulk();
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
