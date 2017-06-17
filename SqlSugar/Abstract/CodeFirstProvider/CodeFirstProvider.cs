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
        private bool IsBackupTable { get; set; }
        private int MaxBackupDataRows { get; set; }
        #endregion

        #region Public methods
        public ICodeFirst BackupTable(int maxBackupDataRows = int.MaxValue)
        {
            this.IsBackupTable = true;
            this.MaxBackupDataRows = maxBackupDataRows;
            return this;
        }
        public void InitTables(Type entityType)
        {
            Check.Exception(this.Context.IsSystemTablesConfig, "Please set SqlSugarClent Parameter ConnectionConfig.InitKeyType=InitKeyType.Attribute ");
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
        public void InitTables(params string[] entitiesNamespaces)
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

        #region Core Logic
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
                var droupColumns = dbColumns.Where(dbColumn => !entityInfo.Columns.Any(entityCoulmn => dbColumn.DbColumnName.Equals(entityCoulmn.DbColumnName))).ToList();
                var addColumns = entityInfo.Columns.Where(entityColumn => !dbColumns.Any(dbColumn => entityColumn.DbColumnName.Equals(dbColumn.DbColumnName))).ToList();
                var alterColumns = dbColumns.Where(dbColumn => entityInfo.Columns
                                            .Any(entityCoulmn =>
                                                                  dbColumn.DbColumnName.Equals(entityCoulmn.DbColumnName)
                                                                  && ((entityCoulmn.Length != dbColumn.Length && PubMethod.GetUnderType(entityCoulmn.PropertyInfo).IsIn(PubConst.StringType)) || entityCoulmn.IsNullable != dbColumn.IsNullable)
                                                                  )).ToList();
                var isChange = false;
                foreach (var item in addColumns)
                {
                    this.Context.DbMaintenance.AddColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
                    isChange = true;
                }
                foreach (var item in droupColumns)
                {
                    this.Context.DbMaintenance.DropColumn(tableName, item.DbColumnName);
                    isChange = true;
                }
                foreach (var item in alterColumns)
                {
                    var newitem = this.Context.RewritableMethods.TranslateCopy(item);
                    newitem.IsPrimarykey = false;
                    newitem.IsIdentity = false;
                    if (newitem.Length > 0)
                        newitem.Length = newitem.Length / 2;
                    this.Context.DbMaintenance.UpdateColumn(tableName, newitem);
                    isChange = true;
                }
                if (isChange && IsBackupTable)
                {
                    this.Context.DbMaintenance.BackupTable(tableName, tableName + DateTime.Now.ToString("yyyyMMddHHmmss"),MaxBackupDataRows);
                }
            }
        }
        #endregion

        #region Helper methods
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
            var result= new DbColumnInfo()
            {
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
            return result;
        }
        #endregion
    }
}
