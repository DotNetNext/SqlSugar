using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial interface IDMLBuilder
    {
        string SqlTemplate { get; }
        SqlSugarClient  Conext { get; set; }
        StringBuilder Sql { get; set; }
        string ToSqlString();
        void Clear();
    }
}
