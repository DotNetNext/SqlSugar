using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.Unitadfa1231;

namespace OrmTest 
{
    public class Unitadfa1231
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;

            db.CurrentConnectionConfig.ConfigureExternalServices.EntityNameService = (x, p) => //处理列名
            {
                if (!p.DbTableName.Contains("_"))
                    p.DbTableName = UtilMethods.ToUnderLine(p.DbTableName);//ToUnderLine驼峰转下划线方法
            };
            db.CurrentConnectionConfig.ConfigureExternalServices.EntityService = (x, p) => //处理列名
            {
                if (!p.DbColumnName.Contains("_"))
                    p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName);//ToUnderLine驼峰转下划线方法
            };
            db.CodeFirst.InitTables<LawsRegulations, LawsStudyRole, SysUserRole>();
            var NeedStudyUserCountList = db.Queryable<LawsRegulations>()

                  .InnerJoin<LawsStudyRole>((a, b) => a.LawsStudyRuleId == b.LawsStudyRuleId)

                  .InnerJoin<SysUserRole>((a, b, c) => b.RoleId == c.RoleId)

                  .Where((a, b, c) => a.status == 1 && a.type == 1)

                  .Select((a, b, c) => new

                  {

                      rindex = SqlFunc.RowNumber($"{b.NeedStudyLength} desc ", $"{a.Id},{c.UserId}"),

                      Id = a.Id,

                      LawsStudyRuleId = a.LawsStudyRuleId,

                      UserId = c.UserId,

                      NeedStudyLength = b.NeedStudyLength

                  })

                  .MergeTable()

                  .Where(u => u.rindex == 1)

                  .GroupBy(u => u.LawsStudyRuleId)

                  .Select(u => new { LawsStudyRuleId = u.LawsStudyRuleId, NeedStudyUserCount = SqlFunc.AggregateCount(u.UserId) })

                  .ToList();
             
        }



        [SugarTable("laws_regulations", "法律法规表")]

        public class LawsRegulations : EntityBase

        {

            /// <summary>

            /// 分类 （0目录 1文件）

            /// </summary>

            public int type { get; set; }

            /// <summary>

            /// 父Id

            /// </summary>

            public long pid { get; set; }

            /// <summary>

            /// 名称

            /// </summary>

            public string title { get; set; }

            /// <summary>

            /// 状态（0失效 1正常）

            /// </summary>

            public int status { get; set; }

            /// <summary>

            /// 法规学习规则表Id

            /// </summary>

            public long LawsStudyRuleId { get; set; }

            /// <summary>

            /// 失效时间

            /// </summary>

            public DateTime? invalidTime { get; set; }

            /// <summary>

            /// （过期时间）失效时间

            /// </summary>

            public DateTime? expireTime { get; set; }

            /// <summary>

            /// 是否新法速递

            /// </summary>

            public Boolean IsNewLaw { get; set; }

            /// <summary>

            /// 是否系统设定，禁止修改

            /// </summary>

            public Boolean IsSystem { get; set; }

            /// <summary>

            /// 排序

            /// </summary>

            public int orderNo { get; set; }

            /// <summary>

            /// 备注

            /// </summary>

            public string  remark { get; set; }





         

        }



        [SugarTable("laws_study_role", "角色学习法规需要时长表")]

        public class LawsStudyRole : EntityBaseId

        {

            /// <summary>

            /// 法规学习规则表Id

            /// </summary>

            public long LawsStudyRuleId { get; set; }

            /// <summary>

            /// 角色Id

            /// </summary>

            public long RoleId { get; set; }

            /// <summary>

            /// 学习所需时长（分钟）

            /// </summary>

            public int NeedStudyLength { get; set; }





            [Navigate(NavigateType.OneToMany, nameof(SysUserRole.RoleId), nameof(RoleId))]

            public List<SysUserRole> SysUserRoleList { get; set; }

        }





        [SugarTable(null, "系统用户角色表")]

 

        public class SysUserRole : EntityBaseId

        {

            /// <summary>

            /// 用户Id

            /// </summary>

            [SugarColumn(ColumnDescription = "用户Id")]

            public long UserId { get; set; }



            /// <summary>

            /// 角色Id

            /// </summary>

            [SugarColumn(ColumnDescription = "角色Id")]

            public long RoleId { get; set; }



            /// <summary>

            /// 角色

          

        }


    }
}
