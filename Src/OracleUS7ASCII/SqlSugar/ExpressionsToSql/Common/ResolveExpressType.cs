using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public enum ResolveExpressType
    {
        None = 0,
        WhereSingle = 1,
        WhereMultiple = 2,
        SelectSingle=3,
        SelectMultiple=4,
        FieldSingle=5,
        FieldMultiple=7,
        Join=8,
        ArraySingle=9,
        ArrayMultiple = 10,
        Update =11

    }
}
