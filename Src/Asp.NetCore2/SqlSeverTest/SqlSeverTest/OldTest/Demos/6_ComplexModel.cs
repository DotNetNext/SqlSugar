using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;
using OrmTest.Demo;

namespace OrmTest.Demo
{
    public class ComplexModel : DemoBase
    {
        public static void Init()
        {
            var db = GetInstance();
            db.Insertable(new CMStudent() { SchoolId = 1, Name = "xx1" }).ExecuteCommand();
            var students = db.Queryable<CMStudent>().Take(10).ToList();
            if (students != null)
            {
                foreach (var item in students)
                {
                    Console.WriteLine(item.SchoolName);
                    if (item.SchoolSingle != null)
                    {
                        Console.WriteLine(item.SchoolSingle.Name);
                    }
                    if (item.SchoolList != null)
                    {
                        Console.WriteLine(item.SchoolList.Count);
                    }
                }
            }

            db.Insertable(new CMStudent() { Name="xx" }).ExecuteCommand();
        }
    }

    [SugarTable("Student")]
    public class CMStudent : ModelContext
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SchoolId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string SchoolName
        {
            get
            {
                if (this.SchoolSingle != null)
                    return this.SchoolSingle.Name;
                else
                    return null;
            }
        }

        [SugarColumn(IsIgnore = true)]
        public CMSchool SchoolSingle
        {
            get
            {
                return base.CreateMapping<CMSchool>().Single(it => it.Id == this.SchoolId);
            }
        }

        [SugarColumn(IsIgnore = true)]
        public List<CMSchool> SchoolList
        {
            get
            {
                return base.CreateMapping<CMSchool>().Where(it => it.Id == this.SchoolId).Take(2).ToList();
            }
        }
    }

    [SugarTable("School")]
    public class CMSchool
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}