using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class DefaultServices
    {
        public static ICacheService ReflectionInoCache { get; set; }
        public static ICacheService DataInoCache
        {
            get; set;
        public static ISerializeService Serialize { get; set; }
    }
}
