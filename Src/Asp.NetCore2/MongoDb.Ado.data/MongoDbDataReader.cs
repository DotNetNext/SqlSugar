﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;
using System.Collections;
using System.Linq;
using MongoDB.Bson.Serialization;

namespace MongoDb.Ado.data
{
    public class MongoDbBsonDocumentDataReader : DbDataReader
    {
        private readonly IEnumerator<BsonDocument> _enumerator;
        private BsonDocument _current;
        private IEnumerable<BsonDocument> _documents;
        private BsonDocument _firstObj;
        public MongoDbBsonDocumentDataReader(IEnumerable<BsonDocument> documents)
        {
            _enumerator = documents.GetEnumerator();
            _documents = documents;
            _firstObj=_documents.FirstOrDefault();
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

        public override int FieldCount => _documents?.FirstOrDefault()?.Count()??0;

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
        public override Type GetFieldType(int ordinal)
        {
            var firstObj = _firstObj;
            if(firstObj==null) return typeof(object);
            var obj = firstObj.GetValue(ordinal);
            if (obj is BsonObjectId) 
            {
                return typeof(string);
            }
            if (obj == null) return typeof(object);
            if (obj is BsonNull) return typeof(object);
            return BsonTypeMapper.MapToDotNetValue(obj).GetType();
        }
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
            if (ordinal < 0 || ordinal >= _current.ElementCount)
                throw new IndexOutOfRangeException($"Invalid ordinal: {ordinal}");

            var value = _current.GetElement(ordinal).Value;
            return value == null || value.IsBsonNull;
        }

        public override string GetName(int ordinal)
        {
            var firstObj=_firstObj;
            if (firstObj == null)
                return "";

            // 获取当前文档的字段元素列表（Elements）
            var elements = firstObj.Elements.ToList();

            // 确保 ordinal 是有效的索引
            if (ordinal < 0 || ordinal >= elements.Count)
                throw new IndexOutOfRangeException($"Invalid ordinal: {ordinal}");

            // 返回对应索引的字段名
            return elements[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            var firstObj = _firstObj;
            int i = 0;
            foreach (var elem in firstObj.Elements)
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
            var result= BsonTypeMapper.MapToDotNetValue(element.Value);
            return result;
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
