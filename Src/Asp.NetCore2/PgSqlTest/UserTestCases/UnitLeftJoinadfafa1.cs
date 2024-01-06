using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SqlSugar;
namespace OrmTest
{ 
    public class UnitTestOneToOne
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                PgSqlIsAutoToLower = false,
                PgSqlIsAutoToLowerCodeFirst = false,
            };
            db.CodeFirst.InitTables<UnitSale, UnitBackSale>();
            db.DbMaintenance.TruncateTable<UnitSale, UnitBackSale>();

            for (int i = 0; i < 100; i++)
            {
                var sale = new UnitSale
                {
                    Name = $"UnitSale{i}",
                    IsOn = true,
                };
                int saleId = db.Insertable(sale).ExecuteReturnIdentity();

                var listBacksale = new List<UnitBackSale> {
                    new() {
                        Name = $"UnitBackSale{i}",
                        IsOn = true,
                        SaleId = saleId,
                        Money = i + new Random(DateTime.Now.Millisecond).NextDouble()
                    },
                    new() {
                        Name = $"UnitBackSale{i}",
                        IsOn = false,
                        SaleId = saleId,
                        Money = i + new Random(DateTime.Now.Millisecond).NextDouble()
                    }
                };
                int result = db.Insertable(listBacksale).ExecuteCommand();
            };

            var total = db.Queryable<UnitSale>()
                .IncludeLeftJoin(x => x.BackSale)
                .Where(x => x.BackSale.IsOn == true)
                .Sum(x => x.BackSale.Money); 
        }


    }
    public static class SqlSugarExtension
    {
        public static List<T> Where<T>(this T thisValue, Func<T, bool> whereExpression) where T : class, new()
        {
            return new List<T>() { thisValue };
        }
    }
    public class UnitSale
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }


        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(UnitBackSale.SaleId))]
        public UnitBackSale BackSale { get; set; }
    }

    public class UnitBackSale
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
        public double Money { get; set; }
    }
}


