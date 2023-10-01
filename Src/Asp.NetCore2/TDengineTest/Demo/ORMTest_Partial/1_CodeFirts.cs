using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void CodeFirst(SqlSugarClient db)
        {
            CodeFirst1(db);

            db.CodeFirst.InitTables<AllCSharpTypes>();
            db.Insertable(new AllCSharpTypes() { Ts = DateTime.Now, Boolean = true, Char = 'a', Decimal = Convert.ToDecimal(18.2), Int16 = 1, Int32 = 1, Int64 = 1, String = "2" }).ExecuteCommand();
            var list3 = db.Queryable<AllCSharpTypes>().ToList();
            db.CodeFirst.InitTables<AllCSharpTypes>();
        }

        private static void CodeFirst1(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<CodeFirst1>();
            db.Insertable(new CodeFirst1() { Boolean = true, Ts = DateTime.Now }).ExecuteCommand();
            var list = db.Queryable<CodeFirst1>().ToList(); 
        }
    }
}
