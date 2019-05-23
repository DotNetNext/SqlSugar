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
        public string Name { get; set; }
    }
}
