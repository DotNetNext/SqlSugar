using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitNavInsertadfa1
    {
        public static void Init() 
        {
   
            //SQLiteConnection.CreateFile(Environment.CurrentDirectory + @"\Test.sqlite");

            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = @"DataSource=" + Environment.CurrentDirectory + @"\Test1.sqlite",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            });

            //建表 

            db.CodeFirst.InitTables<AssignMission>();
            db.CodeFirst.InitTables<AssignMissionDetail>();
            db.DbMaintenance.TruncateTable<AssignMission>();
            db.DbMaintenance.TruncateTable<AssignMissionDetail>();
            db.Insertable(new AssignMission()
            {
                Transporttask_NO = "a",
                Carrier_Name = "a1"
            }).ExecuteReturnEntity();
            db.Insertable(new AssignMissionDetail()
            {
                Transporttask_NO = "a",
                Brand_No = "child1"
            }).ExecuteReturnEntity();
            //用例代码 
            List<AssignMission> list = db.Queryable<AssignMission>()
               .Includes(it => it.detailList)
               .ToList();
            foreach (var item in list)
            {
                item.id = 0;
            }
            bool b = db.InsertNav(list)
            .Include(z1 => z1.detailList)
            .ExecuteCommand();

            List<AssignMission> list2 = db.Queryable<AssignMission>()
              .Includes(it => it.detailList)
              .ToList();

            if (list2.Count() != 2) 
            {
                throw new Exception("unit error");
            }
        }

        //建类
        [SugarTable("ti_assignmission1")]
        public partial class AssignMission
        {
            public AssignMission()
            {


            }

            /// <summary>
            /// Desc:
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }

            /// <summary>
            /// Desc:运输任务单号
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(ColumnName = "transporttask_no", IsNullable = true)]
            public string Transporttask_NO { get; set; }

            /// <summary>
            /// Desc:承运商名称
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "carrier_name", IsNullable = true)]
            public string Carrier_Name { get; set; }

            /// <summary>
            /// Desc:车牌号
            /// Default:
            /// Nullable:True
            /// </summary>  
            [SugarColumn(ColumnName = "truck_name", IsNullable = true)]
            public string Truck_Name { get; set; }

            /// <summary>
            /// Desc:驾驶员
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "driver_name", IsNullable = true)]
            public string Driver_Name { get; set; }

            /// <summary>
            /// Desc:装货地磅号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "wb_no", IsNullable = true)]
            public string wbNO { get; set; }

            /// <summary>
            /// Desc:变更类型(新增、删除、内容变更)
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "change_type", IsNullable = true)]
            public string changeType { get; set; }


            [Navigate(NavigateType.OneToMany, nameof(AssignMissionDetail.Transporttask_NO), nameof(Transporttask_NO))]
            public List<AssignMissionDetail> detailList { get; set; }

        }


        [SugarTable("ti_assignmission_detail1")]
        public partial class AssignMissionDetail
        {
            public AssignMissionDetail()
            {


            }

            /// <summary>
            /// Desc:
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int id { get; set; }

            /// <summary>
            /// Desc:运输任务单号
            /// Default:
            /// Nullable:False
            /// </summary>           
            [SugarColumn(ColumnName = "transporttask_no", IsNullable = true)]
            public string Transporttask_NO { get; set; }

            /// <summary>
            /// Desc:销售订单号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "order_no", IsNullable = true)]
            public string Order_No { get; set; }

            /// <summary>
            /// Desc:交货单号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "delivery_order_no", IsNullable = true)]
            public string Delivery_Order_No { get; set; }

            /// <summary>
            /// Desc:交货单行号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "ap_rowno", IsNullable = true)]
            public string Sap_Rownos { get; set; }

            /// <summary>
            /// Desc:批次
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "batch", IsNullable = true)]
            public string Batch { get; set; }

            /// <summary>
            /// Desc:品名
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "product_name", IsNullable = true)]
            public string Product_Name { get; set; }

            /// <summary>
            /// Desc:牌号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "brand_no", IsNullable = true)]
            public string Brand_No { get; set; }

            /// <summary>
            /// Desc:IC卡号
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "ic_cardid", IsNullable = true)]
            public string IC_Cardid { get; set; }

            /// <summary>
            /// Desc:变更类型(新增、删除、内容变更)
            /// Default:
            /// Nullable:True
            /// </summary>           
            [SugarColumn(ColumnName = "change_type", IsNullable = true)]
            public string changeType { get; set; }

        }
    }
}
