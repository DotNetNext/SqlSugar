using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.Abstract.DbFirstProvider
{
    public class DefaultTemplate
    {
        public static string ClassTemplate { get; set; }
        public static string ClassDescriptionTemplate { get; set; }
        public static string PropertyTemplate { get; set; }
        public static string PropertyDescriptionTemplate { get; set; }
        public static string ConstructorTemplate { get; set; }
        public static string NamespaceTemplate { get; set; }
    }
}
