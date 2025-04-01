using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.GaussDB
{
    internal class GaussDBInserttable<T> : PostgreSQLInserttable<T> where T : class, new()
    {
    }
}
