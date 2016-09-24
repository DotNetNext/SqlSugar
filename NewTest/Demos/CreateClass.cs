using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;

namespace NewTest.Demos
{
    //生成实体函数
    public class CreateClass : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动CreateClass.Init");
            using (var db = SugarDao.GetInstance())
            {
                //可以结合别名表，请看别名表的用法
                //db.SetMappingTables(mappingTableList);

                db.ClassGenerating.CreateClassFiles(db, ("e:/TestModels"), "Models");

                //只生成student和school表的实体
                db.ClassGenerating.CreateClassFilesByTableNames(db, "e:/TestModels2", "Models", new string[] { "student", "school" });

                //根据表名生成class字符串
                var str = db.ClassGenerating.TableNameToClass(db, "Student");

                var dynamicToClassStr = db.ClassGenerating.DynamicToClass(new { id = 1 }, "dyName");

                //根据SQL语句生成class字符串
                var str2 = db.ClassGenerating.SqlToClass(db, "select top 1 * from Student", "student");

                //改变值（lassTemplate.ItemTemplate=XXXX）可以自定义格式
                var tempItem = ClassTemplate.ItemTemplate;//例如可以在生成的实体添加默认构造函数给指定的字段赋默认值或者公司信息等
                var temp = ClassTemplate.Template;



                //设置新格式模板
                //主键Guid.New(),
                //CreateTime为DateTime.Now 
                //IsRemove=0
                //UpdateTime为DateTime.Now 
                ClassTemplate.Template = "using System;\r\nusing System.Linq;\r\nusing System.Text;\r\n\r\nnamespace $namespace\r\n{\r\n    public class $className\r\n    {\r\n        public $className() \r\n        { \r\n            this.CreateTime = DateTime.Now;\r\n            this.  = 0;\r\n            this.UpdateTime=DateTime.Now;\r\n            this.$primaryKeyName=Guid.NewGuid().ToString(\"N\").ToUpper();\r\n        }\r\n        $foreach\r\n    }\r\n}\r\n";

                //新格式的实体字符串
                var str3 = db.ClassGenerating.TableNameToClass(db, "Student");

            }
        }
    }
}
