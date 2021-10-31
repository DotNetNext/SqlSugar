using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public partial class NewUnitTest {
        public static void SplitTest() 
        {
            var db = Db;

            db.CodeFirst.SplitTables().InitTables<SplitTestTable>();

            db.Queryable<SplitTestTable>().Where(it => it.Name.Contains("a")).SplitTable(tas => tas.Take(3)).ToList();

            var table2019=Db.SplitHelper<SplitTestTable>().GetTableName("2019-12-1");
            db.Queryable<SplitTestTable>().Where(it => it.Name.Contains("a")).SplitTable(tas => tas.InTableNames(table2019)).ToList();

            db.Queryable<SplitTestTable>().Where(it => it.Id == 1).SplitTable(tas => tas.Where(y => y.TableName.Contains("2019"))).ToList();
        }
    }

    [SplitTable(SplitType.Year)]
    [SugarTable("SplitTestTable_{year}{month}{day}")]
    public class SplitTestTable 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public long Id { get; set; }

        public string Name { get; set; }
        [SplitField]
        public string CreateTime { get; set; }
    }
}
