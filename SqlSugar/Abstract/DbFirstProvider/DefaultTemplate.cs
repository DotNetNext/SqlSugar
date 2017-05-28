using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DefaultTemplate
    {
        public static string ClassTemplate = @"namespace {Namespace}
{
    {ClassPropertyDescription}
    {SugarTable}
    public class {ClassName}
    {
        public {ClassName}(){
           {Constructor}
        }
        {PropertyDescription}
        {Property}
    }
}";
        public static string ClassDescriptionTemplate = @"    /// <summary>
    ///{ClassDescription}
    /// </summary>\r\n";

        public static string PropertyTemplate = @"           {SugarColumn}
public {PropertyType} {PropertyName} {get;set;}\r\n";

        public static string PropertyDescriptionTemplate = @"/// <summary>
        /// Desc:{PropertyDescription}
        /// Default:{DefaultValue}
        /// Nullable:{IsNullable}
        /// </summary>\r\n";

        public static string ConstructorTemplate = @"           this.$PropertyName =Convert.To{PropertyType}(""{DefaultValue}"");\r\n";

        public static string NamespaceTemplate = @"using System;
        using System.Linq;
        using System.Text;\r\n";
    }
}
