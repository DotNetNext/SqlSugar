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
            Test_ToParentList();
            Test_ToParentListWhere();
            Task.Run(Test_ToParentListAsync).Wait();
            Task.Run(Test_ToParentListWhereAsync).Wait();
        }

        private static void Test_ToParentList()
        {
            var db = GetInstance();
            var data = db.Queryable<CustomerAddressTemplateDetail>().ToParentList(x => x.ParentCode, 2);
            var data2 = db.Queryable<District>().ToParentList(x => x.ParentId, 110101004004);
        }

        private static void Test_ToParentListWhere()
        {
            var db = GetInstance();
            var data = db.Queryable<CustomerAddressTemplateDetail>().ToParentList(x => x.ParentCode, 2, x => x.TemplateId == 1611933284013932544);
            var data2 = db.Queryable<District>().ToParentList(x => x.ParentId, 110101004004,x=>x.DeletedAt==0);
        }
        private static async Task Test_ToParentListAsync()
        {
            var db = GetInstance();
            var data = await db.Queryable<CustomerAddressTemplateDetail>().ToParentListAsync(x => x.ParentCode, 2);
            var data2 = await db.Queryable<District>().ToParentListAsync(x => x.ParentId, 110101004004);
        }

        private static async Task Test_ToParentListWhereAsync()
        {
            var db = GetInstance();
            var data = await db.Queryable<CustomerAddressTemplateDetail>().ToParentListAsync(x => x.ParentCode, 2, x => x.TemplateId == 1611933284013932544);
            var data2 = await db.Queryable<District>().ToParentListAsync(x => x.ParentId, 110101004004, x => x.DeletedAt == 0);
        }

        private static SqlSugarClient GetInstance()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.MySql,
                ConnectionString = "Data Source=192.168.95.11;port=33306;Database=cube;AllowLoadLocalInfile=true;User ID=root;Password=dljs2022;allowPublicKeyRetrieval=true;pooling=true;CharSet=utf8;sslmode=none;AllowUserVariables=true;",
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
    [SugarTable("district", TableDescription = "省市区街道村5级信息(https://github.com/adyliu/china_area)")]
    [SugarIndex("idx_parentid_level", nameof(ParentId), OrderByType.Asc, nameof(Level), OrderByType.Asc)]
    public class District 
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnDescription = "上级Id")]
        public long ParentId { get; set; }

        [SugarColumn(ColumnDescription = "名称", Length = 128)]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "级别")]
        public int Level { get; set; }

        public long DeletedAt { get; set; }
    }


}
