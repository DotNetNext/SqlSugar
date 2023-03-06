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
            UinitCustomConvert.Init();
            UnitFilterasdfas.Init();
            Unitadfa1231.Init();
            UInsert3.Init();
            USelectTest.Init();
            UnitSubToList.Init();
            UnitByteArray.Init();
            UnitInsertNavOneToOne.Init();
            CrossDatabase01.Init();
            CrossDatabase02.Init();
            CrossDatabase03.Init();
            UInsertNav0adsf.Init();
            UExp.Init();
            USelectSinleDTO.Init();
            UCustom024.Init();
            UCustom24.Init();
            UCustom20.Init();
            UTran2.Init();
            UnitUpdateSubQuery.Init();
            UnitManyToManyDeleteNav.Init();
            UnitTestReturnPkList.Init();
            UnitCustom12312.Init();
            UnitEnum22.Init();
            UCustom025.Init();
            UnitTestConfigQuery.Init();
            UnitSub.Init();
            UnitUpdateNavN3.Init();
            UnitOneToOne12.Init();
            UnitInsertNav3.Init();
            UnitInsertNav2.Init();
            UnitInsertNav.Init();
            UnitInsertNav.Init();
            UnitSelectN.Init();
            UnitOneToOneN2.Init();
            UnitManyToManyUpdate.Init();
            UnitManyToMay1231.Init();
            UnitUpdateNav2.Init();
            UnitUpdateNav.Init();
            UnitOneToOneN.Init();
            ULock.Init();
            UnitManyToMany2.Init();
            UOneManyMany5.init();
            UOneManyMany4.init();
            UOneManyMany3.init();
            UOneManyMany2.init();
            UOneManyMany.init();
            UNavDynamic111N.Init();
            UCustomNavigate01.Init();
            UCustom023.Init();
            UCustom22.Init();
            UByteArray.Init();
            UCustom021.Inti();
            UCustom020.Init();
            UCustom019.Init();
            UnitManyToMany.Init();
            UCustom018.Init();
            UCustom017.Init();
            UCustom016.Init();
            UCustom015.Init();
            UCustom014.Init();
            UCustom013.Init();
            UCustom012.Init();
            UintDynamic.Init();
            UCustom09.Init();
            UCustom011.Init();
            UCustom010.Init();
            UCustom08.Init();
            UCustom07.Init();
            UCustom01.Init();
            UCustom02.Init();
            UCustom03.Init();
            UCustom04.Init();
            UCustom05.Init();
            UCustom06.Init();
            SubQueryTest();
            UConfig();
            DeleteTest();
            Fastest2();
            SplitTest();
            Filter();
            Insert();
            Insert2();
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
            AopTest();
            UTran3.Init();
            UnitSqlFunc.Init();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
