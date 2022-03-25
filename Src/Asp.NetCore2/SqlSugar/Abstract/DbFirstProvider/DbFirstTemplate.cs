using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DbFirstTemplate
    {
        #region Template
        public static string ClassTemplate =   "{using}\r\n" +
                                               "namespace {Namespace}\r\n" +
                                               "{\r\n" +
                                               "{ClassDescription}{SugarTable}\r\n" +
                                                ClassSpace+ "public partial class {ClassName}\r\n" +
                                                ClassSpace + "{\r\n" +
                                                PropertySpace + "public {ClassName}(){\r\n\r\n" +
                                                "{Constructor}\r\n" +
                                                PropertySpace + "}\r\n" +
                                                "{PropertyName}\r\n" +
                                                 ClassSpace + "}\r\n" +
                                                "}\r\n";
        public static string ClassDescriptionTemplate =
                                                ClassSpace + "///<summary>\r\n" +
                                                ClassSpace + "///{ClassDescription}" +
                                                ClassSpace + "///</summary>";

        public static string PropertyTemplate = PropertySpace + "{SugarColumn}\r\n" +
                                                PropertySpace + "public {PropertyType} {PropertyName} {get;set;}\r\n";

        public static string PropertyDescriptionTemplate =
                                                PropertySpace + "/// <summary>\r\n" +
                                                PropertySpace + "/// Desc:{PropertyDescription}\r\n" +
                                                PropertySpace + "/// Default:{DefaultValue}\r\n" +
                                                PropertySpace + "/// Nullable:{IsNullable}\r\n" +
                                                PropertySpace + "/// </summary>";

        public static string ConstructorTemplate = PropertySpace + " this.{PropertyName} ={DefaultValue};\r\n";

        public static string UsingTemplate =   "using System;\r\n" +
                                               "using System.Linq;\r\n" +
                                               "using System.Text;" + "\r\n";
        #endregion

        #region Replace Key
        public const string KeyUsing = "{using}";
        public const string KeyNamespace = "{Namespace}";
        public const string KeyClassName = "{ClassName}";
        public const string KeyIsNullable = "{IsNullable}";
        public const string KeySugarTable = "{SugarTable}";
        public const string KeyConstructor = "{Constructor}";
        public const string KeySugarColumn = "{SugarColumn}";
        public const string KeyPropertyType = "{PropertyType}";
        public const string KeyPropertyName = "{PropertyName}";
        public const string KeyDefaultValue = "{DefaultValue}";
        public const string KeyClassDescription = "{ClassDescription}";
        public const string KeyPropertyDescription = "{PropertyDescription}";
        #endregion

        #region Replace Value
        public const string ValueSugarTable = "\r\n"+ClassSpace+"[SugarTable(\"{0}\")]";
        public const string ValueSugarCoulmn = "\r\n"+PropertySpace+"[SugarColumn({0})]";
        #endregion

        #region Space
        public const string PropertySpace = "           ";
        public const string ClassSpace = "    "; 
        #endregion
    }
}
