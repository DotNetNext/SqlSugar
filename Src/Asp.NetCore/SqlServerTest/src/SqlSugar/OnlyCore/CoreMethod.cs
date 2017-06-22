using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class CoreMethod
    {
        public static Assembly LoadAssembly(string name)
        {
            return  Assembly.Load(new AssemblyName(PubConst.AssemblyName)) ;
        }
    }
}
