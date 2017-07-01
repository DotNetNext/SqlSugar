using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// In order to be compatible with .NET CORE, make sure that the two versions are consistent in syntax
    /// </summary>
    public static class CompatibleExtensions
    {
        public static Type GetTypeInfo(this Type typeInfo)
        {
            return typeInfo;
        }
    }
}
