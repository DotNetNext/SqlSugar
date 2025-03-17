using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitsadfadsayss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            //建表 
            db.CodeFirst.InitTables<Test001>();
            //清空表
            //db.DbMaintenance.TruncateTable<Test001>();


            //插入测试数据
            var result = db.Insertable(new Test001()).ExecuteCommand();//用例代码
            var xxx = db.Queryable<Test001>().ToList();
            if (xxx.First().test6 != 2)
            {
                throw new Exception("unit error");
            }
            db.CodeFirst.InitTables<TEST001>();
            var xxx2 = db.Queryable<TEST001>().ToList();
            if (xxx2.First().test7 != 2)
            {
                throw new Exception("unit error");
            }
        }
    }


    //建类
    [SugarTable("UnitdaayTest0012")]
    public class Test001
    {
        public int id { get; set; }
        [SugarColumn(ColumnName = "test6", DefaultValue = "2", IsOnlyIgnoreInsert = true)]
        public double test6 { get; set; }
    }
    //建类
    [SugarTable("UnitdaayTest0012")]
    public class TEST001
    {
        public int id { get; set; }
        [SugarColumn(ColumnName = "test6", DefaultValue = "2", IsOnlyIgnoreInsert = true)]
        public double test6 { get; set; }
        [SugarColumn(ColumnName = "test7", DefaultValue = "2", IsOnlyIgnoreInsert = true)]
        public double test7 { get; set; }
    }

}
 
