using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UnitSqlFunc
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.DbMaintenance.TruncateTable<Order>();
            db.Insertable(new Order() { Name = "a,b,c,aa,bb,cc" }).ExecuteCommand();
            db.Insertable(new Order() { Name = "e,a,c" }).ExecuteCommand();
            db.Insertable(new Order() { Name = "zac" }).ExecuteCommand();
            db.Insertable(new Order() { Name = "ab,az,ad" }).ExecuteCommand();
            NewMethod(db);
            NewMethod1(db);
            NewMethod2(db);
            NewMethod3(db);
            NewMethod4(db);
            NewMethod5(db);
            NewMethod6(db);
        }

        private static void NewMethod1(SqlSugarClient db)
        {
            var count = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "a")).Count();
            if (count != 2)
            {
                throw new Exception("unit error");
            }
        }

        private static void NewMethod(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "c")).Count();
            if (count2 != 2)
            {
                throw new Exception("unit error");
            }
        }
        private static void NewMethod2(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "az")).Count();
            if (count2 != 1)
            {
                throw new Exception("unit error");
            }
        }
        private static void NewMethod3(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "ab")).Count();
            if (count2 != 1)
            {
                throw new Exception("unit error");
            }
        }
        private static void NewMethod4(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "ad")).Count();
            if (count2 != 1)
            {
                throw new Exception("unit error");
            }
        }
        private static void NewMethod5(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "z")).Count();
            if (count2 != 0)
            {
                throw new Exception("unit error");
            }
        }
        private static void NewMethod6(SqlSugarClient db)
        {
            var count2 = db.Queryable<Order>().Where(it => SqlFunc.SplitIn(it.Name, "zac")).Count();
            if (count2 != 1)
            {
                throw new Exception("unit error");
            }
        }
    }
}
