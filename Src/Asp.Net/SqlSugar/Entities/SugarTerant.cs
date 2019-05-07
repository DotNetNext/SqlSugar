using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SugarTenant
    {
        public ISqlSugarClient Context { get; set; }
        public ConnectionConfig ConnectionConfig { get; set; }
    }
}
