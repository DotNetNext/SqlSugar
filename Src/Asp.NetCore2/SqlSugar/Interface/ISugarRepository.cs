using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISugarRepository
    {
        ISqlSugarClient Context { get; set; }
    }
}
