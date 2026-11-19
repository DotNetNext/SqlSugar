using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBUpdateBuilder : UpdateBuilder
    {
        public override string ToSqlString() => throw new NotSupportedException("IoTDB tree model does not support SQL UPDATE; write a new point with Insertable instead.");
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList) => throw new NotSupportedException("IoTDB tree model does not support SQL UPDATE.");
    }
}
