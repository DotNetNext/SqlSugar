using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom013
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            try
            {
                db.Ado.ExecuteCommand("create type week as enum('Sun','Mon','Tues','Wed','Thur','Fri','Sat');");
                db.Ado.ExecuteCommand(@"create table unitduty(
                    person text,
                    weekday week
                ); ");
            }
            catch  
            {
                 
            }
            db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings()
            {
                 TableEnumIsString = true,  
            };
            //db.Insertable(new unitduty()
            //{
            //    weekday = weekday.Fri,
            //    person="a"
            //}).ExecuteCommand();
            var list=db.Queryable<unitduty>().ToList();
        }
        public class unitduty 
        {
            public string person { get; set; }
            public weekday weekday { get;set; }
        }
        public enum weekday 
        {
            Fri,
            Thur,
            Sun
        }

    }
}
