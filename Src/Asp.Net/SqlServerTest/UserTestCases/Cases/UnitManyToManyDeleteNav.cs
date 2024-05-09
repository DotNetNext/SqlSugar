 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitManyToManyDeleteNav 
    {

        public static void Init()
        {
            var db = NewUnitTest.Db;
            string businessKey = "";
            //建表 
            if (!db.DbMaintenance.IsAnyTable("sys_schedule", false))
            {
                db.CodeFirst.InitTables<ScheduleEntity>();
                db.CodeFirst.InitTables<Schedule_SysUser_Mapping>();
                db.CodeFirst.InitTables<SysUserEntity>();
            }
            //var userId = Core.App.CurrentUserFunc().UserId;
            //var schIds = baseQueryable.Where(e => e.BusinessKey == businessKey).ToList()?.Select(it => it.Id).ToArray();
            //if (schIds == null) return;
            //Db.Deleteable<Schedule_SysUser_Mapping>(x => schIds.Contains(x.ScheduleId) && x.SysUserId == userId).ExecuteCommand();
            //只删除关系
            db.DbMaintenance.TruncateTable<ScheduleEntity>();
            db.DbMaintenance.TruncateTable<Schedule_SysUser_Mapping>();
            db.DbMaintenance.TruncateTable<SysUserEntity>();
            db.Insertable(new SysUserEntity()
            {
                 Id=1,
                  userName="A"
            }).ExecuteCommand();
            db.Insertable(new ScheduleEntity()
            {
                Id = 1,
                  Route = "A"
            }).ExecuteCommand();
            db.Insertable(new Schedule_SysUser_Mapping()
            {
                 ScheduleId=1,
                  SysUserId=1
            }).ExecuteCommand();
            db.MappingTables = new MappingTableList();
            db.MappingColumns = new MappingColumnList();
            db.DeleteNav<ScheduleEntity>(x => true)
                            .Include(x => x.SysUsers.Where(u => u.Id == 1).ToList()) // B表
                            .ExecuteCommand();

        
        }

    }
    /// <summary>
    /// 创 建：
    /// 日 期：2022/8/24 16:03:03
    /// 描 述：待办事项
    ///</summary>
   
    [SugarTable("sys_schedule11")]
    public class ScheduleEntity
    {
 
        [SugarColumn(IsPrimaryKey = true, ColumnName = "id")]
        public long Id { get; set; }
        /// <summary>
	    /// 0：未读，1：进行中，2：完成
        ///</summary>
        [SugarColumn(ColumnDescription = "状态", ColumnName = "status", IsNullable = true )]
        public int? Status { get; set; }


        /// <summary>
	    /// 摘要()
        ///</summary>
        [SugarColumn(ColumnDescription = "摘要", ColumnName = "summary", IsNullable = true, Length = 255)]
        public string Summary { get; set; }


        /// <summary>
	    /// 内容()
        ///</summary>
        [SugarColumn(ColumnDescription = "内容", ColumnName = "content", IsNullable = true, Length = 255)]
        public string Content { get; set; }


        /// <summary>
	    /// 路由()
        ///</summary>
        [SugarColumn(ColumnDescription = "路由", ColumnName = "route", IsNullable = true, Length = 255)]
        public string Route { get; set; }



        /// <summary>
        /// 路由参数()
        ///</summary>
        [SugarColumn(ColumnDescription = "路由参数", ColumnDataType = "text", ColumnName = "routingParameters", IsNullable = true )]
        public string RoutingParameters { get; set; }


        /// <summary>
        /// 业务Key()
        ///</summary>
        [SugarColumn(ColumnDescription = "业务Key", ColumnName = "businessKey", IsNullable = true, Length = 255)]
        public string BusinessKey { get; set; }

        /// <summary>
	    /// 系统用户
        ///</summary>
        [Navigate(typeof(Schedule_SysUser_Mapping), nameof(Schedule_SysUser_Mapping.ScheduleId), nameof(Schedule_SysUser_Mapping.SysUserId))]//注意顺序
        public List<SysUserEntity> SysUsers { get; set; }

    }
    /// <summary>
    /// 创 建：
    /// 日 期：2022/8/24 16:03:10
    /// 描 述：待办事项对多SysUser
    ///</summary>
    [Tenant("0")]
    [SugarTable("schedule_sysuser_mapping11")]
    public class Schedule_SysUser_Mapping
    {

        [SugarColumn(IsPrimaryKey = true, ColumnName = "ScheduleId")]
        public long ScheduleId { get; set; }


        [SugarColumn(IsPrimaryKey = true, ColumnName = "SysUserId")]
        public long SysUserId { get; set; }
    }
    /// <summary>
    /// 创 建：
    /// 日 期：2022/8/24 8:56:38
    /// 描 述：系统用户
    ///</summary>
    [Tenant("0")]
    [SugarTable("sys_user11")]
    public class SysUserEntity
    {
        /// <summary>
	    /// 用户账号()
        ///</summary>
        [SugarColumn(ColumnDescription = "用户账号", ColumnName = "userName", IsNullable = false, Length = 30)]
        public string userName { get; set; }



        /// <summary>
	    /// 用户 userId
        ///</summary>
        [SugarColumn(ColumnDescription = "用户ID", ColumnName = "userId", IsNullable = false, IsPrimaryKey = true )]

        public long Id { get; set; }



         


    }
}
