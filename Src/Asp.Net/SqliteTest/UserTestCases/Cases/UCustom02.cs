using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F9.DataEntity.Entity;
using SqlSugar;
namespace  OrmTest
{
    public class UCustom02
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            foreach (DbTableInfo table in db.DbMaintenance.GetTableInfoList())
            {
                string entityName = table.Name.ToUpper();
                db.MappingTables.Add(entityName, table.Name);
                foreach (DbColumnInfo col in db.DbMaintenance.GetColumnInfosByTableName(table.Name))
                {
                    string propertyName =  col.DbColumnName.ToUpper();
                    //Console.WriteLine("propertyName=" + propertyName + ", column=" + col.DbColumnName);
                    db.MappingColumns.Add(propertyName, col.DbColumnName, entityName);
                }
            }

            // Console.WriteLine(db.Utilities.SerializeObject(db.MappingColumns));

            Check.Exception(!db.Queryable<Order>().ToClassString(null).Contains("Name"), "unit error");
        }
    }
}
