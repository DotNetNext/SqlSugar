using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class Unitdfafaaa
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Invoice>();
            var result =   db.Updateable<Invoice>(new Invoice() { })

                                     .WhereColumns(c => new { c.comp_code, c.adi_order, c.invoice_no, c.cust_po_no })

                                     .UpdateColumns(u => new { u.customer_original_po })

                                     .ExecuteCommandAsync().GetAwaiter().GetResult();
        }
        [SqlSugar.SugarTable("inv_master")]

        public class Invoice

        {

            [SqlSugar.SugarColumn(IsIdentity = true)]

            public long id { get; set; }

            public string comp_code { get; set; } = "a";

            public string adi_order { get; set; } = "a";

            public string invoice_no { get; set; } = "a";

            public string cust_po_no { get; set; } = "a";

            public string customer_original_po { get; set; } = "a";

        }
    }
}
