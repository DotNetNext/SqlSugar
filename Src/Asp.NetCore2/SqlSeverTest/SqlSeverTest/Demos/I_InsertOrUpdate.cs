using OrmTest.Demo;
using OrmTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class InsertOrUpdate : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            var entity= db.Insertable<Student>(new Student() { Name = "abc" }).ExecuteReturnEntity();
             db.Saveable<Student>(entity).ExecuteReturnEntity();
            //UPDATE [STudent]  SET
            //[SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime WHERE[Id] = @Id
            db.Saveable<Student>(new Student() { Name="" }).ExecuteReturnEntity();
            // INSERT INTO[STudent]
            //([SchoolId],[Name],[CreateTime])
            // VALUES
            //(@SchoolId, @Name, @CreateTime); SELECT SCOPE_IDENTITY();


            db.Saveable<Student>(new Student() { Name = "" }).InsertColumns(it=>it.Name).ExecuteReturnEntity();
            db.Saveable<Student>(new Student() { Name = "" }).InsertIgnoreColumns(it => it.SchoolId).ExecuteReturnEntity();
            db.Saveable<Student>(entity).UpdateIgnoreColumns(it=>it.SchoolId).ExecuteReturnEntity();
            db.Saveable<Student>(entity).UpdateColumns(it=>new { it.Name,it.CreateTime }).ExecuteReturnEntity();



            db.Saveable<Student>(new List<Student>() {
                entity,
                new Student() { Name = "" }
            }).ExecuteCommand();

        }
    }
}
