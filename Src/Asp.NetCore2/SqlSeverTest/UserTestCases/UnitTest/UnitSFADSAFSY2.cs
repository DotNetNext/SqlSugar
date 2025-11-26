using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitSFADSAFSY2
    {
        public static void Init() 
        {
            var client = NewUnitTest.Db;
            client.CodeFirst.InitTables<UnitdafadfaaaTest1>();
            client.DbMaintenance.TruncateTable<UnitdafadfaaaTest1>();
            var test = new UnitdafadfaaaTest1()
            {
                Id = 1,
                Type = MyType.Type1,
                Name = "3"
            };
            client.Deleteable<UnitdafadfaaaTest1>().ExecuteCommand();

            client.Storageable(test).ExecuteCommand();

            var list=client.Queryable<UnitdafadfaaaTest1>().ToList();

            client.Storageable(test).ExecuteCommand();
        }
        [SugarTable]
        public class UnitdafadfaaaTest1
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            [SugarColumn(IsPrimaryKey = true,ColumnDataType ="varchar(10)", SqlParameterDbType = typeof(EnumToStringConvert))]
            public MyType Type { get; set; }
            public string Name { get; set; }
        }

        public enum MyType
        {
            Type1, Type2
        } 
    }
}
