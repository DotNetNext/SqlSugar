using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom01
    {

        public static void Init()
        {
   
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = SqlSugar.DbType.PostgreSQL,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(UtilMethods.GetSqlString(SqlSugar.DbType.PostgreSQL,s,p));
            };
            try
            {
                db.CodeFirst.InitTables<User_Test001>();
            }
            catch 
            {
                //In the future to deal with
            }
            var list=db.Queryable<User_Test001>().ToList();


            var dt = new DataTable();
            dt.Columns.Add("ID",typeof(int));
            dt.Columns.Add("UserName");
            dt.TableName = "public.unitUser_Test001"; //设置表名
            var addRow = dt.NewRow();
            addRow["ID"] = 1;
            addRow["UserName"] = "a";
            dt.Rows.Add(addRow);//添加娄据

            var x1 = db.Storageable(dt).WhereColumns("id").ToStorage();//id作为主键

            string p1 = "p1";
            db.Queryable<Order>().Where(x11 => x11.Name + "a" == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == x11.Name + "a").ToList();
            db.Queryable<Order>().Where(x11 => "a" + x11.Name + p1 == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == "a" + x11.Name + p1).ToList();
            db.Queryable<Order>().Where(x11 => SqlFunc.ToString("a" + p1 + x11.Name) == x11.Name).ToList();
            db.Updateable<Order>()
                .SetColumns(x => x.Name == x.Name + "a")
                .Where(z => z.Id == 1)
                .ExecuteCommand();
            db.Updateable<Order>()
              .SetColumns(x => new Order() { Name = x.Name + "a" })
              .Where(z => z.Id == 1)
              .ExecuteCommand();
        }
        [SugarTable("public.unitUser_Test001")]
        public class User_Test001
        {

            public int ID { get; set; }
            public string UserName { get; set; }
        }
 
    }
}
