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
        /// Some MYSQL databases do not support Nvarchar set true
        /// </summary>
        public bool MySqlDisableNvarchar { get; set; }

        /// <summary>
        ///Oracle nvarchar partial operation disabled
        /// </summary>
        public bool OracleDisableNvarchar { get; set; }
        public bool PgSqlIsAutoToLower = true;
        public int DefaultCacheDurationInSeconds { get; set; }
    }
}
