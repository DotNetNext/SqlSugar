using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class CustomProvider
    {
        internal static ISugarQueryable<T> GetQueryable<T>(string dbName, string dllName)
        {
            throw new NotImplementedException();
        }
    }
}
