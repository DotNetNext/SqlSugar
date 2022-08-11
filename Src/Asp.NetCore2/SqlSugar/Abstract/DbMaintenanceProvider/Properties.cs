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
        private ISqlBuilder _SqlBuilder;
        public SqlSugarProvider Context { get; set; }
        public ISqlBuilder SqlBuilder
        {
            get
            {
                if (_SqlBuilder == null)
                {
                    _SqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
                    _SqlBuilder.Context = this.Context;
                }
                return _SqlBuilder;
            }
        }
        #endregion

        #region DML
        protected abstract string GetViewInfoListSql { get; }
        protected abstract string GetDataBaseSql { get; }
        protected abstract string GetTableInfoListSql { get; }
        protected abstract string GetColumnInfosByTableNameSql { get; }
        #endregion

        #region DDL
        protected abstract string CreateIndexSql { get;  }
        protected abstract string IsAnyIndexSql { get; }
        protected abstract string AddDefaultValueSql { get;  }
        protected abstract string CreateDataBaseSql { get; }
        protected abstract string AddColumnToTableSql { get; }
        protected abstract string AlterColumnToTableSql { get; }
        protected abstract string BackupDataBaseSql { get; }
        protected abstract string CreateTableSql { get; }
        protected abstract string CreateTableColumn { get; }
        protected abstract string BackupTableSql { get; }
        protected abstract string TruncateTableSql { get; }
        protected abstract string DropTableSql { get; }
        protected abstract string DropColumnToTableSql { get; }
        protected abstract string DropConstraintSql { get; }
        protected abstract string AddPrimaryKeySql { get; }
        protected abstract string RenameColumnSql { get; }
        protected abstract string AddColumnRemarkSql { get;  }
        protected abstract string DeleteColumnRemarkSql { get; }
        protected abstract string IsAnyColumnRemarkSql { get;  }
        protected abstract string AddTableRemarkSql { get;  }
        protected abstract string DeleteTableRemarkSql { get; }
        protected abstract string IsAnyTableRemarkSql { get;  }
        protected abstract string RenameTableSql { get; }
        #endregion

        #region Check
        protected abstract string CheckSystemTablePermissionsSql { get; }
        #endregion

        #region Scattered
        protected abstract string CreateTableNull { get; }
        protected abstract string CreateTableNotNull { get; }
        protected abstract string CreateTablePirmaryKey { get; }
        protected abstract string CreateTableIdentity { get; }
        #endregion
    }
}
