using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DefaultServices
    {
        public static ICacheService ReflectionInoCache = new ReflectionInoCache();
        public static ISerializeService Serialize = new SerializeService();
    }
}
