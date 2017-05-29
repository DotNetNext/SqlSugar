using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class DbFirst : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            //Create all class
            db.DbFirst.CreateClassFile("c:\\Demo\\1");

            //Create student calsss
            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\2");
            //Where(array)

            //Mapping name
            db.MappingTables.Add("ClassStudent", "Student");
            db.MappingColumns.Add("NewId", "Id", "ClassStudent");
            db.DbFirst.Where("Student").CreateClassFile("c:\\Demo\\3");

            //Remove mapping
            db.MappingTables.Clear();

            //Create class with default value
            db.DbFirst.IsCreateDefaultValue().CreateClassFile("c:\\Demo\\4", "Demo.Models");


            //Mapping and Attribute
            db.MappingTables.Add("ClassStudent", "Student");
            db.MappingColumns.Add("NewId", "Id", "ClassStudent");
            db.DbFirst.IsCreateAttribute().Where("Student").CreateClassFile("c:\\Demo\\5");


            //Remove mapping
            db.MappingTables.Clear();
            db.MappingColumns.Clear();

            //Custom format,Change old to new
            db.DbFirst.
                SettingClassTemplate(old =>
                {
                    return old;
                })
               .SettingNamespaceTemplate(old =>
               {
                   return old;
               })
               .SettingPropertyDescriptionTemplate(old =>
               {
                   return @"           /// <summary>
           /// Desc_New:{PropertyDescription}
           /// Default_New:{DefaultValue}
           /// Nullable_New:{IsNullable}
           /// </summary>";
               })
                .SettingPropertyTemplate(old =>
                {
                    return old;
                })
                .SettingConstructorTemplate(old =>
                {
                    return old;
                })
            .CreateClassFile("c:\\Demo\\6");
        }
    }
}