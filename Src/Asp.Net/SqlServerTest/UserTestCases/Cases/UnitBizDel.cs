using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UnitBizDel
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<SysLogOp>();
            db.Deleteable<SysLogOp>() 
            .Where(t => long.Parse(t.Id) > 0)
            .IsLogic()
            .ExecuteCommandAsync("Deleted", true, "UpdateTime", "UpdateUserId", "111")
            .GetAwaiter().GetResult();

            db.Deleteable<SysLogOp>()
          .Where(t => long.Parse(t.Id) > 0)
          .IsLogic()
          .ExecuteCommand ("Deleted", true, "UpdateTime", "UpdateUserId", "111")
   ;
        } 
        ///<summary>
        ///系统操作日志表
        ///</summary>
        public partial class SysLogOp
        {
            public SysLogOp()
            {


            }
            /// <summary>
            /// Desc:Id
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// Desc:请求方式
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string HttpMethod { get; set; }

            /// <summary>
            /// Desc:请求地址
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string RequestUrl { get; set; }

            /// <summary>
            /// Desc:请求参数
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string RequestParam { get; set; }

            /// <summary>
            /// Desc:返回结果
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string ReturnResult { get; set; }

            /// <summary>
            /// Desc:事件Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public int? EventId { get; set; }

            /// <summary>
            /// Desc:线程Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public int? ThreadId { get; set; }

            /// <summary>
            /// Desc:请求跟踪Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string TraceId { get; set; }

            /// <summary>
            /// Desc:异常信息
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Exception { get; set; }

            /// <summary>
            /// Desc:日志消息Json
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Message { get; set; }

            /// <summary>
            /// Desc:模块名称
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string ControllerName { get; set; }

            /// <summary>
            /// Desc:方法名称
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string ActionName { get; set; }

            /// <summary>
            /// Desc:显示名称
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string DisplayTitle { get; set; }

            /// <summary>
            /// Desc:执行状态
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Status { get; set; }

            /// <summary>
            /// Desc:IP地址
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string RemoteIp { get; set; }

            /// <summary>
            /// Desc:登录地点
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Location { get; set; }

            /// <summary>
            /// Desc:经度
            /// Default:
            /// Nullable:True
            /// </summary>           
            public double? Longitude { get; set; }

            /// <summary>
            /// Desc:维度
            /// Default:
            /// Nullable:True
            /// </summary>           
            public double? Latitude { get; set; }

            /// <summary>
            /// Desc:浏览器
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Browser { get; set; }

            /// <summary>
            /// Desc:操作系统
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Os { get; set; }

            /// <summary>
            /// Desc:操作用时
            /// Default:
            /// Nullable:True
            /// </summary>           
            public long? Elapsed { get; set; }

            /// <summary>
            /// Desc:日志时间
            /// Default:
            /// Nullable:True
            /// </summary>           
            public DateTime? LogDateTime { get; set; }



            /// <summary>
            /// Desc:账号
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string Account { get; set; }

            /// <summary>
            /// Desc:真实姓名
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string RealName { get; set; }

            /// <summary>
            /// Desc:租户Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public long? TenantId { get; set; }

            /// <summary>
            /// Desc:创建时间
            /// Default:
            /// Nullable:True
            /// </summary>           
            public DateTime? CreateTime { get; set; }

            /// <summary>
            /// Desc:更新时间
            /// Default:
            /// Nullable:True
            /// </summary>           
            public DateTime? UpdateTime { get; set; }

            /// <summary>
            /// Desc:创建者Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string CreateUserId { get; set; }

            /// <summary>
            /// Desc:修改者Id
            /// Default:
            /// Nullable:True
            /// </summary>           
            public string UpdateUserId { get; set; }

            /// <summary>
            /// Desc:软删除
            /// Default:0
            /// Nullable:False
            /// </summary>           
            public bool Deleted { get; set; }

            /// <summary>
            /// Desc:版本号
            /// Default:1
            /// Nullable:True
            /// </summary>           
            public int? Version { get; set; }

        }
    }
}
