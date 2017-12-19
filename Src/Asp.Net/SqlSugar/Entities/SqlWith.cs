using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqlWith
    {
        public const string NoLock = "WITH(NOLOCK) ";
        public const string HoldLock = "WITH(HOLDLOCK)";
        public const string PagLock = "WITH(PAGLOCK)";
        public const string ReadCommitted = "WITH(READCOMMITTED)";
        public const string TabLockX = "WITH(TABLOCKX)";
        public const string UpdLock = "WITH(UPDLOCK)";
        public const string RowLock = "WITH(ROWLOCK)";
        public const string Null = "Non";
    }
}
