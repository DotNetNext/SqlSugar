using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DbTableInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DbObjectType DbObjectType { get; set; }
    }
}
