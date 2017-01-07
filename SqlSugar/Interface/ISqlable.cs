using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial interface ISugarSqlable
    {
        SqlSugarClient Context { get; set; }
    }
}
