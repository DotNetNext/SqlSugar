using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public abstract partial class DbMaintenanceProvider : IDbMaintenance
    {
        #region Context
        public SqlSugarClient Context { get; set; }
        #endregion

        #region DML
        protected abstract string GetViewInfoListSql { get; }
        protected abstract string GetTableInfoListSql { get; }
        protected abstract string GetColumnInfosByTableNameSql { get; }
        #endregion

        #region DDL
        protected abstract string AddColumnToTableSql { get; }
        protected abstract string AlterColumnToTableSql { get; }
        protected abstract string BackupDataBaseSql { get; }
        protected abstract string CreateTableSql { get; }
        protected abstract string CreateTableColumn { get; }
        protected abstract string BackupTableSql { get; }
        protected abstract string TruncateTableSql { get; }
        protected abstract string DropTableSql { get; }
        protected abstract string DropColumnToTableSql { get; }
        #endregion

        #region Scattered
        protected abstract string CreateTableNull { get; }
        protected abstract string CreateTableNotNull { get; }
        protected abstract string CreateTablePirmaryKey { get; }
        protected abstract string CreateTableIdentity { get; }
        #endregion
    }
}
