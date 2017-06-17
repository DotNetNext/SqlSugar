using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public partial class CodeFirstProvider : ICodeFirst
    {
        #region Properties
        public virtual SqlSugarClient Context { get; set; }
        #endregion

        #region Fields
        private bool _isBackupData = true;
        private bool _isBackupTable = false;
        private bool _isDeleteNoExistColumn = true;
        #endregion

        #region Public methods

        public void InitTables(Type entityType)
        {
            var executeResult = Context.Ado.UseTran(() =>
            {
                Execute(entityType);
            });
            Check.Exception(!executeResult.IsSuccess, executeResult.Messaage);
        }

        public void InitTables(Type[] entityTypes)
        {
            if (entityTypes.IsValuable())
            {
                foreach (var item in entityTypes)
                {
                    InitTables(item);
                }
            }
        }

        public void InitTables(string entitiesNamespace)
        {
            var types = Assembly.Load(entitiesNamespace).GetTypes();
            InitTables(types);
        }

        public void InitTables(string[] entitiesNamespaces)
        {
            if (entitiesNamespaces.IsValuable())
            {
                foreach (var item in entitiesNamespaces)
                {
                    InitTables(item);
                }
            }
        }
        #endregion

        #region Core
        private void Execute(Type entityType)
        {
            var entityInfo = this.Context.EntityProvider.GetEntityInfo(entityType);
            var tableName = GetTableName(entityInfo);
            var isAny = this.Context.DbMaintenance.IsAnyTable(tableName);
            if (isAny)
                ExistLogic(entityInfo);
            else
                NoExistLogic(entityInfo);
        }

        private void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.IsValuable())
            {
                foreach (var item in entityInfo.Columns)
                {
                    DbColumnInfo dbColumnInfo = EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns);
        }

        private void ExistLogic(EntityInfo entityInfo)
        {
            if (entityInfo.Columns.IsValuable())
            {
                var tableName = GetTableName(entityInfo);
                var dbColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName);
                var errorColumns = dbColumns.Where(dbColumn => !entityInfo.Columns.Any(entityCoulmn => dbColumn.DbColumnName.Equals(entityCoulmn.DbColumnName))).ToList();
                var addColumns= entityInfo.Columns.Where(entityColumn => !dbColumns.Any(dbColumn => entityColumn.DbColumnName.Equals(dbColumn.DbColumnName))).ToList();

                foreach (var item in addColumns)
                {
                    this.Context.DbMaintenance.AddColumnToTable(tableName,EntityColumnToDbColumn(entityInfo,tableName,item));
                }
            }
         //   this.Context.DbMaintenance.CreateTable(tableName, columns);
        }

        public string GetCreateTableString(EntityInfo entityInfo)
        {
            StringBuilder result = new StringBuilder();
            var tableName = GetTableName(entityInfo);
            return result.ToString();
        }

        public string GetCreateColumnsString(EntityInfo entityInfo)
        {
            StringBuilder result = new StringBuilder();
            var tableName = GetTableName(entityInfo);

            return result.ToString();
        }

        private static string GetTableName(EntityInfo entityInfo)
        {
            return entityInfo.DbTableName == null ? entityInfo.EntityName : entityInfo.DbTableName;
        }
        private DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            return new DbColumnInfo()
            {
                Length = item.Length,
                DataType = this.Context.Ado.DbBind.GetDbTypeName(PubMethod.GetUnderType(item.PropertyInfo).Name),
                TableId = entityInfo.Columns.IndexOf(item),
                DbColumnName = item.DbColumnName.IsValuable() ? item.DbColumnName : item.PropertyName,
                IsPrimarykey = item.IsPrimarykey,
                IsIdentity = item.IsIdentity,
                TableName = tableName,
                IsNullable = item.IsNullable,
                DefaultValue = item.DefaultValue,
                ColumnDescription = item.ColumnDescription
            };
        }

        #endregion
    }
}
