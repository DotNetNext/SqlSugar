using System;
using System.Collections.Generic;
using System.Data.Common;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;
using System.Collections;
using System.Linq;

namespace MongoDb.Ado.data
{
    public class MongoDbBsonDocumentDataReader : DbDataReader
    {
        private readonly IEnumerator<BsonDocument> _enumerator;
        private BsonDocument _current;
        private List<string> _fieldNames;
        private List<Type> _fieldTypes;
        public MongoDbBsonDocumentDataReader(IEnumerable<BsonDocument> documents)
        {
            var docList = documents.ToList();
            _enumerator = docList.GetEnumerator();
            if (docList.Any()==true)
            {
                _fieldNames = docList.Take(1).SelectMany(d => d.Names).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                _fieldTypes = new List<Type>(); 
                foreach (var fieldName in _fieldNames)
                {
                    Type fieldType = typeof(object); // 默认类型

                    foreach (var doc in docList)
                    {
                        if (doc.TryGetValue(fieldName, out var value) && value != BsonNull.Value)
                        {
                            fieldType = BsonTypeMapper.MapToDotNetValue(value)?.GetType() ?? typeof(object);
                        }
                        break;
                    }
                    _fieldTypes.Add(fieldType);
                }
            }
            else
            {
                _fieldNames = new List<string>();
                _fieldTypes = new List<Type>();
            } 
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

        public override int FieldCount => _fieldNames.Count;

        public override int Depth => throw new NotImplementedException();

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
        public override Type GetFieldType(int ordinal) => _fieldTypes[ordinal] ?? typeof(object);
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

        public override string GetName(int ordinal)
        {
            if (ordinal < 0 || ordinal >= _fieldNames.Count)
                throw new IndexOutOfRangeException($"Invalid ordinal: {ordinal}");
            return _fieldNames[ordinal];
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

        public override object GetValue(int ordinal)
        {
            if (_current == null)
                throw new InvalidOperationException("No current document.");

            var element = GetElementByOrdinal(ordinal);
            return BsonTypeMapper.MapToDotNetValue(element.Value);
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
    }

}
