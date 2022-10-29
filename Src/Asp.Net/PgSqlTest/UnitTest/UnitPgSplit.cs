using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest {
    public class UnitPgSplit
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            db.Insertable(new SysTest() { CName = "a", Name = "a", Status = OrderStatus.Todo }).SplitTable().ExecuteReturnSnowflakeId();
            db.Updateable(new List<SysTest>() {
            new SysTest() { CName = "b", Name = "b" }
            })
                .UpdateColumns(x => x.Status).SplitTable(it=>it.Take(3))
                .ExecuteCommand();
        }
    }
}
/// <summary>
/// 测试表1
/// </summary>
[SplitTable(SplitType.Year)]//按年分表 （自带分表支持 年、季、月、周、日）
[SugarTable("sys_test1111_{year}{month}{day}")]
public class SysTest
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    public virtual long Id { get; set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 中文名
    /// </summary>
    public string CName { get; set; }

    /// <summary>
    ///状态
    /// </summary>
    public OrderStatus Status { get; set; }
}

/// <summary>
/// 工单状态
/// </summary>
[Description("工单状态")]
public enum OrderStatus
{
    /// <summary>
    /// 待办
    /// </summary>
    [Description("待办")]
    Todo = 0,
    /// <summary>
    /// 进行中
    /// </summary>
    [Description("进行中")]
    Doing = 1,
    /// <summary>
    /// 完成
    /// </summary>
    [Description("完成")]
    Done = 2,
    /// <summary>
    /// 超时完成
    /// </summary>
    [Description("超时完成")]
    TimeOut = 3,
}

