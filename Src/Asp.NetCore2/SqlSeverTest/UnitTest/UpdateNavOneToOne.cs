using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;

namespace OrmTest
{
    internal class UpdateNavOneToOne
    {
    
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            if (!db.DbMaintenance.IsAnyTable("RosterCollection", false))
            {
                db.CodeFirst.InitTables<RosterCollection>();
            }
            if (!db.DbMaintenance.IsAnyTable("RosterBasicInfo", false))
            {
                db.CodeFirst.InitTables<RosterBasicInfo>();
            }


            //用例代码
            var id = db.Insertable<RosterCollection>(new RosterCollection() { TestText1 = "主表业务字段" }).ExecuteReturnIdentity();
            var roster = new RosterCollection()
            {
                RosterId = id,
                TestText1 = "新主表业务字段",
                BasicInfo = new RosterBasicInfo() { TestText2 = "从表业务字段", ToRosterId = id }
            };
            var result = db
                .UpdateNav<RosterCollection>(roster)
                .Include(c => c.BasicInfo).ExecuteCommand();//用例代码 

           

            db.Queryable<RosterCollection>()
                .Where(x => x.BasicInfo.BasicInfoId == 1)
                  .WhereIF(true,z => z.BasicInfo.BasicInfoId == 1)
                        .Where( z => z.BasicInfo.BasicInfoId == 1)
                .ToList();
        }

        //用例实体

        //主表
        public class RosterCollection
        {
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int RosterId { get; set; }

            /// <summary>
            /// 从表
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(RosterId), nameof(RosterBasicInfo.ToRosterId))]
            public RosterBasicInfo? BasicInfo { get; set; }

            public string TestText1 { get; set; }
        }

        //从表
        public class RosterBasicInfo
        {
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BasicInfoId { get; set; }

            /// <summary>
            /// 主表Id
            /// </summary>
            public int ToRosterId { get; set; }

            public string TestText2 { get; set; }
        }
    }
}
