using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Tree
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public Tree Parent { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public List<Tree> Child { get; set; }
    }
}
