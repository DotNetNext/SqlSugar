using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitTest
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.SetStringDefaultLength(200)
                .SplitTables()
                .InitTables(typeof(tb_test));
            db.Insertable(new tb_test
            {
                Key = "123"
            })
        .SplitTable()
        .ExecuteCommand();
 

           var rows=db.Updateable(new tb_test
            {
               Id=Guid.NewGuid(),
                Key = "123",
                Value = "901"
            })
                .WhereColumns(c => c.Key)
                .UpdateColumns(c => c.Value)
                .SplitTable()
                .ExecuteCommand();
            if (rows == 0) 
            {
                throw new Exception("unit error");
            }
        }
    }
    [SplitTable(SplitType.Day)]  //需要这里取消并InitTables时不用SplitTables()，SetStringDefaultLength才会生效
    [SugarTable("tb_test_{year}{month}{day}")]
    public class tb_test
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }

        public string Key { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Value { get; set; }
    }
}
