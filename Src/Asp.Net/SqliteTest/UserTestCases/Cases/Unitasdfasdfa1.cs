using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitasdfasdfa1
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.MoreSettings = new SqlSugar.ConnMoreSettings()
            {
                 IsCorrectErrorSqlParameterName=true
            };
            db.CodeFirst.InitTables<UnitTERTAFA>();
            db.Insertable(new UnitTERTAFA()
            {
                Name = "a"
            }).ExecuteCommand();
            var dt=db.Queryable<UnitTERTAFA>().Take(2) .ToDataTable();
            dt.Columns.RemoveAt(0);
            db.Fastest<DataTable>().AS("UnitTERTAFA").BulkCopy(dt);
        }
    }
    public class UnitTERTAFA 
    {
        [SqlSugar.SugarColumn(ColumnName = "id 1",IsPrimaryKey =true,IsIdentity =true)]
        public int id { get; set; }
        [SqlSugar.SugarColumn(ColumnName ="Name 1")]
        public string Name { get; set; }
        
    }
}
