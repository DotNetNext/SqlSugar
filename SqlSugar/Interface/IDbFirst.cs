using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbFirst
    {
        SqlSugarClient Context { get; set; }
    }
}
