using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitFADFASFAY
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitfaafafa>();
            db.DbMaintenance.TruncateTable<Unitfaafafa>();
            db.Insertable(new Unitfaafafa() { DateTime = Convert.ToDateTime("2020-01-01") }).ExecuteCommand();
            db.Insertable(new Unitfaafafa() { DateTime = Convert.ToDateTime("2020-01-02").AddHours(1) }).ExecuteCommand();
            db.Insertable(new Unitfaafafa() { DateTime = Convert.ToDateTime("2020-01-03").AddHours(1) }).ExecuteCommand();
            var cons = new List<IConditionalModel>() {
                new ConditionalModel(){ ConditionalType=ConditionalType.RangeDate, FieldName="DateTime", FieldValue="2020-01-01,2020-01-02" }
            };
           var data= db.Queryable<Unitfaafafa>().Where(cons).ToList();
            if (data.Count != 2) throw new Exception("unit error");
        }
        public class Unitfaafafa 
        {
            public DateTime DateTime { get; set; }
        }
    }
}
