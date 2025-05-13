using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MongoDb.Ado.data
{ 
  // 将类的访问修饰符从 private 更改为 internal，以解决 CS1527 错误  
    internal class EmptyDbParameterCollection : DbParameterCollection
    {
        public override int Add(object value) => 0;
        public override void AddRange(Array values) { }
        public override void Clear() { }
        public override bool Contains(string value) => false;
        public override bool Contains(object value) => false;
        public override void CopyTo(Array array, int index) { }
        public override int Count => 0;
        public override System.Collections.IEnumerator GetEnumerator() => Array.Empty<object>().GetEnumerator();
        public override int IndexOf(string parameterName) => -1;
        public override int IndexOf(object value) => -1;
        public override void Insert(int index, object value) { }
        public override bool IsFixedSize => true;
        public override bool IsReadOnly => true;
        public override bool IsSynchronized => false;
        public override void Remove(object value) { }
        public override void RemoveAt(string parameterName) { }
        public override void RemoveAt(int index) { }

        protected override DbParameter GetParameter(int index)
        {
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return null;
        }

        protected override void SetParameter(int index, DbParameter value)
        {
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
        }

        public override object SyncRoot => new object();
    }
}
