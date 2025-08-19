using Microsoft.Identity.Client;
using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitdafasyfasfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                TableEnumIsString = true
            };
            //建表 
            db.CodeFirst.InitTables<TestEntity>();
            db.DbMaintenance.TruncateTable<TestEntity>();
            var insertData = new TestEntity
            {
                ID = 1,
                Stage = TestStage.Stage2
            };
            db.Insertable(insertData).ExecuteCommand();
            var e1 = db.Ado.GetString("select Stage from unit_test_version");
            insertData.Stage = TestStage.Stage3;
            db.Updateable(insertData).WhereColumns(s => s.ID).ExecuteCommand();
            var e2 = db.Ado.GetString("select Stage from unit_test_version");
            if (e1 != "Stage2") throw new Exception("unit error");
            if (e2 != "Stage3") throw new Exception("unit error");
        }
        [SqlSugar.SugarTable("unit_test_version")]
        public class TestEntity
        {
            public long ID { get; set; }

            [SugarColumn(ColumnDataType = "varchar(100)", SqlParameterDbType = typeof(EnumToStringConvert))]
            public TestStage Stage { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum TestStage
        {
            /// <summary>
            /// 
            /// </summary>
            Stage1 = 0,
            /// <summary>
            /// 
            /// </summary>
            Stage2,
            /// <summary>
            /// 
            /// </summary>
            Stage3
        }

    }
}
