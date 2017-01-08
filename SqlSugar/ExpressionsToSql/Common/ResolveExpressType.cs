using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum ResolveExpressType
    {
        WhereSingle = 0,
        WhereMultiple = 1,
        SelectSingle=2,
        SelectMultiple=3,
        FieldSingle=4,
        FieldMultiple=5
    }
}
