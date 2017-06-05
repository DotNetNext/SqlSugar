using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface ICodeFirst
    {
        SqlSugarClient Context { get; set; }
        ICodeFirst IsBackupTable(bool isCreateTable = false);
        ICodeFirst IsBackupData(bool isCreateTable = true);
        ICodeFirst IsDeleteNoExistColumn(bool isDeleteNoExistColumn = true);
        ICodeFirst Where(Func<string, bool> filterMethod);
        void InitTables(string entitiesNamespace);
        void InitTables(string [] entitiesNamespaces);
        void InitTables(Type [] entityTypes);
        void InitTables(Type entityType);
    }
}
