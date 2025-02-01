using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unityadfasasdfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db; 
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                 IsCorrectErrorSqlParameterName = true
            };
            try
            { 
                db.Ado.ExecuteCommand("CREATE TABLE [unitadfaaaafa2](\r\n[打印费(折扣等)] varchar(255) NOT NULL,id varchar(50)  )");
            }
            catch (Exception)
            {
                 
            }  
            db.Updateable<Unitadfa121>()
                .SetColumns(it =>it.a==it.a )
                .Where(it=>it.id=="a")
                .ExecuteCommand();
            Console.WriteLine("Start");
        }
        [SugarTable("unitadfaaaafa2")]
        public class Unitadfa121 
        {
            [SqlSugar.SugarColumn(ColumnName ="打印费(折扣等)")]
            public string a { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
            public string id { get; set; }
        }
    }
}
