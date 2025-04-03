using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DynamicCoreSelectModel
    {
        public object Value { get; set; }

        public object ToList()
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToList", 0);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToList method with no parameters.");
            }

            return method.Invoke(Value, null);
        }

        public object ToPageList(int pageNumber, int pageSize)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageList", 2);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageList method with two parameters.");
            }

            return method.Invoke(Value, new object[] { pageNumber, pageSize });
        }

        public object ToPageList(int pageNumber, int pageSize, ref int totalNumber)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageList", 3);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageList method with three parameters.");
            }

            var parameters = new object[] { pageNumber, pageSize, totalNumber };
            var result = method.Invoke(Value, parameters);
            totalNumber = (int)parameters[2];
            return result;
        }
    }
}
