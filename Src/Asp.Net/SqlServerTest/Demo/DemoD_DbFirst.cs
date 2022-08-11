using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class DemoD_DbFirst
    {
        public static void Init()
        {
            Console.WriteLine();
            Console.WriteLine("#### DbFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });

            db.DbFirst.CreateClassFile("c:\\Demo\\1", "Models");


            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\2", "Models");


            db.DbFirst.Where(it => it.ToLower().StartsWith("view")).CreateClassFile("c:\\Demo\\3", "Models");


            db.DbFirst.Where(it => it.ToLower().StartsWith("view")).CreateClassFile("c:\\Demo\\4", "Models");


            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\5", "Models");


            db.DbFirst.IsCreateDefaultValue().CreateClassFile("c:\\Demo\\6", "Demo.Models");


            db.DbFirst
           //类
           .SettingClassTemplate(old => { return old;/*修改old值替换*/ })
           //类构造函数
           .SettingConstructorTemplate(old => { return old;/*修改old值替换*/ })
            .SettingNamespaceTemplate(old => {
                return old + "\r\nusing SqlSugar;"; //追加引用SqlSugar
             })
           //属性备注
           .SettingPropertyDescriptionTemplate(old => { return old;/*修改old值替换*/})

           //属性:新重载 完全自定义用配置
           .SettingPropertyTemplate((columns, temp, type) => {

               var columnattribute = "\r\n           [SugarColumn({0})]";
               List<string> attributes = new List<string>();
               if (columns.IsPrimarykey)
                   attributes.Add("IsPrimaryKey=true");
               if (columns.IsIdentity)
                   attributes.Add("IsIdentity=true");
               if (attributes.Count == 0)
               {
                   columnattribute = "";
               }
               return temp.Replace("{PropertyType}", type)
                            .Replace("{PropertyName}", columns.DbColumnName)
                            .Replace("{SugarColumn}", string.Format(columnattribute, string.Join(",", attributes)));
           })

          .CreateClassFile("c:\\Demo\\8");

            foreach (var item in db.DbMaintenance.GetTableInfoList())
            {
                string entityName = item.Name.ToUpper();/*Format class name*/
                db.MappingTables.Add(entityName , item.Name);
                foreach (var col in db.DbMaintenance.GetColumnInfosByTableName(item.Name))
                {
                    db.MappingColumns.Add(col.DbColumnName.ToUpper() /*Format class property name*/, col.DbColumnName, entityName);
                }
            }
            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\9", "Models");


            //Use Razor Template
            //db.DbFirst.UseRazorAnalysis(RazorFirst.DefaultRazorClassTemplate).CreateClassFile("");

            Console.WriteLine("#### DbFirst End ####");
        }
    }
}
