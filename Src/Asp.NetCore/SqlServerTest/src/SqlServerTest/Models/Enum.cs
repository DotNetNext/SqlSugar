using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Models
{
    public enum SchoolEnum
    {
        HarvardUniversity = 0,
        UniversityOfOxford = 1
    }
    public class StudentEnum
    {
        public int Id { get; set; }
        public SchoolEnum SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public int TestId { get; set; }
    }
}
