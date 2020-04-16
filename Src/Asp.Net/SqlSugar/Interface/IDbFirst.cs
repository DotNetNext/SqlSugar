using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbFirst
    {
        ISqlSugarClient Context { get; set; }
        IDbFirst SettingClassTemplate(Func<string, string> func);
        IDbFirst SettingClassDescriptionTemplate(Func<string, string> func);
        IDbFirst SettingPropertyTemplate(Func<string, string> func);
        IDbFirst SettingPropertyDescriptionTemplate(Func<string, string> func);
        IDbFirst SettingConstructorTemplate(Func<string, string> func);
        IDbFirst SettingNamespaceTemplate(Func<string, string> func);
        RazorFirst UseRazorAnalysis(string razorClassString, string classNamespace = "Models");
        IDbFirst IsCreateAttribute(bool isCreateAttribute = true);
        IDbFirst IsCreateDefaultValue(bool isCreateDefaultValue = true);
        IDbFirst Where(params string[] objectNames);
        IDbFirst Where(Func<string, bool> func);
        IDbFirst WhereColumns(Func<string, bool> func);
        IDbFirst Where(DbObjectType dbObjectType);
        void CreateClassFile(string directoryPath, string nameSpace = "Models");
        Dictionary<string, string> ToClassStringList(string nameSpace = "Models");
        void Init();
    }
}
