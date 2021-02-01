using PerformanceTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PerformanceTest.Models2
{
    public class Group
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; } //Id、GroupId、Group_id
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<User> AUsers { get; set; }
    }
}
namespace PerformanceTest.Models
{
    public class Group
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; } //Id、GroupId、Group_id
        public string Name { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public ICollection<User> AUsers { get; set; }
    }

    public class User
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; } //Id、UserId、User_id
        public int AGroupId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public Group AGroup { get; set; }
    }
}
