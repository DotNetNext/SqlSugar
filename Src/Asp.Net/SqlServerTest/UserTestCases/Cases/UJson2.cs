using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    internal class UJson2
    {
        public static void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
            db.CodeFirst.InitTables<Tb1>();

            var tb = new Tb1()
            {
                Title = "标题这里中文正常",
                JsonData = new JsonClass() { text = "这里中文乱码" }
            };
            db.Insertable(tb).ExecuteCommand();
            Console.WriteLine(db.Insertable(tb).ToSqlString());
            ;

        }
        public class Tb1
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long Id { get; set; }

            [SugarColumn(ColumnDataType = "Nvarchar(255)")]
            public string Title { get; set; }

            [SugarColumn(ColumnDataType = "Nvarchar(255)", IsJson = true)]
            public JsonClass JsonData { get; set; }
        }
        public class JsonClass
        {
            public string text { get; set; }
        }
    }
}
