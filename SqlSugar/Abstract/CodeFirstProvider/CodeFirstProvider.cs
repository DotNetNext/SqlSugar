using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial class CodeFirstProvider : ICodeFirst
    {
        public virtual SqlSugarClient Context { get; set; }

        public void InitTables(Type entityType)
        {
            throw new NotImplementedException();
        }

        public void InitTables(Type[] entityTypes)
        {
            throw new NotImplementedException();
        }

        public void InitTables(string entitiesNamespace)
        {
            throw new NotImplementedException();
        }

        public void InitTables(string[] entitiesNamespaces)
        {
            throw new NotImplementedException();
        }

        public ICodeFirst IsBackupData(bool isCreateTable = true)
        {
            throw new NotImplementedException();
        }

        public ICodeFirst IsBackupTable(bool isCreateTable = false)
        {
            throw new NotImplementedException();
        }

        public ICodeFirst IsDeleteNoExistColumn(bool isDeleteNoExistColumn = true)
        {
            throw new NotImplementedException();
        }

        public ICodeFirst Where(Func<string, bool> filterMethod)
        {
            throw new NotImplementedException();
        }
    }
}
