using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DbFirstTemplate
    {
        #region Template
        public static string ClassTemplate = @"{using}
namespace {Namespace}
{
    {ClassDescription}{SugarTable}
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

        public static string ConstructorTemplate = @"           this.{$PropertyName} =Convert.To{PropertyType}(""{DefaultValue}"");\r\n";

        public static string UsingTemplate = @"using System;
using System.Linq;
using System.Text;"+"\r\n";
        #endregion

        #region Replace Key
        public const string KeySugarTable = "{SugarTable}";
        public const string KeyClassName = "{ClassName}";
        public const string KeyConstructor = "{Constructor}";
        public const string KeyPropertyDescription = "{PropertyDescription}";
        public const string KeyClassDescription = "{ClassDescription}";
        public const string KeySugarColumn = "{SugarColumn}";
        public const string KeyPropertyType = "{PropertyType}";
        public const string KeyPropertyName = "{PropertyName}";
        public const string KeyDefaultValue = "{DefaultValue}";
        public const string KeyIsNullable = "{IsNullable}";
        public const string KeyNamespace = "{Namespace}";
        public const string KeyUsing = "{using}";
        #endregion

        #region Replace Value
        public const string ValueSugarTable = "[SugarTable(\"{0}\")]";
        public const string ValueSugarCoulmn= "[SugarColumn({0})]";
        #endregion
    }
}
