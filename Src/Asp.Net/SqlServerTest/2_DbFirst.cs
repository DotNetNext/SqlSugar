using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class _2_DbFirst
    {
        /// <summary>
        /// 初始化方法，包含各种DbFirst操作的演示
        /// Initialization method containing demonstrations of various DbFirst operations
        /// </summary>
        public static void Init()
        {
            var db = DbHelper.GetNewDb();

            // 生成干净的实体类文件
            // Generate clean entity class files
            Demo1(db);

            // 生成带有SqlSugar特性的实体类文件
            // Generate entity class files with SqlSugar attributes
            Demo2(db);

            // 支持字符串类型的Nullable特性
            // Support String Nullable attribute
            Demo3(db);

            // 格式化类名、属性名和文件名
            // Format class names, property names, and file names
            Demo4(db);

            // 条件过滤生成实体类文件
            // Generate entity class files with condition filtering
            Demo5(db);

            // 修改模版生成实体类文件（禁用IsCreateAttribute，避免冲突）
            // Generate entity class files with modified templates (disable IsCreateAttribute to avoid conflicts)
            Demo6(db);
        }

        /// <summary>
        /// 生成干净的实体类文件
        /// Generate clean entity class files
        /// </summary>
        private static void Demo1(SqlSugarClient db)
        {
            db.DbFirst.CreateClassFile("c:\\Demo\\1", "Models");
        }

        /// <summary>
        /// 生成带有SqlSugar特性的实体类文件
        /// Generate entity class files with SqlSugar attributes
        /// </summary>
        private static void Demo2(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\2", "Models");
        }

        /// <summary>
        /// 支持字符串类型的Nullable特性
        /// Support String Nullable attribute
        /// </summary>
        private static void Demo3(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().StringNullable().CreateClassFile("c:\\Demo\\3", "Models");
        }

        /// <summary>
        /// 格式化类名、属性名和文件名
        /// Format class names, property names, and file names
        /// </summary>
        private static void Demo4(SqlSugarClient db)
        {
            db.DbFirst
                .IsCreateAttribute()
                .FormatFileName(it => "File_" + it)
                .FormatClassName(it => "Class_" + it)
                .FormatPropertyName(it => "Property_" + it)
                .CreateClassFile("c:\\Demo\\4", "Models");
        }

        /// <summary>
        /// 条件过滤生成实体类文件
        /// Generate entity class files with condition filtering
        /// </summary>
        private static void Demo5(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().Where(it => it.ToLower() == "userinfo001").CreateClassFile("c:\\Demo\\5", "Models");
        }

        /// <summary>
        /// 修改模版生成实体类文件（禁用IsCreateAttribute，避免冲突）
        /// Generate entity class files with modified templates (disable IsCreateAttribute to avoid conflicts)
        /// </summary>
        private static void Demo6(SqlSugarClient db)
        {
            db.DbFirst
               // 类
               .SettingClassTemplate(old => { return old;/* 修改old值替换 */ })
               // 类构造函数
               .SettingConstructorTemplate(old => { return old;/* 修改old值替换 */ })
                .SettingNamespaceTemplate(old =>
                {
                    return old + "\r\nusing SqlSugar;"; // 追加引用SqlSugar
                })
               // 属性备注
               .SettingPropertyDescriptionTemplate(old => { return old;/* 修改old值替换 */})

               // 属性:新重载 完全自定义用配置
               .SettingPropertyTemplate((columns, temp, type) =>
               {

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

               .CreateClassFile("c:\\Demo\\6");
        }
    }
}
 