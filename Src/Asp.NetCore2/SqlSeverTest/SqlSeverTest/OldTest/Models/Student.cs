using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;

namespace OrmTest.Models
{
    [SugarTable("STudent")]
    public class Student
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID",ColumnDescription ="主键")]
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [SugarColumn(IsIgnore=true,NoSerialize =true)]
        public int TestId { get; set; }
    }

    public class StudentTest
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        [SugarColumn(IsIgnore = true)]
        public int TestId { get; set; }
    }
}
