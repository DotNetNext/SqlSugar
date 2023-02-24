using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DbType = SqlSugar.DbType;

namespace OrmTest
{
 
    public class UnitSubToList001  
    {
      
        public  static void Init()
        {

            var db = new SqlSugarScope(new List<ConnectionConfig>()
            {
                new ConnectionConfig(){ConfigId="A",DbType=DbType.MySql,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true},
                new ConnectionConfig(){ConfigId="B",DbType=DbType.MySql,ConnectionString=Config.ConnectionString2,IsAutoCloseConnection=true},
            });

            db.Aop.OnLogExecuting=db.GetConnectionScope("B").Aop.OnLogExecuting=(x,b)=>Console.WriteLine(x); ;

            db.DbMaintenance.CreateDatabase();
            db.GetConnectionScope("B").DbMaintenance.CreateDatabase();
            //建表 
            if (!db.GetConnectionScope("A").DbMaintenance.IsAnyTable("g_wo_base", false))
            {

                db.CodeFirst.InitTables<GWoBase>();
            }

            if (!db.GetConnectionScope("A").DbMaintenance.IsAnyTable("g_wo_param", false))
            {
                db.DbMaintenance.CreateDatabase();
                db.CodeFirst.InitTables<GWoParam>();
            }

            if (!db.GetConnectionScope("B").DbMaintenance.IsAnyTable("sys_org", false))
            {
                var childDb = db.GetConnectionScope("B");
                childDb.CodeFirst.InitTables<SysOrg>();
            }

            //种子数据
            if (db.QueryableWithAttr<GWoBase>().Any() == false)
            {
                db.InsertableWithAttr(new GWoBase() { Id = 100001, WorkOrder = "W001", OrgId = 1001 }).ExecuteCommand();
                db.InsertableWithAttr(new GWoBase() { Id = 100002, WorkOrder = "W002", OrgId = 1001 }).ExecuteCommand();
                db.InsertableWithAttr(new GWoParam() { WorkOrder = "W001", Param = "Test01", Values = "V1" }).ExecuteCommand();
                db.InsertableWithAttr(new GWoParam() { WorkOrder = "W001", Param = "Test02", Values = "V2" }).ExecuteCommand();
                db.InsertableWithAttr(new GWoParam() { WorkOrder = "W001", Param = "Test03", Values = "V3" }).ExecuteCommand();
                db.InsertableWithAttr(new SysOrg() { Id = 1001, OrgCode = "OrgTest" }).ExecuteCommand();
            }
 

            //测试用例 
           var list = db.Queryable<GWoBase>().AS<GWoBase>("zd_mes_wo.g_wo_base")
                .LeftJoin<SysOrg>((g, s) => g.OrgId == s.Id).AS<SysOrg>("zd_mes_bd.sys_org")
                .Where(g => g.WorkOrder == "W001")
                .Select((g, s) => new GWoBase
                {
                    Id = g.Id,
                    WorkOrder=g.WorkOrder,
                    WoParams = SqlFunc.Subqueryable<GWoParam>().Where(c => c.WorkOrder == g.WorkOrder).ToList()
                })
                .ToPageListAsync(1, 10).GetAwaiter().GetResult();

        }
    }

    [SugarTable("g_wo_base", "工单信息表")]
    [Tenant("A")]
    public class GWoBase
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn]
        public string WorkOrder { get; set; }

        [SugarColumn]
        public long OrgId { get; set; }


        [SugarColumn(IsIgnore = true)]
        public List<GWoParam> WoParams { get; set; }
    }

    [SugarTable("g_wo_param", "工单参数表")]
    [Tenant("A")]
    public class GWoParam
    {
        [SugarColumn]
        public string WorkOrder { get; set; }

        [SugarColumn]
        public string Param { get; set; }

        [SugarColumn]
        public string Values { get; set; }
    }

    [SugarTable("sys_org", "机构表")]
    [Tenant("B")]
    public class SysOrg
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn]
        public string OrgCode { get; set; }

    }

}

