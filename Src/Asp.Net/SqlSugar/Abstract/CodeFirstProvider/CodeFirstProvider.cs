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
        public virtual SqlSugarProvider Context { get; set; }
        protected bool IsBackupTable { get; set; }
        protected int MaxBackupDataRows { get; set; }
        protected virtual int DefultLength { get; set; }
        public CodeFirstProvider()
        {
            if (DefultLength == 0)
            {
                DefultLength = 255;
            }
        }
        #endregion

        #region Public methods
        public virtual ICodeFirst BackupTable(int maxBackupDataRows = int.MaxValue)
        {
            this.IsBackupTable = true;
            this.MaxBackupDataRows = maxBackupDataRows;
            return this;
        }

        public virtual ICodeFirst SetStringDefaultLength(int length)
        {
            DefultLength = length;
            return this;
        }

        public virtual void InitTables(Type entityType)
        {

            this.Context.Utilities.RemoveCacheAll();
            this.Context.InitMappingInfo(entityType);
            if (!this.Context.DbMaintenance.IsAnySystemTablePermissions())
            {
                Check.Exception(true, "Dbfirst and  Codefirst requires system table permissions");
            }
            Check.Exception(this.Context.IsSystemTablesConfig, "Please set SqlSugarClent Parameter ConnectionConfig.InitKeyType=InitKeyType.Attribute ");
            var executeResult = Context.Ado.UseTran(() =>
            {
                Execute(entityType);
            });
            Check.Exception(!executeResult.IsSuccess, executeResult.ErrorMessage);
        }
        public void InitTables<T>()
        {
            InitTables(typeof(T));
        }
        public void InitTables<T, T2>()
        {
            InitTables(typeof(T), typeof(T2));
        }
        public void InitTables<T, T2, T3>()
        {
            InitTables(typeof(T), typeof(T2), typeof(T3));
        }
        public void InitTables<T, T2, T3, T4>()
        {
            InitTables(typeof(T), typeof(T2), typeof(T3), typeof(T4));
        }
        public virtual void InitTables(params Type[] entityTypes)
        {
            if (entityTypes.HasValue())
            {
                foreach (var item in entityTypes)
                {
                    try
                    {
                        InitTables(item);
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(item.Name +" 创建失败,请认真检查 1、属性需要get set 2、特殊类型需要加Ignore 具体错误内容： "+ex.Message);
                    }
                }
            }
        }
        public virtual void InitTables(string entitiesNamespace)
        {
            var types = Assembly.Load(entitiesNamespace).GetTypes();
            InitTables(types);
        }
        public virtual void InitTables(params string[] entitiesNamespaces)
        {
            if (entitiesNamespaces.HasValue())
            {
                foreach (var item in entitiesNamespaces)
                {
                    InitTables(item);
                }
            }
        }
        #endregion

        #region Core Logic
        protected virtual void Execute(Type entityType)
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(entityType);
            if (this.DefultLength > 0)
            {
                foreach (var item in entityInfo.Columns)
                {
                    if (item.PropertyInfo.PropertyType == UtilConstants.StringType && item.DataType.IsNullOrEmpty() && item.Length == 0)
                    {
                        item.Length = DefultLength;
                    }
                }
            }
            var tableName = GetTableName(entityInfo);
            this.Context.MappingTables.Add(entityInfo.EntityName,tableName);
            entityInfo.DbTableName = tableName;
            entityInfo.Columns.ForEach(it => { it.DbTableName = tableName; });
            var isAny = this.Context.DbMaintenance.IsAnyTable(tableName);
            if (isAny&&entityInfo.IsDisabledUpdateAll)
            {
                return;
            }
            if (isAny)
                ExistLogic(entityInfo);
            else
                NoExistLogic(entityInfo);

            this.Context.DbMaintenance.AddRemark(entityInfo);
            this.Context.DbMaintenance.AddIndex(entityInfo);
            this.Context.DbMaintenance.AddDefaultValue(entityInfo);
        }
        public virtual void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.HasValue())
            {
                foreach (var item in entityInfo.Columns.OrderBy(it => it.IsPrimarykey ? 0 : 1).Where(it => it.IsIgnore == false))
                {
                    DbColumnInfo dbColumnInfo = EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns, true);
        }
        public virtual void ExistLogic(EntityInfo entityInfo)
        {
            if (entityInfo.Columns.HasValue()&&entityInfo.IsDisabledUpdateAll==false)
            {
                //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Multiple primary keys do not support modifications");

                var tableName = GetTableName(entityInfo);
                var dbColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName);
                ConvertColumns(dbColumns);
                var entityColumns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
                var dropColumns = dbColumns
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.DbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .ToList();
                var addColumns = entityColumns
                                          .Where(ec => ec.OldDbColumnName.IsNullOrEmpty() || !dbColumns.Any(dc => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(ec => !dbColumns.Any(dc => ec.DbColumnName.Equals(dc.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
                var alterColumns = entityColumns
                                           .Where(ec => !dbColumns.Any(dc => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                           .Where(ec =>
                                                          dbColumns.Any(dc => dc.DbColumnName.Equals(ec.DbColumnName)
                                                               && ((ec.Length != dc.Length && !UtilMethods.GetUnderType(ec.PropertyInfo).IsEnum() && UtilMethods.GetUnderType(ec.PropertyInfo).IsIn(UtilConstants.StringType)) ||
                                                                    ec.IsNullable != dc.IsNullable ||
                                                                    IsSamgeType(ec, dc)))).ToList();
                var renameColumns = entityColumns
                    .Where(it => !string.IsNullOrEmpty(it.OldDbColumnName))
                    .Where(entityColumn => dbColumns.Any(dbColumn => entityColumn.OldDbColumnName.Equals(dbColumn.DbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();


                var isMultiplePrimaryKey = dbColumns.Where(it => it.IsPrimarykey).Count() > 1 || entityColumns.Where(it => it.IsPrimarykey).Count() > 1;


                var isChange = false;
                foreach (var item in addColumns)
                {
                    this.Context.DbMaintenance.AddColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
                    isChange = true;
                }
                if (entityInfo.IsDisabledDelete==false)
                {
                    foreach (var item in dropColumns)
                    {
                        this.Context.DbMaintenance.DropColumn(tableName, item.DbColumnName);
                        isChange = true;
                    }
                }
                foreach (var item in alterColumns)
                {
                    this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
                    isChange = true;
                }
                foreach (var item in renameColumns)
                {
                    this.Context.DbMaintenance.RenameColumn(tableName, item.OldDbColumnName, item.DbColumnName);
                    isChange = true;
                }

                foreach (var item in entityColumns)
                {
                    var dbColumn = dbColumns.FirstOrDefault(dc => dc.DbColumnName.Equals(item.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                    if (dbColumn == null) continue;
                    bool pkDiff, idEntityDiff;
                    KeyAction(item, dbColumn, out pkDiff, out idEntityDiff);
                    if (dbColumn != null && pkDiff && !idEntityDiff && isMultiplePrimaryKey == false)
                    {
                        var isAdd = item.IsPrimarykey;
                        if (isAdd)
                        {
                            this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
                        }
                        else
                        {
                            this.Context.DbMaintenance.DropConstraint(tableName, string.Format("PK_{0}_{1}", tableName, item.DbColumnName));
                        }
                    }
                    else if ((pkDiff || idEntityDiff) && isMultiplePrimaryKey == false)
                    {
                        ChangeKey(entityInfo, tableName, item);
                    }
                }
                if (isMultiplePrimaryKey)
                {
                    var oldPkNames = dbColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                    var newPkNames = entityColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                    if (!Enumerable.SequenceEqual(oldPkNames, newPkNames))
                    {
                        Check.Exception(true, ErrorMessage.GetThrowMessage("Modification of multiple primary key tables is not supported. Delete tables while creating", "不支持修改多主键表，请删除表在创建"));
                    }

                }
                if (isChange && IsBackupTable)
                {
                    this.Context.DbMaintenance.BackupTable(tableName, tableName + DateTime.Now.ToString("yyyyMMddHHmmss"), MaxBackupDataRows);
                }
            }
        }

        protected virtual void KeyAction(EntityColumnInfo item, DbColumnInfo dbColumn, out bool pkDiff, out bool idEntityDiff)
        {
            pkDiff = item.IsPrimarykey != dbColumn.IsPrimarykey;
            idEntityDiff = item.IsIdentity != dbColumn.IsIdentity;
        }

        protected virtual void ChangeKey(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            string constraintName = string.Format("PK_{0}_{1}", tableName, item.DbColumnName);
            if (this.Context.DbMaintenance.IsAnyConstraint(constraintName))
                this.Context.DbMaintenance.DropConstraint(tableName, constraintName);
            this.Context.DbMaintenance.DropColumn(tableName, item.DbColumnName);
            this.Context.DbMaintenance.AddColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            if (item.IsPrimarykey)
                this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }

        protected virtual void ConvertColumns(List<DbColumnInfo> dbColumns)
        {

        }
        #endregion

        #region Helper methods
        public virtual string GetCreateTableString(EntityInfo entityInfo)
        {
            StringBuilder result = new StringBuilder();
            var tableName = GetTableName(entityInfo);
            return result.ToString();
        }
        public virtual string GetCreateColumnsString(EntityInfo entityInfo)
        {
            StringBuilder result = new StringBuilder();
            var tableName = GetTableName(entityInfo);
            return result.ToString();
        }
        protected virtual string GetTableName(EntityInfo entityInfo)
        {
            return this.Context.EntityMaintenance.GetTableName(entityInfo.EntityName);
        }
        protected virtual DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var propertyType = UtilMethods.GetUnderType(item.PropertyInfo);
            var result = new DbColumnInfo()
            {
                TableId = entityInfo.Columns.IndexOf(item),
                DbColumnName = item.DbColumnName.HasValue() ? item.DbColumnName : item.PropertyName,
                IsPrimarykey = item.IsPrimarykey,
                IsIdentity = item.IsIdentity,
                TableName = tableName,
                IsNullable = item.IsNullable,
                DefaultValue = item.DefaultValue,
                ColumnDescription = item.ColumnDescription,
                Length = item.Length,
                DecimalDigits = item.DecimalDigits
            };
            GetDbType(item, propertyType, result);
            return result;
        }

        protected virtual void GetDbType(EntityColumnInfo item, Type propertyType, DbColumnInfo result)
        {
            if (!string.IsNullOrEmpty(item.DataType))
            {
                result.DataType = item.DataType;
            }
            else if (propertyType.IsEnum())
            {
                result.DataType = this.Context.Ado.DbBind.GetDbTypeName(item.Length > 9 ? UtilConstants.LongType.Name : UtilConstants.IntType.Name);
            }
            else
            {
                var name = GetType(propertyType.Name);
                result.DataType = this.Context.Ado.DbBind.GetDbTypeName(name);
            }
        }

        protected virtual bool IsSamgeType(EntityColumnInfo ec, DbColumnInfo dc)
        {
            if (!string.IsNullOrEmpty(ec.DataType))
            {
                return ec.DataType != dc.DataType;
            }
            var propertyType = UtilMethods.GetUnderType(ec.PropertyInfo);
            var properyTypeName = string.Empty;
            if (propertyType.IsEnum())
            {
                properyTypeName = this.Context.Ado.DbBind.GetDbTypeName(ec.Length > 9 ? UtilConstants.LongType.Name : UtilConstants.IntType.Name);
            }
            else
            {
                var name = GetType(propertyType.Name);
                properyTypeName = this.Context.Ado.DbBind.GetDbTypeName(name);
            }
            var dataType = dc.DataType;
            if (properyTypeName == "boolean" && dataType == "bool")
            {
                return false;
            }
            return properyTypeName != dataType;
        }
        private static string GetType(string name)
        {
            if (name.IsContainsIn("UInt32", "UInt16", "UInt64"))
            {
                name = name.TrimStart('U');
            }
            if (name == "char")
            {
                name = "string";
            }
            return name;
        }

        #endregion
    }
}
