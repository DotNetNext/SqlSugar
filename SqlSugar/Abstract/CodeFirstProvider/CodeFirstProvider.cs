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

        public ICodeFirst IsBackupData(bool isBackupData = true)
        {
            _isBackupData = isBackupData;
            return this;
        }

        public ICodeFirst IsBackupTable(bool isBackupTable = false)
        {
            _isBackupTable = isBackupTable;
            return this;
        }

        public ICodeFirst IsDeleteNoExistColumn(bool isDeleteNoExistColumn = true)
        {
            _isDeleteNoExistColumn = isDeleteNoExistColumn;
            return this;
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
                    DbColumnInfo dbColumnInfo = new DbColumnInfo()
                    {
                        Length = item.Length,
                        DataType = this.Context.Ado.DbBind.GetDbTypeName(item.PropertyInfo.PropertyType.Name),
                        TableId = entityInfo.Columns.IndexOf(item),
                        DbColumnName = item.DbColumnName.IsValuable() ? item.DbColumnName : item.PropertyName,
                        IsPrimarykey = item.IsPrimarykey,
                        IsIdentity = item.IsIdentity,
                        TableName = tableName,
                        IsNullable = PubMethod.IsNullable(item.PropertyInfo),
                        DefaultValue = item.DefaultValue,
                        ColumnDescription = item.ColumnDescription
                    };
                }
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns);
        }

        private void ExistLogic(EntityInfo entityInfo)
        {
            throw new NotImplementedException();
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
        #endregion
    }
}
