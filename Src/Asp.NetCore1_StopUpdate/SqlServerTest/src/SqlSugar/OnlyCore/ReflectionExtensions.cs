using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SqlSugar
{
    public static class ReflectionCore
    {
        public static Assembly Load(string name)
        {
            return Assembly.Load(new AssemblyName(PubConst.AssemblyName));
        }
    }
}
