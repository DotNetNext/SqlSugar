using SqlSugar;
using System.Collections.Generic;
using System;

namespace OrmTest
{
    public class Unit12CIdentity
    { 
        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = "DATA SOURCE=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 124.223.23.162)(PORT = 1521)) (CONNECT_DATA = (SERVER = DEDICATED) (SERVICE_NAME = orcl)));USER ID=bslis;PASSWORD=PassW0rd;",
                DbType = DbType.Oracle,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoRemoveDataCache = true, // 启用自动删除缓存，所有增删改会自动调用.RemoveDataCache()
                    IsAutoDeleteQueryFilter = true, // 启用删除查询过滤器
                    IsAutoUpdateQueryFilter = true, // 启用更新查询过滤器
                    SqlServerCodeFirstNvarchar = true, // 采用Nvarchar
                    EnableOracleIdentity = true        //启用Oracle自增列，需要12C以上版本，11G看文档1.1
                }
            });

            var cost = new BL_Cost1()
            {
                Id = 0,
                PatId = "0001283782",
                PatNo = "MZ0001283782",
                DeptId = "35",
                RequestTime = DateTime.Now,
            };
            cost.Details = new List<BL_CostDetail1>
            {
                new() {
                    Id = 0,
                    CostId = 0,
                    ItemId = "0001",
                    ItemName = "组套01",
                    Fyxh = "F0001",
                    Name = "明细01",
                    Price = 15,
                    Amount = 2
                },
                new() {
                    Id = 0,
                    CostId = 0,
                    ItemId = "0002",
                    ItemName = "组套02",
                    Fyxh = "F0002",
                    Name = "明细02",
                    Price = 15,
                    Amount = 2
                }
            };

            var xxx=db.Insertable(cost.Details).ExecuteCommandIdentityIntoEntity();

            db.InsertNav(cost)
                    .Include(z1 => z1.Details)
                    .ExecuteCommand();
        }

        /// <summary>
        /// 费用明细
        /// </summary>
        [SugarTable("BL_Cost")]
        public partial class BL_Cost1

        {
            /// <summary>
            /// 主键Id
            /// </summary>
            [SugarColumn(ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            /// <summary>
            /// 病人ID
            /// </summary>
            public string PatId { get; set; }

            /// <summary>
            /// 就诊号
            /// </summary>
            public string PatNo { get; set; }

            /// <summary>
            /// 计费科室
            /// </summary>
            public string DeptId { get; set; }

            /// <summary>
            /// 申请日期
            /// </summary>
            public DateTime RequestTime { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(BL_CostDetail1.CostId))]
            public List<BL_CostDetail1> Details { get; set; }
        }

        /// <summary>
        /// 费用明细
        /// </summary>
        [SugarTable("BL_CostDetail")]
        public partial class BL_CostDetail1
        {
            /// <summary>
            /// 主键Id
            /// </summary>
            [SugarColumn(ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            /// <summary>
            /// 父表主键
            /// </summary>
            public int CostId { get; set; }

            /// <summary>
            /// 组套ID
            /// </summary>
            public string ItemId { get; set; }

            /// <summary>
            /// 组套名称
            /// </summary>
            public string ItemName { get; set; }

            /// <summary>
            /// 费用序号
            /// </summary>
            public string Fyxh { get; set; }

            /// <summary>
            /// 费用名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public decimal Price { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public decimal Amount { get; set; }
        }
    }
}