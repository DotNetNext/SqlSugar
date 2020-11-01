using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Test
{
    public class BugTest
    {
        public static void Init()
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = @"PORT=5433;DATABASE=x;HOST=localhost;PASSWORD=haosql;USER ID=postgres",
                DbType = DbType.Kdbndp,
                IsAutoCloseConnection = true,
                //MoreSettings = new ConnMoreSettings()
                //{
                //    PgSqlIsAutoToLower = true //我们这里需要设置为false
                //},
                InitKeyType = InitKeyType.Attribute,
            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                // Debug.WriteLine(sql);
            };

            Db.CodeFirst.InitTables(typeof(My12311x1));

            var id = Db.Insertable(new My12311x1 { Menu_Id = new int[] { 1, 2 } }).ExecuteReturnIdentity();
            var list = Db.Queryable<My12311x1>().InSingle(id);
            list.Menu_Id = new int[] { 3 };
            Db.Updateable(list).ExecuteCommand();
            list = Db.Queryable<My12311x1>().InSingle(id);
        }
    }

    public class My12311x1
    {
        [Key]
        [Display(Name = "ID")]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "int []", IsArray = true)]
        public int[] Menu_Id { get; set; }
    }

}
