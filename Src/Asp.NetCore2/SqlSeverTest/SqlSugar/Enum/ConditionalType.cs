using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum ConditionalType
    {
        Equal=0,
        Like=1,
        GreaterThan =2,
        GreaterThanOrEqual = 3,
        LessThan=4,
        LessThanOrEqual = 5
    }
}
