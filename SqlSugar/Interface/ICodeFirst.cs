using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface ICodeFirst
    {
        SqlSugarClient Context { get; set; }
        ICodeFirst IsBackupTable(bool isBackupTable = true);
        ICodeFirst IsBackupData(bool isBackupData = true);
        ICodeFirst IsDeleteNoExistColumn(bool isDeleteNoExistColumn = true);
        void InitTables(string entitiesNamespace);
        void InitTables(string [] entitiesNamespaces);
        void InitTables(Type [] entityTypes);
        void InitTables(Type entityType);
    }
}
