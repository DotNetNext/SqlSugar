using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UnitAny
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UintAnyModel>(); 
            var id=db.Insertable(new UintAnyModel() { 
             Name="a"
            }).ExecuteReturnIdentity();
            if (db.Queryable<UintAnyModel>().Any(it => it.Id == id) == false) 
            {
                throw new Exception("unit error");
            }
            if (db.Queryable<UintAnyModel>().Any(it => it.Id == 971151111) == true)
            {
                throw new Exception("unit error");
            }
            if (db.Queryable<UintAnyModel>().AnyAsync(it => it.Id == id).GetAwaiter().GetResult() == false)
            {
                throw new Exception("unit error");
            }
            if (db.Queryable<UintAnyModel>().AnyAsync(it => it.Id == 971151111).GetAwaiter().GetResult() == true)
            {
                throw new Exception("unit error");
            }
        }
        public class UintAnyModel
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
