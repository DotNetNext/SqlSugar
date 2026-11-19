using System;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBDeleteBuilder : DeleteBuilder
    {
        public override string ToSqlString() => throw new NotSupportedException("Use IoTDB DELETE TIMESERIES or a tree-model time-range delete statement explicitly.");
    }
}
