using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace OrmTest
{ 
    internal class Unitdsfasdfys
    {
        public static  void Init()
        {
            ConnectionConfig config = new ConnectionConfig();
            config.ConnectionString =DbHelper.Connection;
            config.ConfigId = "0";
            config.IsAutoCloseConnection = true;
            config.DbType = SqlSugar.DbType.SqlServer;
            config.MoreSettings = new ConnMoreSettings()
            {
                SqlServerCodeFirstNvarchar = true
            };
            config.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (c, p) =>
                {
                    if (c.PropertyType == typeof(object) && p.DataType == "sql_variant")
                    {
                        p.SqlParameterDbType = SqlDbType.Variant;
                    }
                    if (p.IsPrimarykey == false && c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))//自动可为空
                    {
                        p.IsNullable = true;
                    }
                    if (p.PropertyName.ToLower() == "id" && p.IsPrimarykey)//默认Id这个为主键
                    {
                        p.IsPrimarykey = true;
                        if (p.PropertyInfo.PropertyType == typeof(int))
                        {
                            p.IsIdentity = true;//是id并且是int的是自增
                        }
                    }
                },
                EntityNameService = (type, entity) =>
                {
                    //entity.DbTableName 修改表名
                }
            };
            SqlSugarScope sqlSugar = new SqlSugarScope(new List<ConnectionConfig>() { config },
            db =>
            {
                db.GetConnectionScope("0").Aop.OnError = (exp) =>
                {
                    var sql = exp.Sql;
                    var parameters = exp.Parametres;
                    var str = $"0--SqlSugar异常 ：{exp}";
                };
                db.GetConnectionScope("0").Aop.OnLogExecuting = (sql, pars) =>
                {
                    string msg = $"0--SqlSugar 执行了Sql语句：{sql}";
                };
            });
            sqlSugar.GetConnectionScope("0").DbMaintenance.CreateDatabase();
            var Db = sqlSugar.AsTenant().GetConnectionScope("0");
            Db.CodeFirst.InitTables(new Type[] { typeof(TestDateTime) });
            Db.DbMaintenance.TruncateTable<TestDateTime>();
            TestDateTime testDateTime = new TestDateTime()
            {
                CreateTime = DateTime.Parse("2025-03-29 09:27:37.9991749")
            };
            Db.Insertable(testDateTime).ExecuteCommand();

            var data = Db.Queryable<TestDateTime>().First().CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            if (data != "2025-03-29 09:27:37.9991749") throw new Exception("unit error");


            var result = Db.Queryable<SupplierStatementOrder>()
               .Select(s =>  (decimal?)s.TotalArrearsAmount)
               .ToSqlString();

            //if(result.Contains("*")) throw new Exception("unit error"); 
        }
    }
    /// <summary>
    /// 供应商对账单
    /// </summary>
    [SugarIndex($"index_{{table}}_{nameof(SheetNo)}", nameof(SheetNo), OrderByType.Asc)]
    [SugarIndex($"index_{{table}}_{nameof(SupplierId)}", nameof(SupplierId), OrderByType.Asc)]
    [SugarIndex($"index_{{table}}_{nameof(BisDate)}", nameof(BisDate), OrderByType.Asc)]
    [SugarIndex($"index_{{table}}_{nameof(BeginDate)}", nameof(BeginDate), OrderByType.Asc)]
    [SugarIndex($"index_{{table}}_{nameof(EndDate)}", nameof(EndDate), OrderByType.Asc)]
    public class SupplierStatementOrder
    {
        /// <summary>
        /// 雪花Id
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }

        /// <summary>
        /// 单号
        /// </summary>

        public string SheetNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public long SupplierId { get; set; }


        /// <summary>
        /// 对账起始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 对账结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime BisDate { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 2, DefaultValue = "0")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 仅查询当前交易期业务单据
        /// </summary>
        [SugarColumn(DefaultValue = "1")]
        public bool OnlyCurrentPeriod { get; set; } = true;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string Remark { get; set; }


        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? ApproveTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public long ApproveBy { get; set; }

        /// <summary>
        /// 审核人名称
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string ApproveByName { get; set; }

        #region 账款信息

        /// <summary>
        /// 上期结欠金额<br/>
        /// 期初+记账+调整-付款【查询账款流水，按交易期间开始日期前的发生金额合计】--》供应商第一次对账时，这样取；后续的上期结欠=该供应商上一张对账单的累计结欠！
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 2, DefaultValue = "0")]
        public decimal LastArrearsAmount { get; set; }

        /// <summary>
        /// 本期购货金额 <br/>
        /// 本期购货情况的金额合计
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 2, DefaultValue = "0")]
        public decimal PeriodPurchaseAmount { get; set; }

        /// <summary>
        /// 本期付款 <br/>
        /// 本期付款情况的金额合计
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 2, DefaultValue = "0")]
        public decimal PeriodPayAmount { get; set; }

        /// <summary>
        /// 累计结欠 <br/>
        /// 上期结欠+本期购货金额-本期付款
        /// </summary>
        [SugarColumn(Length = 18, DecimalDigits = 2, DefaultValue = "0")]
        public decimal TotalArrearsAmount { get; set; }

        #endregion
    }
    public class TestDateTime
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(SqlParameterDbType=System.Data.DbType.DateTime2 , ColumnDataType = "datetime2(7)")]
        public DateTime CreateTime { get; set; }
    }
}
