using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyModel;

namespace SqlSugar
{
    public class CoreMethod
    {
        public static Assembly LoadAssembly(string name)
        {
            var deps = DependencyContext.Default;
            var assemblyies = deps.CompileLibraries.Where(it => it.Name == name).ToList();
            return assemblyies.Any() ? Assembly.Load(new AssemblyName(assemblyies.First().Name)) : null;
        }
    }
}
