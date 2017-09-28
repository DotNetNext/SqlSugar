using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public interface ISerializeManager<T>
    {
        string SerializeObject(object value);
         T DeserializeObject(string value);
    }
}
