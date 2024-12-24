using Microsoft.Identity.Client;
using Npgsql;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unita1ddys
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;//用于创建schema和删除schema
            try
            {

                db.Ado.ExecuteCommand("CREATE SCHEMA testcm");
            }
            catch (Exception)
            { 
            }
            var newdb = NewUnitTest.Db.CopyNew();//新db指定schema实现crud
            newdb.CurrentConnectionConfig.ConnectionString = Config.ConnectionString + ";searchpath=testcm";
            newdb.CodeFirst.InitTables<Unitsdfafayas>();
            for (int i = 0; i < 3; i++)
            {
                Unitsdfafayas entity = new Unitsdfafayas()
                {
                    Id = Guid.NewGuid().ToString(),
                    Annex = new List<string>() { "12312321332" }
                };
                newdb.Insertable(entity).ExecuteCommand();
            }
            var communityEntities = newdb.Queryable<Unitsdfafayas>().ToList();

            var x = newdb.Storageable(communityEntities).ToStorage();
            x.AsInsertable.ExecuteCommand();
            x.AsUpdateable.ExecuteCommand();  
        }
        public class Unitsdfafayas
        {
            [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
            public string Id { get; set; }

            [SugarColumn(ColumnName = "annex", ColumnDataType = "JSONB", IsJson = true)]
            public List<string> Annex { get; set; }
        }
    }
}
