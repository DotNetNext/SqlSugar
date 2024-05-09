using SqlSugar;
using System.Linq.Expressions;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmTest
{
    public class UnitInsertNav
    {

       public  static  void  Init()
        {
            //生成 DI 容器

            var context = NewUnitTest.Db; ;
            context.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                if (entityInfo.PropertyName == "Iden" && entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    entityInfo.SetValue(Guid.NewGuid().ToString("N").ToUpper());
                }
            };
           
            //建表 
            if (!context.DbMaintenance.IsAnyTable("MENU_INFO_1", false))
            {
                context.CodeFirst.InitTables<MenuInfo>();
            }
            //建表 
            if (!context.DbMaintenance.IsAnyTable("ACTIVITY_RULE_1", false))
            {
                context.CodeFirst.InitTables<ActivityRule>();
            }
            context.DbMaintenance.TruncateTable<MenuInfo,ActivityRule>();
            var result =   context.Insertable(new ActivityRule()
            {
                MenuIden = "1",
                RuleContent = "1"
            }).ExecuteReturnEntity ();

            MenuInfo menuInfo = new MenuInfo
            {
                MenuCode = "2",
                ActivityRules = new List<ActivityRule> { new ActivityRule() { Iden = result.Iden, RuleContent = "2" } }
            };
            Console.WriteLine("begin insert nav");
              context.InsertNav(menuInfo)
                .Include(s => s.ActivityRules)
                .ExecuteCommand ();

            var list = context.Queryable<MenuInfo>()
               .Includes(x => x.ActivityRules)
               .ToList();

            if (list.First().ActivityRules.Count() == 0) 
            {
                throw new Exception("unit error");
            }

        }


        ///<summary>
        ///
        ///</summary>
        [SugarTable("MENU_INFO_1")]
        public partial class MenuInfo
        {
            public MenuInfo()
            {

            }
            /// <summary>
            /// Desc:唯一编号
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN")]
            public string Iden { get; set; }

            /// <summary>
            /// Desc:菜单编号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "MENU_CODE")]
            public string MenuCode { get; set; }

            [SugarColumn(IsIgnore = true)]
            [Navigate(NavigateType.OneToMany, nameof(ActivityRule.MenuIden))]
            public List<ActivityRule> ActivityRules { get; set; }

        }
        ///<summary>
        ///组织信息
        ///</summary>
        [SugarTable("ACTIVITY_RULE_1")]
        public partial class ActivityRule
        {
            public ActivityRule()
            {
            }
            /// <summary>
            /// uuid
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, ColumnName = "IDEN")]
            public string Iden { get; set; }

            /// <summary>
            /// 菜单IDEN
            /// </summary>           
            [SugarColumn(ColumnName = "MENU_IDEN")]
            public string MenuIden { get; set; }
            /// <summary>
            /// uuid
            /// </summary>           
            [SugarColumn(ColumnName = "RUlE_CONTENT")]
            public string RuleContent { get; set; }
        }
    }
}
