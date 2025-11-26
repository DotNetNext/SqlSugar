using Demo;
using SqlSeverTest;
using SqlSeverTest.UserTestCases;
using SqlSeverTest.UserTestCases.UnitTest;
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
            Unitsadsfasdfys.Init();
            Unitsdfyasfs3lsss.Init();
            Unitadsfasyss.Init();
            Unitsadfasysss.Init();
            Unitadfafassys.Init();
            UnitArrayNavQuery.Init();
            Unitdfasdfasfysa.Init();
            UnitDefaultValueTest.Init();
            Unitsdfayderqys.Init();
            Unitsdfaysrs.Init();
            Unitdslasdgy.Init();
            Unitdafaaaaa.Init();
            Unitdfafaa.Init();
            Unitdasfyasdfa.Init();
            Unitafdssfasydsfsf.Init();
            Unitsdfadysdfa.Init();
            Unitadfasdysdfa.Init();
            Unitdfayssf.Init();
            Unitdsfasdfys.Init();
            Unitsadfasys.Init();
            Unitsadfasys.Init();
            Unitadfasfysdfyss.Init();
            Unitsfasdyd.Init();
            Unitafdsafsss.Init();
            Unitdfdaysss.Init();
            Unitdsfsssysf.Init();
            Unitsdfadysssdf.Init();
            UnitOneToMany123131.Init();
            Unitadfasdys.Init();
            Unitadfadfadfa.Init();
            Unitsdfadsfsys.Init();
            Unitadfasdysss.Init();
            Unitdfafassfa.Init();
            Unitasdfays.Init();
            Unitsadfadsayss.Init();
            Unitadfsa1ysfds.Init();
            Unitdsadfays.Init();
            UnitDADF231YAA.Init();
            Unitadfasyya.Init();
            Unitysadfay2.Init();
            Unitdfaatsd2.Init();
            Unityadfasasdfa.Init();
            Unitsdfaafa.Init();
            Unitadfayyadfa.Init();
            Unitsdfa1231.Init();
            Unitasxdfaaa.Init();
            UnitSplitadfaf1.Init();
            Unitaadfas1.Init();
            Unitadfasda.Init();
            Unita3affafa.Init();
            UnitEnumTest.Init();
            Unitadf1yaadfa.Init();
            CrossDatabase03.Init();
            CrossDatabase02.Init();
            UnitDynamicCoread12321.Init();
            UnitManyToManyadfafa.Init();
            UnitAsyncToken.Init();
            UnitManyToMany121231.Init();
            UnitFilteradfa.Init();
            UnitWeek.Init();
            Unitadfaasdfaaa.Init();
            Unitatadffaa1.Init();
            UnitOneToManyNsdfafa.Init();
            UCustomConditionalFunc.Init();
            UnitSelectNASFDADSFA.Init();
            UnitOneToOneFilter.Init();
            UnitOneToManyNsdfafa.Init();
            UnitTreaaafasa.Init();
            UnitaadfafxSubToList.Init();
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
            UConfigSettings.Init();
            //UnitUtilConvert.Init(); pull  unit test is  error
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
            SecurityParameterHandling();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
