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


            db.DbFirst. SettingClassTemplate(old => { return old;})
                       .SettingNamespaceTemplate(old =>{ return old;})
                       .SettingPropertyDescriptionTemplate(old =>
                        {
                            return @"           /// <summary>
                          /// Desc_New:{PropertyDescription}
                          /// Default_New:{DefaultValue}
                                /// Nullable_New:{IsNullable}
                                /// </summary>";
                        })
                        .SettingPropertyTemplate(old =>{return old;})
                        .SettingConstructorTemplate(old =>{return old; })
                   .CreateClassFile("c:\\Demo\\7");



            foreach (var item in db.DbMaintenance.GetTableInfoList())
            {
                string entityName = item.Name.ToUpper();/*Format class name*/
                db.MappingTables.Add(entityName , item.Name);
                foreach (var col in db.DbMaintenance.GetColumnInfosByTableName(item.Name))
                {
                    db.MappingColumns.Add(col.DbColumnName.ToUpper() /*Format class property name*/, col.DbColumnName, entityName);
                }
            }
            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\8", "Models");


            //Use Razor Template
            //db.DbFirst.UseRazorAnalysis(RazorFirst.DefaultRazorClassTemplate).CreateClassFile("");

            Console.WriteLine("#### DbFirst End ####");
        }
    }
}
