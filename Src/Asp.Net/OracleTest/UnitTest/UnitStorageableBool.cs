using System;
using SqlSugar;

namespace OrmTest
{
    public class UnitStorageableBool
    {
        public static void Init()
        {
            var db = NewUnitTest.Db; 
            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test0011", false))
            {
                db.CodeFirst.InitTables<Test001>();
            }
            db.Aop.OnError = (exp) =>//SQL报错
            {
                //Console.WriteLine($"SqlSugar[Error]：{UtilMethods.GetSqlString(DbType.SqlServer, exp.Sql, exp.Parametres)}" + Environment.NewLine);
            };

            //用例代码 
            var result = db.Insertable(new Test001() { id = true }).ExecuteCommand();//用例代码
            var result1 = db.Insertable(new Test001() { id = false }).ExecuteCommand();//用例代码
            var res = db.Storageable(new Test001() { id1 = false, id = false }).WhereColumns(x => x.id1)
                .SplitUpdate(x => x.Any()).ToStorage();
            res.AsUpdateable.ExecuteCommand();
            //Console.WriteLine(result);
            //Console.WriteLine("用例跑完");
           // Console.ReadKey();
        }
        //建类
        [SugarTable("Test0011")]
        public class Test001
        {
            [SugarColumn(ColumnDataType = "NUMBER(1)", SqlParameterDbType = System.Data.DbType.Int16)]
            public bool id { get; set; }
            [SugarColumn(ColumnDataType = "NUMBER(1)", SqlParameterDbType = System.Data.DbType.Int16)]
            public bool id1 { get; set; }
        }
    }
}
