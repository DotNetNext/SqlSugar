using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.Models
{
    public class ViewModelStudent:Student
    {

    }
    public class ViewModelStudent2
    {
        public string Name { get; set; }
        public Student Student { get; set; }
    }
    public class ViewModelStudent3: Student
    {
        public string SchoolName { get; set; }
        public string School_Name { get; set; }
        public string ScId { get; set; }
        public School School { get; set; }
        public List<School> Schools { get; set; }
    }
}
