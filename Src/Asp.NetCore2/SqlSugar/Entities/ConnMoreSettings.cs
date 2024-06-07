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
        public bool DisableWithNoLockWithTran { get; set; }
        public bool IsWithNoLockSubquery { get; set; }

        public bool DisableNvarchar { get; set; }
        public bool DisableMillisecond { get; set; }
        public bool PgSqlIsAutoToLower { get; set; } = true;
        public bool PgSqlIsAutoToLowerCodeFirst { get; set; } = true;
        public bool EnableILike { get; set; }
        public bool IsAutoToUpper { get; set; } = true;
        public int DefaultCacheDurationInSeconds { get; set; }
        public bool? TableEnumIsString { get; set; }
        public DateTime? DbMinDate { get; set; } = DateTime.MinValue.Date.AddYears(1900 - 1);
        public bool IsNoReadXmlDescription { get;  set; }
        public bool SqlServerCodeFirstNvarchar { get;  set; }
        public bool OracleCodeFirstNvarchar2 { get; set; }
        public bool SqliteCodeFirstEnableDefaultValue { get; set; }
        public bool SqliteCodeFirstEnableDescription { get; set; }
        public bool IsAutoUpdateQueryFilter { get; set; }
        public bool IsAutoDeleteQueryFilter { get; set; }
        public bool EnableModelFuncMappingColumn { get; set; }
        public bool EnableOracleIdentity { get; set; }
        public bool EnableCodeFirstUpdatePrecision { get;  set; }
        public bool SqliteCodeFirstEnableDropColumn { get; set; }
        public bool IsCorrectErrorSqlParameterName { get; set; }
        public int MaxParameterNameLength { get; set; }
        public bool DisableQueryWhereColumnRemoveTrim { get; set; }
        public DbType? DatabaseModel { get;set; }
    }
}
