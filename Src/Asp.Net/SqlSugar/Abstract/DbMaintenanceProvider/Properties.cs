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
        public SqlSugarClient Context { get; set; }
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
        protected abstract string DropConstraintSql { get; }
        protected abstract string AddPrimaryKeySql { get; }
        protected abstract string RenameColumnSql { get; }
        protected abstract string AddColumnRemarkSql { get;  }
        protected abstract string DeleteColumnRemarkSql { get; }
        protected abstract string IsAnyColumnRemarkSql { get;  }
        protected abstract string AddTableRemarkSql { get;  }
        protected abstract string DeleteTableRemarkSql { get; }
        protected abstract string IsAnyTableRemarkSql { get;  }
        #endregion

        #region Check
        protected abstract string CheckSystemTablePermissionsSql { get; }
        #endregion

        #region Scattered
        protected abstract string CreateTableNull { get; }
        protected abstract string CreateTableNotNull { get; }
        protected abstract string CreateTablePirmaryKey { get; }
        protected abstract string CreateTableIdentity { get; }

        public virtual bool AddRemark(EntityInfo entity)
        {
            var db = this.Context;
            var columns = entity.Columns.Where(it => it.IsIgnore == false).ToList();

            foreach (var item in columns)
            {
                if (item.ColumnDescription != null)
                {
                    //column remak
                    if (db.DbMaintenance.IsAnyColumnRemark(item.DbColumnName, item.DbTableName))
                    {
                        db.DbMaintenance.DeleteColumnRemark(item.DbColumnName, item.DbTableName);
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);
                    }
                    else
                    {
                        db.DbMaintenance.AddColumnRemark(item.DbColumnName, item.DbTableName, item.ColumnDescription);
                    }
                }
            }

            //table remak
            if (entity.TableDescription != null)
            {
                if (db.DbMaintenance.IsAnyTableRemark(entity.DbTableName))
                {
                    db.DbMaintenance.DeleteTableRemark(entity.DbTableName);
                    db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
                }
                else
                {
                    db.DbMaintenance.AddTableRemark(entity.DbTableName, entity.TableDescription);
                }
            }
            return true;
        }
        #endregion
    }
}
