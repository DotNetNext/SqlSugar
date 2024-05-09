using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.Unitadfasfa;

namespace OrmTest
{
    internal class Unitdfafa11
    {
        public static void Init() 
        {
            NewUnitTest.Db.CodeFirst.InitTables<OutboundLine, OutboundOrder, OutboundLineAllocation, WaveTask>();
            var outboundLineAllocationList = NewUnitTest.Db.Context.Queryable<OutboundLineAllocation>()

                                                  .Where(x => SqlFunc.Subqueryable<OutboundLine>()
                                                            .Where(y => y.OutboundOrder.WaveId == x.OutboundLine.OutboundOrder.WaveId)
                                                            .Where(y => y.QuantityAllocated > 0).NotAny()
                                                        )
                                                  .OrderBy(x => x.Id)
                                                  .Take(int.MaxValue)
                                                  .ToList();
        }
    }
    [SugarTable("OutboundLine")]
    [Tenant("0")]
    public class OutboundLine
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int OutboundOrderId { get; set; }
        public int QuantityAllocated { get; set; }


        [Navigate(NavigateType.OneToOne, nameof(OutboundOrderId))]
        public OutboundOrder OutboundOrder { get; set; }
    }
    [SugarTable("OutboundOrder")]
    [Tenant("0")]
    public class OutboundOrder
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(OutboundLine.OutboundOrderId))]
        public List<OutboundLine> OutboundLines { get; set; }

        [SugarColumn(ColumnName = "WaveId", IsNullable = false)]
        public int WaveId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(WaveId))]
        public WaveTask Wave { get; set; }
    }
    [SugarTable("WaveTask")]
    [Tenant("0")]
    public class WaveTask
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(OutboundOrder.WaveId))]
        public List<OutboundOrder> OutboundOrders { get; set; }
    }
    [SugarTable("OutboundLineAllocation")]
    [Tenant("0")]
    public class OutboundLineAllocation
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true)]
        public int Id { get; set; }

        public int OutboundLineId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(OutboundLineId))]
        public OutboundLine OutboundLine { get; set; }
    }
}
