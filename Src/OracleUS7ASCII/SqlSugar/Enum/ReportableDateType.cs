using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public enum ReportableDateType
    {
        MonthsInLast1years=0,
        MonthsInLast3years=1,
        MonthsInLast10years=2,
        years1=3,
        years3=4,
        years10=5
    }
}
