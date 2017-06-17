using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface ICodeFirst
    {
        SqlSugarClient Context { get; set; }
        ICodeFirst BackupTable(int maxBackupDataRows=int.MaxValue);
        void InitTables(string entitiesNamespace);
        void InitTables(string [] entitiesNamespaces);
        void InitTables(params Type [] entityTypes);
        void InitTables(Type entityType);
    }
}
