using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public interface ITenant
    {
        void BeginAllTran();
        void CommitAllTran();
        void RollbackAllTran();
    }
}
