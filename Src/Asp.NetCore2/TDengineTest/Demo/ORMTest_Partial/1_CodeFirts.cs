using OrmTest;
using SqlSugar;
using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void CodeFirst(SqlSugarClient db)
        {
            //建库
            db.DbMaintenance.CreateDatabase();
            
            //简单建表
            CodeFirst1(db);

            //简单建表
            CodeFirst2(db);

            CodeFirst4(db);

            //多标签建表
            CodeFirst5(db);

            db.CodeFirst.InitTables<TDHistoryValue>();

            //更多建表用例
            db.CodeFirst.InitTables<CodeFirst03>();
            db.Insertable(new CodeFirst03()
            {
                Ts = DateTime.Now,
                Boolean = true,
                Char = 'a',
                Decimal = Convert.ToDecimal(18.2),
                Int16 = 16,
                Int32 = 32,
                Int64 = 64,
                String = "string",
                SByte=3,
                Byte = 2,
                Decimal2 = Convert.ToDecimal(18.3),
                Double = Convert.ToDouble(18.44),
                Float = Convert.ToSingle(18.45),
                String2 = "2",
                 UInt16=116,
                  UInt32=332,
                   UInt64=664
            }).ExecuteCommand();
            var dt = db.Ado.GetDataTable("select * from  CodeFirst03 ");
            var list3 = db.Queryable<CodeFirst03>().ToList();
        }

        private static void CodeFirst5(SqlSugarClient db)
        {
              
               db.CodeFirst.InitTables<CodeFirstTags33>();

                db.Insertable(new CodeFirstTags33()
                {
                    Boolean = true,
                    Ts = DateTime.Now
                }).ExecuteCommand();

                  
                   db.CodeFirst.InitTables<CodeFirstTags44>();

                db.Insertable(new CodeFirstTags44()
                {
                    Boolean = true,
                    Ts = DateTime.Now
                }).ExecuteCommand();


        }

        private static void CodeFirst1(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<CodeFirst01>();
            db.Insertable(new CodeFirst01() { Boolean = true, Ts = DateTime.Now }).ExecuteCommand();
            var list = db.Queryable<CodeFirst01>().ToList();
        }
        private static void CodeFirst2(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<CodeFirst04>();
            db.Insertable(new CodeFirst04() { Boolean = true, Ts = DateTime.Now }).ExecuteCommand();
            var list = db.Queryable<CodeFirst04>().ToList();
        }
        private static void CodeFirst4(SqlSugarClient db) 
        {
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                 PgSqlIsAutoToLower = false,
                 PgSqlIsAutoToLowerCodeFirst = false,
            };
            db.CodeFirst.InitTables<CodeFirst05>();
            db.Insertable(new CodeFirst05() { Boolean = true, Ts = DateTime.Now }).ExecuteCommand();
            var list = db.Queryable<CodeFirst05>().ToList();
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                PgSqlIsAutoToLower = true,
                PgSqlIsAutoToLowerCodeFirst = true,
            };
        }
    }
}
