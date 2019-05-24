using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class TestTree
    {
        [SqlSugar.SugarColumn(ColumnDataType = "hierarchyid")]
        public string TreeId { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "Geography")]
        public string GId { get; set; }
        public string Name { get; set; }
    }
}
