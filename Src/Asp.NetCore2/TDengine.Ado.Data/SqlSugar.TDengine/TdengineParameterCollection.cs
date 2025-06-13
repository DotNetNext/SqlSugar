using SqlSugar.TDengineAdo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SqlSugar.TDengineAdo
{
    public class TDengineParameterCollection : DbParameterCollection
    {
        private List<TDengineParameter> parameters = new List<TDengineParameter>();

        public override int Count => parameters.Count;

        public override object SyncRoot => ((ICollection)parameters).SyncRoot;

        public override int Add(object value)
        {
            parameters.Add((TDengineParameter)value);
            return parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
            {
                parameters.Add((TDengineParameter)value);
            }
        }

        public override void Clear()
        {
            parameters.Clear();
        }

        public override bool Contains(string value)
        {
            foreach (var param in parameters)
            {
                if (param.ParameterName.Equals(value, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public override bool Contains(object value)
        {
            return parameters.Contains((TDengineParameter)value);
        }

        public override void CopyTo(Array array, int index)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                array.SetValue(parameters[i], index + i);
            }
        }

        public override IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        public override int IndexOf(string parameterName)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].ParameterName.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }

        public override int IndexOf(object value)
        {
            return parameters.IndexOf((TDengineParameter)value);
        }

        public override void Insert(int index, object value)
        {
            parameters.Insert(index, (TDengineParameter)value);
        }

        public override void Remove(object value)
        {
            parameters.Remove((TDengineParameter)value);
        }

        public override void RemoveAt(int index)
        {
            parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index >= 0)
                parameters.RemoveAt(index);
        }

        protected override DbParameter GetParameter(int index)
        {
            return parameters[index];
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index >= 0)
                return parameters[index];
            return null;
        } 
        protected override void SetParameter(int index, DbParameter value)
        {
            if (index < 0 || index >= parameters.Count)
                throw new IndexOutOfRangeException("Index is out of range.");

            if (value == null)
                throw new ArgumentNullException(nameof(value), "Parameter cannot be null.");

            parameters[index] = (TDengineParameter)value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            int index = IndexOf(parameterName);
            if (index == -1)
                throw new IndexOutOfRangeException("Parameter not found.");

            if (value == null)
                throw new ArgumentNullException(nameof(value), "Parameter cannot be null.");

            parameters[index] = (TDengineParameter)value;
        }
    }

}
