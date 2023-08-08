using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.TDengine
{
    public class TDengineUpdateBuilder : UpdateBuilder
    { 
        public override string ToSqlString()
        {
            throw new NotSupportedException("TDengine库不支持更新操作");
        }
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            throw new NotSupportedException("TDengine库不支持更新操作");
        }

        
    }
}
