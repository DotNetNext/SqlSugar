using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OracleUpdateable<T>:UpdateableProvider<T> where T:class,new()
    {

    }
}
