using SqlSugar;
using SqlSugar.DbConvert;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OrmTest
{

    public enum DataStatus
    {
        Normal,
        Error,
    }

    public class DemoTable
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "data_status", ColumnDataType = "varchar", Length = 50, SqlParameterDbType = typeof(EnumToStringConvert))]
        public DataStatus Status { get; set; }
    }

    public class Unitadfasfysdfyss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(typeof(DemoTable));
            db.DbMaintenance.TruncateTable<DemoTable>();
            db.Insertable<DemoTable>(new DemoTable
            {
                Status = DataStatus.Normal
            }).ExecuteCommand();

            var obj =db.Updateable<DemoTable>()
                .SetColumns(it => new DemoTable { Status = DataStatus.Error }) // 数据库中的字段会变成int值
                //.SetColumns(it => it.Status == DataStatus.Error)//数据库的字段是字符串值
                                                                //.SetColumns(it => it.Status, DataStatus.Error)//数据库中的字段会变成int值
                .Where(it => it.Id != 0)
                .ExecuteCommand();

            if (db.Queryable<DemoTable>().First().Status != DataStatus.Error) 
            {
                throw new System.Exception("unit error");
            }
        }
    }
}