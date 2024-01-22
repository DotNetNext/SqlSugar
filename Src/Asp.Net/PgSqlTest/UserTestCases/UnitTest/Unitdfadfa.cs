using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbType = SqlSugar.DbType;

namespace OrmTest
{
    internal class Unitdfaf2yyyyasda
    {
         public  static void Init()
        {
            //通过ConfigId进行区分是哪个库
            var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
              new ConnectionConfig(){
                  ConfigId="Postgre",DbType=DbType.PostgreSQL,
                    ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true,
                   ConfigureExternalServices=new ConfigureExternalServices()
                   {
                       EntityService = (x,p) => //处理列名
                       {
                          //最好排除DTO类
                         p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName);//ToUnderLine驼峰转下划线方法
                       },
                       EntityNameService = (x, p) => //处理表名
                       {
                          //最好排除DTO类
                         p.DbTableName=UtilMethods.ToUnderLine(p.DbTableName);//ToUnderLine驼峰转下划线方法
                       }
                    }
              },
              new ConnectionConfig(){
                  ConfigId="Oracle",DbType=DbType.SqlServer,
                    ConnectionString="server=.;uid=sa;pwd=sasa;database=SqlSugar5Demo",IsAutoCloseConnection=true
              }
            }
         );
            db.GetConnection("Oracle").Aop.OnLogExecuting = (sql, pars) =>
            {

                //获取原生SQL推荐 5.1.4.63  性能OK
                Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

            };
            db.GetConnection("Postgre").Aop.OnLogExecuting = (sql, pars) =>
            {

                //获取原生SQL推荐 5.1.4.63  性能OK
                Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));

            };


           db.CodeFirst.InitTablesWithAttr(typeof(Doctor),typeof(HisDoctor)); // 业务需求只迁移一个表

            db.Insertable(new Doctor()
            {
                 Avatar="",
                  CreatedAt=DateTime.Now,
                   GoodAt="",
                    HisId=SnowFlakeSingle.Instance.NextId(),
                     Intro="",
                      UpdatedAt=DateTime.Now
            }).ExecuteCommand();
            var list = db.GetConnection("Postgre").Queryable<Doctor>()
                           .CrossQuery(typeof(HisDoctor), "Oracle")
                           .Includes(z => z.HisInfo)
                           .ToList();
            Console.WriteLine(list);
        }
    }

    public abstract class BaseModel
    {
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public virtual long Id { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true, IsNullable = true, InsertServerTime = true)]
        public virtual DateTime CreatedAt { get; set; }

        [SugarColumn(IsNullable = true, InsertServerTime = true)]
        public virtual DateTime UpdatedAt { get; set; }
    }

    [TenantAttribute("Postgre")]
    [SugarTable("doctorsaa")]
    [SugarIndex("index_doctors_his_id", nameof(Doctor.HisId), OrderByType.Asc, true)]
    public class Doctor : BaseModel
    { 
        public long HisId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public virtual string Intro { get; set; }
        [SugarColumn(ColumnDescription = "擅长", IsNullable = true)]
        public virtual string GoodAt { get; set; }
        [SugarColumn(IsNullable = true)]
        public virtual string Avatar { get; set; }
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(HisId), nameof(HisDoctor.Id))]
        public HisDoctor HisInfo { get; set; }
    }


    [TenantAttribute("Oracle")]
    [SugarTable("ZH_EMPLOYEE_DICT")]
    public class HisDoctor
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "EMPLOYEEID")]
        public long Id { get; set; }
        [SugarColumn(ColumnName = "INPUTCODE")]
        public string Code { get; set; }
        [SugarColumn(ColumnName = "EMPLOYEENAME")]
        public string Name { get; set; }
        [SugarColumn(ColumnName = "EMPLOYEETYPE", ColumnDescription = "员工类别")]
        public int ProfessionRoleId { get; set; }
        [SugarColumn(ColumnName = "EMPLOYEETITLE", ColumnDescription = "员工职称")]
        public int ProfessionTitleId { get; set; }
        [SugarColumn(ColumnName = "INTRODUCE", ColumnDescription = "员工介绍")]
        public string Intro { get; set; }
    }

}