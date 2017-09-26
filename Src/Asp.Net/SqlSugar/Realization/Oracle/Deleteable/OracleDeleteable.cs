using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OracleDeleteable<T>:DeleteableProvider<T> where T:class,new()
    {
    }
}
