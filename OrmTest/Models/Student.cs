using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Linq.Expressions;
using Chloe.Entity;

namespace OrmTest.Models
{
    [SugarTable("STudent")]
    public class Student
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "ID")]
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        [SugarColumn(IsIgnore=true)]
        [NotMappedAttribute]
        public int TestId { get; set; }
    }
}
