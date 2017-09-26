using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar 
{
    public class OracleInsertable<T> : InsertableProvider<T> where T : class, new()
    {
    }
}
