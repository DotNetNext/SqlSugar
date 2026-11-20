using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar.SonnetDB;

/// <summary>在原生读取器成功关闭后，向本次调用冻结的 SugarParameter 写回输出。</summary>
internal sealed class SonnetDBOutputDataReader(
    DbDataReader inner, (SugarParameter Target, IDataParameter Source)[] outputs) : DbDataReader
{
    private bool failed;
    private int closed;
    public override int Depth => inner.Depth;
    public override int FieldCount => inner.FieldCount;
    public override bool HasRows => inner.HasRows;
    public override bool IsClosed => inner.IsClosed;
    public override int RecordsAffected => inner.RecordsAffected;
    public override object this[int ordinal] => inner[ordinal];
    public override object this[string name] => inner[name];
    public override bool GetBoolean(int ordinal) => inner.GetBoolean(ordinal);
    public override byte GetByte(int ordinal) => inner.GetByte(ordinal);
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => inner.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    public override char GetChar(int ordinal) => inner.GetChar(ordinal);
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => inner.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    public override string GetDataTypeName(int ordinal) => inner.GetDataTypeName(ordinal);
    public override DateTime GetDateTime(int ordinal) => inner.GetDateTime(ordinal);
    public override decimal GetDecimal(int ordinal) => inner.GetDecimal(ordinal);
    public override double GetDouble(int ordinal) => inner.GetDouble(ordinal);
    public override IEnumerator GetEnumerator() => new DbEnumerator(this, false);
    public override Type GetFieldType(int ordinal) => inner.GetFieldType(ordinal);
    public override float GetFloat(int ordinal) => inner.GetFloat(ordinal);
    public override Guid GetGuid(int ordinal) => inner.GetGuid(ordinal);
    public override short GetInt16(int ordinal) => inner.GetInt16(ordinal);
    public override int GetInt32(int ordinal) => inner.GetInt32(ordinal);
    public override long GetInt64(int ordinal) => inner.GetInt64(ordinal);
    public override string GetName(int ordinal) => inner.GetName(ordinal);
    public override int GetOrdinal(string name) => inner.GetOrdinal(name);
    public override DataTable? GetSchemaTable() => inner.GetSchemaTable();
    public override string GetString(int ordinal) => inner.GetString(ordinal);
    public override object GetValue(int ordinal) => inner.GetValue(ordinal);
    public override int GetValues(object[] values) => inner.GetValues(values);
    public override bool IsDBNull(int ordinal) => inner.IsDBNull(ordinal);
    public override bool Read()
    {
        try { return inner.Read(); }
        catch { failed = true; throw; }
    }
    public override bool NextResult()
    {
        try { return inner.NextResult(); }
        catch { failed = true; throw; }
    }
    public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        try { return await inner.ReadAsync(cancellationToken).ConfigureAwait(false); }
        catch { failed = true; throw; }
    }
    public override async Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
        try { return await inner.NextResultAsync(cancellationToken).ConfigureAwait(false); }
        catch { failed = true; throw; }
    }
    public override void Close()
    {
        if (Interlocked.Exchange(ref closed, 1) != 0) return;
        try { inner.Close(); Publish(); }
        catch { failed = true; throw; }
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) Close();
        base.Dispose(disposing);
    }
    public override async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref closed, 1) != 0) return;
        try { await inner.DisposeAsync().ConfigureAwait(false); Publish(); }
        catch { failed = true; throw; }
        GC.SuppressFinalize(this);
    }
    private void Publish()
    {
        if (failed) return;
        object[] values = new object[outputs.Length];
        for (int index = 0; index < outputs.Length; index++) values[index] = outputs[index].Source.Value ?? DBNull.Value;
        for (int index = 0; index < outputs.Length; index++) outputs[index].Target.Value = values[index];
    }
}
