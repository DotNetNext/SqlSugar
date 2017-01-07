using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface ICodeFirst
    {
        SqlSugarClient Context { get; set; }
    }
}
