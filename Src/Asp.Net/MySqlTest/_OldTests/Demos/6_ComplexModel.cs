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
            var students = db.Queryable<CMStudent>().ToList();
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
                return base.CreateMapping<CMSchool>().Where(it => it.Id == this.SchoolId).ToList();
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