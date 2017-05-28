using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbFirst
    {
        SqlSugarClient Context { get; set; }
        IDbFirst SettingClassTemplate(Func<string> func);
        IDbFirst SettingClassDescriptionTemplate(Func<string> func);
        IDbFirst SettingPropertyTemplate(Func<string> func);
        IDbFirst SettingPropertyDescriptionTemplate(Func<string> func);
        IDbFirst SettingConstructorTemplate(Func<string> func);
        IDbFirst Where(params string[] objectNames);
        IDbFirst Where(Func<string,bool> func);
        IDbFirst Where(DbObjectType dbObjectType);
        List<SchemaInfo> GetSchemaInfoList { get; }
        void CreateClassFile();
        void ToClassStringList();
    }
}
