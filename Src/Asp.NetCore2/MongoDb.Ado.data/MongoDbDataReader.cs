using System;
using System.Collections.Generic;
using System.Data.Common;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;
using System.Collections;

namespace MongoDb.Ado.data
{
 
    public class MongoDbDataReader : DbDataReader
    {
        private readonly IAsyncCursor<BsonDocument> _cursor;
        private IEnumerator<BsonDocument> _enumerator;
        private BsonDocument _current;

        public MongoDbDataReader(IAsyncCursor<BsonDocument> cursor)
        {
            _cursor = cursor;
            _enumerator = cursor.ToEnumerable().GetEnumerator();
        }

        public override bool Read()
        {
            if (_enumerator.MoveNext())
            {
                _current = _enumerator.Current;
                return true;
            }
            return false;
        }

        public override int FieldCount => _current?.ElementCount ?? 0;

        public override object GetValue(int ordinal)
        {
            if (_current == null)
                throw new InvalidOperationException("No current document.");

            var element = GetElementByOrdinal(ordinal);
            return BsonTypeMapper.MapToDotNetValue(element.Value);
        }

        public override string GetName(int ordinal)
        {
            var element = GetElementByOrdinal(ordinal);
            return element.Name;
        }

        public override int GetOrdinal(string name)
        {
            int i = 0;
            foreach (var elem in _current.Elements)
            {
                if (elem.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return i;
                i++;
            }
            throw new IndexOutOfRangeException($"Field '{name}' not found.");
        }

        private BsonElement GetElementByOrdinal(int ordinal)
        {
            if (_current == null)
                throw new InvalidOperationException("No current document.");

            int i = 0;
            foreach (var elem in _current.Elements)
            {
                if (i == ordinal)
                    return elem;
                i++;
            }
            throw new IndexOutOfRangeException();
        }

        public override bool HasRows => true;
        public override bool IsClosed => false;
        public override int RecordsAffected => -1;
        public override bool NextResult() => false;

        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));

        // 下面这些可以根据需要进一步实现或抛异常
        public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);
        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override char GetChar(int ordinal) => (char)GetValue(ordinal);
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => throw new NotSupportedException();
        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;
        public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);
        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);
        public override double GetDouble(int ordinal) => (double)GetValue(ordinal);
        public override Type GetFieldType(int ordinal) => GetValue(ordinal)?.GetType() ?? typeof(object);
        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);
        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);
        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);
        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);
        public override long GetInt64(int ordinal) => (long)GetValue(ordinal);
        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int Depth => 0;
    }

}
