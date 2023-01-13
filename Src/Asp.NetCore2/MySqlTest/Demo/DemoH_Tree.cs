using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlTest.Demo
{
    public class DemoH_Tree
    {
        public static void Init()
        {
            Task.Run(Test_ToParentList).Wait();
        }
        private static async Task Test_ToParentList()
        {
            var db = GetInstance();
            var data = await db.Queryable<CustomerAddressTemplateDetail>().ToParentListAsync(x => x.ParentCode, 2, x => x.TemplateId == 1611933284013932544);
        }

        private static SqlSugarClient GetInstance()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.MySql,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
        }
    }

    [SugarTable("customer_address_template_detail", TableDescription = "客户地址模板详情")]
    [SugarIndex("idx_code", nameof(Code), OrderByType.Asc)]
    public class CustomerAddressTemplateDetail 
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }
        /// <summary>
        /// 模板id
        /// </summary>
        [SugarColumn(ColumnDescription = "模板id")]
        public long TemplateId { get; set; }

        [SugarColumn(ColumnDescription = "地址", Length = 128,IsTreeKey = true)]
        public string Code { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "上级地址", Length = 128)]
        public string ParentCode { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "名称", Length = 256)]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "级别")]
        public int Level { get; set; }

        [SugarColumn(ColumnDescription = "得力末级地址")]
        public long DistrictId { get; set; }


    }


}
