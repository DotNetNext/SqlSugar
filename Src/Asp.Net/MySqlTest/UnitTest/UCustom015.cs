using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom015
    {

        public static void Init()
        {
            var db =   NewUnitTest.Db;
            db.DbMaintenance.CreateDatabase();
        
            db.CodeFirst.InitTables<Country1111>();
            db.CodeFirst.InitTables<Province1111>();
            db.CodeFirst.InitTables<Country111Info>();
            db.DbMaintenance.TruncateTable("Country_1111");
            db.DbMaintenance.TruncateTable("Province_1111");
            db.DbMaintenance.TruncateTable("Country111Info");
            var c = new Country1111()
            {
                 Id=1,
                  Name="中国",InfoId=1
            };
            var ps =  new List<Province1111>(){
                            new Province1111{
                                 Id=1001,
                                 Name="江苏", CountryId=1
                                 
                            },
                           new Province1111{
                                 Id=1002,
                                 Name="上海",  CountryId=1
                                  
                            },
                           new Province1111{
                                 Id=1003,
                                 Name="北京", CountryId=1
                                  
                            }
                       };

            db.Insertable(c).ExecuteCommand();
            db.Insertable(ps).ExecuteCommand();
            db.Insertable(new Country111Info {  Id=1, Name="infoa"}).ExecuteCommand();
            db.Aop.OnLogExecuted = (sq, p) =>
            {
                Console.WriteLine(sq);
            };

            var list = db.Queryable<Country1111>()
            .Includes(x => x.Info)
            .ToList();
            var list2 = db.Queryable<Country1111>()
              .Includes(x => x.Provinces.OrderByDescending(x111 => x111.Id).ToList())
              .ToList();
            db.CodeFirst.InitTables<SysTimer>();
            db.DbMaintenance.TruncateTable<SysTimer>();
            db.Insertable(new SysTimer()
            {
                 CreatedTime=DateTime.Now,
                  CreatedUserId=1,
                   CreatedUserName=DateTime.Now.ToString(),
                    Cron="",
                     DoOnce=1, 
                       Headers="",
                       ExecuteType=1,
                        Interval=1,
                         IsDeleted=1,
                        RequestParameters="",
                         StartNow=1,
                          JobName="a",
                           Remark="a",
                            RequestType=1,
                             RequestUrl="a",
                              TimerType=1,
                               UpdatedTime=DateTime.Now,
                                UpdatedUserId=1,
                                 UpdatedUserName="admin"
                                
            }).ExecuteCommand();
            var list3=db.Queryable<SysTimer>().Select<LocalJobOutput>().ToList();
        }
        /// <summary>
        /// 本地任务信息
        /// </summary>
        public class LocalJobOutput
        {
            /// <summary>
            /// 任务名称
            /// </summary>
            public string JobName { get; set; }

            /// <summary>
            /// 只执行一次
            /// </summary>
            public bool DoOnce { get; set; } = false;

            /// <summary>
            /// 立即执行（默认等待启动）
            /// </summary>
            public bool StartNow { get; set; } = false;

            /// <summary>
            /// 执行间隔时间（单位秒）
            /// </summary>
            public int Interval { get; set; }

            /// <summary>
            /// Cron表达式
            /// </summary>
            public string Cron { get; set; }

            /// <summary>
            /// 请求url
            /// </summary>
            public string RequestUrl { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }
        }
        /// <summary>
        /// 定时任务表
        ///</summary>
        [SugarTable("sys_timer")]
        public class SysTimer
        {
            /// <summary>
            /// Id主键 
            ///</summary>
            [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }
            /// <summary>
            /// 任务名称 
            ///</summary>
            [SugarColumn(ColumnName = "JobName")]
            public string JobName { get; set; }
            /// <summary>
            /// 只执行一次 
            ///</summary>
            [SugarColumn(ColumnName = "DoOnce")]
            public byte DoOnce { get; set; }
            /// <summary>
            /// 立即执行 
            ///</summary>
            [SugarColumn(ColumnName = "StartNow")]
            public byte StartNow { get; set; }
            /// <summary>
            /// 执行类型 
            ///</summary>
            [SugarColumn(ColumnName = "ExecuteType")]
            public int ExecuteType { get; set; }
            /// <summary>
            /// 间隔时间 
            /// 默认值: NULL
            ///</summary>
            public int? Interval { get; set; }
            /// <summary>
            /// Cron表达式 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "Cron")]
            public string Cron { get; set; }
            /// <summary>
            /// 定时器类型 
            ///</summary>
            [SugarColumn(ColumnName = "TimerType")]
            public int TimerType { get; set; }
            /// <summary>
            /// 请求url 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "RequestUrl")]
            public string RequestUrl { get; set; }
            /// <summary>
            /// 请求参数 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "RequestParameters")]
            public string RequestParameters { get; set; }
            /// <summary>
            /// Headers 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "Headers")]
            public string Headers { get; set; }
            /// <summary>
            /// 请求类型 
            ///</summary>
            [SugarColumn(ColumnName = "RequestType")]
            public int RequestType { get; set; }
            /// <summary>
            /// 备注 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "Remark")]
            public string Remark { get; set; }
            /// <summary>
            /// 创建时间 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "CreatedTime")]
            public DateTime? CreatedTime { get; set; }
            /// <summary>
            /// 更新时间 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "UpdatedTime")]
            public DateTime? UpdatedTime { get; set; }
            /// <summary>
            /// 创建者Id 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "CreatedUserId")]
            public long? CreatedUserId { get; set; }
            /// <summary>
            /// 创建者名称 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "CreatedUserName")]
            public string CreatedUserName { get; set; }
            /// <summary>
            /// 修改者Id 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "UpdatedUserId")]
            public long? UpdatedUserId { get; set; }
            /// <summary>
            /// 修改者名称 
            /// 默认值: NULL
            ///</summary>
            [SugarColumn(ColumnName = "UpdatedUserName")]
            public string UpdatedUserName { get; set; }
            /// <summary>
            /// 软删除标记 
            ///</summary>
            [SugarColumn(ColumnName = "IsDeleted")]
            public byte IsDeleted { get; set; }
        }

        [SugarTable("Country_1111")]
        public class Country1111
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true, ColumnName = "cid")]
            public int Id { get; set; }
            public string Name { get; set; }
            public int InfoId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(InfoId))]
            public Country111Info Info { get; set; }

            [Navigate(NavigateType.OneToMany,nameof(Province1111.CountryId))]
            public List<Province1111> Provinces { get; set; }
        }

        public class Country111Info
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,ColumnName = "infoId")]
            public int Id { get; set; }
            public string Name { get; set; } 
        }

        [SugarTable("Province_1111")]
        public class Province1111
        {
            [SqlSugar.SugarColumn(   ColumnName = "pid")]
            public int Id { get; set; }
            public string Name { get; set; }
            [SugarColumn(ColumnName = "coid")]
            public int CountryId { get; set; } 
        }
     
    }
}
