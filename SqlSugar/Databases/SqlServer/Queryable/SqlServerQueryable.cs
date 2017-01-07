using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SqlServerQueryable<T>:QueryableProvider<T> where T:class,new()
    {
    
    }
}
