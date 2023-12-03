using Demo;
using SqlSeverTest.UserTestCases;
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
            DbType = DbType.SqlServer,
            ConnectionString = Config.ConnectionString,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            AopEvents = new AopEvents
            {
                OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(UtilMethods.GetNativeSql(sql,p));
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
            UnitGridSave2.Init();
            Unitdfafa11.Init();
            UnitSelectN.Init();
            UnitSubqueryN.Init();
            Unitadfasfa.Init();
            UnitOneToMany1231123.Init();
            OneToManyInString.Init();
            UnitSplitTask.Init();
            UinitUpdateNavOneToOne.Init();
            UnitCreateNavClass.Init();
            UnitBulkMerge.Init();
            UnitBool.Init();
            UnitGridSave.Init();
            UnitNavDynamic.Init();
            CrossDatabase01.Init();
            UnitStringToExp.Init();
            UnitOneToMany2.Init();
            UnitOneToMany.Init();
            UnitOneToOneDel.Init();
            EntityInfoTest.Init();
            UnitOneToManyafdaa.Init();
            Unitadfafa.Init();
            AnimalTest.Init();
            UnitOneToOneNAny.Init();
            Unitrasdfa.Init();
            Unitadfasdfa.Init();
            UpdateNavOneToOne.Init();
            Unitasf1.Init();
            UOneManyMany7.init();
            UOneManyMany6.init();
            UinitCustomConvert.Init();
            UnitCustom020.Init();
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
