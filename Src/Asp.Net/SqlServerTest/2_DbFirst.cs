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
        public static void Init()
        {
            var db = DbHelper.GetNewDb();

            //生成干净的类
            Demo1(db);

            //生成SqlSugar类
            Demo2(db);

            //支持string?
            Demo3(db);

            //格式化类名属性名文件名
            Demo4(db);

            //修改模版 
            Demo5(db);

            //条件过滤
            Demo6(db);
        }
        private static void Demo1(SqlSugarClient db)
        {
            db.DbFirst.CreateClassFile("c:\\Demo\\1", "Models");
        }
        private static void Demo2(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().CreateClassFile("c:\\Demo\\2", "Models");
        }
        private static void Demo3(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().StringNullable().CreateClassFile("c:\\Demo\\3", "Models");
        }

        private static void Demo4(SqlSugarClient db)
        {
            db.DbFirst
                .IsCreateAttribute()
                .FormatFileName(it => "File_" + it)
                .FormatClassName(it => "Class_" + it)
                .FormatPropertyName(it => "Property_" + it)
                .CreateClassFile("c:\\Demo\\4", "Models");
        }
         
        private static void Demo5(SqlSugarClient db)
        {
            db.DbFirst.IsCreateAttribute().Where(it => it.ToLower() == "userinfo001").CreateClassFile("c:\\Demo\\5", "Models");
        }

        private static void Demo6(SqlSugarClient db)
        {
            //禁用IsCreateAttribute不然会有冲突
            db.DbFirst
               //类
               .SettingClassTemplate(old => { return old;/*修改old值替换*/ })
               //类构造函数
               .SettingConstructorTemplate(old => { return old;/*修改old值替换*/ })
                .SettingNamespaceTemplate(old =>
                {
                    return old + "\r\nusing SqlSugar;"; //追加引用SqlSugar
                })
               //属性备注
               .SettingPropertyDescriptionTemplate(old => { return old;/*修改old值替换*/})

               //属性:新重载 完全自定义用配置
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
