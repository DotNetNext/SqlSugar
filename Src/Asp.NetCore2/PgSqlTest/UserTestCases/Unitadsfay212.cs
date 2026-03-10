using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;

namespace OrmTest
{
    internal class Unitdaadsysfs
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表
            db.CodeFirst.InitTables<Test001>();
            db.CodeFirst.InitTables<Test002>();

            //清空表
            db.DbMaintenance.TruncateTable<Test001>();
            db.DbMaintenance.TruncateTable<Test002>();

            List<Test001> inserDataList = new List<Test001>
            {
                new Test001() { Id = 1 },
                new Test001() { Id = 2 }
            };
            List<Test002> inserData2List = new List<Test002>
            {
                new Test002() { Id = 2 }
            }; 

            //插入测试数据
            db.Insertable(inserDataList).ExecuteCommand();
            db.Insertable(inserData2List).ExecuteCommand();

            //正常更新值为B
            db.Updateable<Test001>()
                .Where(one => one.TestEnum == TestEnum.A && one.Id == 1)
                .SetColumns(one => new Test001
                {
                    TestEnum = TestEnum.B
                })
                .ExecuteCommand();

            //错误更新为了2
            db.Updateable<Test001>()
                .InnerJoin<Test002>((one, two) => one.Id == two.Id)
                .Where((one, two) => one.TestEnum == TestEnum.A)
                .SetColumns((one, two) => new Test001
                {
                    TestEnum = TestEnum.B
                })
                .ExecuteCommand(); 
        }

        [SugarTable("unitadadsfadsy2")]
        public class Test001
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnDescription = "测试枚举", ColumnDataType = "varchar(20)", SqlParameterDbType = typeof(EnumToStringConvert), DefaultValue = "A")]
            public TestEnum TestEnum { get; set; } = TestEnum.A;
        }
        [SugarTable("unitadadsfadsy222")]
        public class Test002
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
        }

        public enum TestEnum
        {
            A = 1,
            B = 2
        }
    }
}