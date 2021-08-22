using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class CacheKey
    {
        public string AppendKey { get; set; }
        public string Database { get; set; }
        public List<string> Tables { get; set; }
        public List<string> IdentificationList { get; set; }
        public new string ToString()
        {
            var result= "SqlSugarDataCache" + UtilConstants.Dot + string.Join(UtilConstants.Dot, this.Tables) +UtilConstants.Dot+ string.Join(UtilConstants.Dot, this.IdentificationList.Where(it=>it.HasValue()));
            result = result + AppendKey;
            return result;
        }
    }
}
