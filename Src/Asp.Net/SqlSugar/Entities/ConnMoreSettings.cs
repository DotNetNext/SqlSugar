using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ConnMoreSettings
    {
        public bool IsAutoRemoveDataCache { get; set; }
        public bool IsWithNoLockQuery { get; set; }
        /// <summary>
        /// Some MYSQL databases do not support NVarchar set true
        /// </summary>
        public bool MySqlDisableNarvchar { get; set; }
        public int DefaultCacheDurationInSeconds { get; set; }
    }
}
