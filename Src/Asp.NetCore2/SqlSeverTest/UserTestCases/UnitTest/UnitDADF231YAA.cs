using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{

    public class UnitDADF231YAA
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            db.CodeFirst.InitTables<Test001faf1aaa>();
            //清空表
            db.DbMaintenance.TruncateTable<Test001faf1aaa>();

            //插入测试数据
            var result = db.Insertable(new Test001faf1aaa()
            {
                Id = 1000000000001,
                Code = "0001",
                Name = "Test"
            }).ExecuteCommand();//用例代码

            var permissions = new string[] { "ok" };
            var display = permissions.Contains("ok");

            var list = db.Queryable<Test001faf1aaa>()
                .Select(it => new Test001faf1aaa
                {
                    Id = it.Id,
                    Code = it.Code,
                    Name = it.Name,
                    cProjectTrackingDefine7 = display ? it.cProjectTrackingDefine7 : null
                })
                .ToList();

            var list2 = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = it.Id,
                  Code = it.Code,
                  Name = it.Name,
                  cProjectTrackingDefine7 = SqlFunc.IIF(display, it.cProjectTrackingDefine7, null)
              })
              .ToList();
            Test01(db);
            Test02(db);
            Test03(db);
            Test04(db);
            Test05(db);
            Test06(db);
        }

        private static void Test01(SqlSugarClient db)
        { 
            var num = 3;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num == 1 ? it.Id : num == 2 ? 2 : 3
              })
              .First().Id;
            if (value != 3)
            {
                throw new Exception("unit error");
            }
        }
        private static void Test02(SqlSugarClient db)
        {
            var num = 2;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num == 1 ? it.Id : num == 2 ? 2 : 3
              })
              .First().Id;
            if (value != 2)
            {
                throw new Exception("unit error");
            }
        }
        private static void Test03(SqlSugarClient db)
        {
            var num = 1;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num == 1 ? it.Id : num == 2 ? 2 : 3
              })
              .First().Id;
            if (value != 1000000000001)
            {
                throw new Exception("unit error");
            }
        }
        private static void Test04(SqlSugarClient db)
        {
            var num = 10;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num == 1 ? 1 : num == 2 ? 2 : num== it.Id?4:5
              })
              .First().Id;
            if (value != 5)
            {
                throw new Exception("unit error");
            }
        }

        private static void Test05(SqlSugarClient db)
        {
            var num = 1000000000001;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num == 1 ? 1 : num == 2 ? 2 : num == it.Id ? 4 : 5
              })
              .First().Id;
            if (value != 4)
            {
                throw new Exception("unit error");
            }
        }

        private static void Test06(SqlSugarClient db)
        {
            var num = 1000000000001;
            var value = db.Queryable<Test001faf1aaa>()
              .Select(it => new Test001faf1aaa
              {
                  Id = num != 1 ? (num!=2?1:2):2
              })
              .First().Id;
            if (value != 1)
            {
                throw new Exception("unit error");
            }
        }

        [SqlSugar.SugarTable("unitdafaf1311")]
        public class Test001faf1aaa
        {
            /// <summary>
            /// 主键Id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public virtual long Id { get; set; }

            [SugarColumn(Length = 100, IsNullable = false)]
            public string Code { get; set; }

            [SugarColumn(Length = 100, IsNullable = false)]
            public string Name { get; set; }

            [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true, IsJson = true, IsOnlyIgnoreUpdate = true)]
            public List<FormattedTextOutput> cProjectTrackingDefine7 { get; set; }
        }

        public class FormattedTextOutput
        {
            public FormattedTextOutput()
            {

            }

            public FormattedTextOutput(string type, string text)
            {
                this.Type = type;
                this.Text = text;
            }

            public string Type { get; set; }

            public string Text { get; set; }
        }
    }


}
