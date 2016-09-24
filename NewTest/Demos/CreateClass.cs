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
                var tempItem = ClassTemplate.ItemTemplate;
                var temp = ClassTemplate.Template;

            }
        }
    }
}
