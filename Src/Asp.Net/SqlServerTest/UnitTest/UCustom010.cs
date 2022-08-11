using OrmTest.UnitTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom010
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitasfa1sadfa>();
            var list = db.Queryable<Unitasfa1sadfa>()
               .Select(x => new
               {
                   x = Convert.ToBoolean(x.ItemId),
                   x1 = SqlFunc.ToBool(x.ItemId)
               })
               .ToList();
            db.CodeFirst.InitTables<UnitADSFA>();
            var list2 = db.Queryable<UnitADSFA>().GroupBy(it => new
            {
                it.A,
                x = it.A.ToString()
            }).ToList();

            var e = E.E1;
            var query = db.Queryable<T>().Where(p =>  (p.E | e)  ==  e );
            var sql1 = query.ToSql().Key;
            if (!sql1.Contains("=")) 
            {
                throw new Exception("unit error");
            }
        }

        public class Unitasfa1sadfa
        {
            [SugarColumn(IsPrimaryKey = true)]
            public Guid Id { get; set; }
            [SugarColumn(IsNullable = true)]
            public bool ItemId { get; set; }

        }

        public class UnitADSFA
        {
            public bool A { get; set; }
        }
       
            [Flags]

          public  enum E

            {

                E1 = 0x1,

                E2 = 0x2,

                E3 = 0x4,

            }
         


        public class T

            {

                public E E { get; set; }

            }
 
    }
}