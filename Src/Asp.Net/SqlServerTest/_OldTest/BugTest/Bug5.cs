using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Test
{
    public class BugTest5
    {
        public static void Init()
        {
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
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
                Console.WriteLine(sql);
            };
          
            Db.CodeFirst.InitTables(typeof(Base_MenuList));
            Db.CodeFirst.InitTables(typeof(aaaa));
            Db.CodeFirst.InitTables(typeof(bbbbb));

        }
        [SugarTable("xxxx.Base_MenuList")]

        public class Base_MenuList

        {

            /// <summary>

            /// 菜单主键Id

            /// </summary>

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "菜单主键Id")]

            public int MenuID { get; set; }



            /// <summary>

            /// 名称

            /// </summary>

            [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "名称")]

            public string MenuName { get; set; }

        }

        [SugarTable("xxxx1x")]

        public class aaaa

        {

            /// <summary>

            /// 菜单主键Id

            /// </summary>

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "菜单主键Id")]

            public int MenuID { get; set; }



            /// <summary>

            /// 名称

            /// </summary>

            [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "名称")]

            public string MenuName { get; set; }

        }
        [SugarTable("bbbbb")]

        public class bbbbb

        {

            /// <summary>

            /// 菜单主键Id

            /// </summary>

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "菜单主键Id")]

            public int MenuID { get; set; }



            /// <summary>

            /// 名称

            /// </summary>

            [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "名称")]

            public string MenuName { get; set; }

        }
    }
   

}
