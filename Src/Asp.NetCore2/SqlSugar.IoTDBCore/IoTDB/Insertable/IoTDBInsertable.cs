using System;
using System.Collections.Generic;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBInsertable<T> : InsertableProvider<T> where T : class, new()
    {
        public override int ExecuteCommand()
        {
            var command = ToSql();
            var result = this.Context.Ado.ExecuteCommand(command.Key, command.Value?.ToArray());
            After(command.Key, null);
            return result < 0 ? this.InsertObjs.Length : result;
        }

        public override System.Collections.Generic.KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InsertBuilder.IsReturnIdentity = false;
            return base.ToSql();
        }

        public override int ExecuteReturnIdentity() => throw new NotSupportedException("IoTDB tree model has no auto-increment identity columns.");
        public override long ExecuteReturnBigIdentity() => throw new NotSupportedException("IoTDB tree model has no auto-increment identity columns.");
    }
}
