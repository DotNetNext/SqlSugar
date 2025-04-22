using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar.DuckDB
{
    public class DuckDBCodeFirst : CodeFirstProvider
    {
        protected override void ExistLogicEnd(List<EntityColumnInfo> dbColumns)
        {
            foreach (EntityColumnInfo column in dbColumns) 
            {
                if (column.DefaultValue != null) 
                {
                    this.Context.DbMaintenance.AddDefaultValue(column.DbTableName,column.DbColumnName,column.DefaultValue.ToSqlValue());
                }
            }
        }
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.HasValue())
            {
                foreach (var item in entityInfo.Columns.Where(it=>it.IsIgnore==false))
                {
                    DbColumnInfo dbColumnInfo = this.EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
                if (entityInfo.IsCreateTableFiledSort)
                {
                    columns = columns.OrderBy(c => c.CreateTableFieldSort).ToList();
                }
            }
            columns = columns.OrderBy(it => it.IsPrimarykey ? 0 : 1).ToList();
            this.Context.DbMaintenance.CreateTable(tableName, columns,true);
        }
        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
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
                CreateTableFieldSort = item.CreateTableFieldSort
            };
            if (propertyType == UtilConstants.DecType) 
            {
                result.Scale = item.DecimalDigits;
                result.DecimalDigits = item.DecimalDigits;
            }
            GetDbType(item, propertyType, result);
            if (result.DataType.Equals("varchar", StringComparison.CurrentCultureIgnoreCase) && result.Length == 0)
            {
                result.Length = 1;
            }
            return result;
        }

        protected override void ConvertColumns(List<DbColumnInfo> dbColumns)
        {
            foreach (var item in dbColumns)
            {
                if (item.DataType == "DateTime")
                {
                    item.Length = 0;
                }
            }
        }
        public override void ExistLogic(EntityInfo entityInfo)
        {
            if (entityInfo.Columns.HasValue() && entityInfo.IsDisabledUpdateAll == false)
            {
                //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Multiple primary keys do not support modifications");

                var tableName = GetTableName(entityInfo);
                var dbColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName, false);
                ConvertColumns(dbColumns);
                var entityColumns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
                var dropColumns = dbColumns
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.DbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .ToList();
                var addColumns = entityColumns
                                          .Where(ec => ec.OldDbColumnName.IsNullOrEmpty() || !dbColumns.Any(dc => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(ec => !dbColumns.Any(dc => ec.DbColumnName.Equals(dc.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
                var alterColumns =new List<EntityColumnInfo>();

                alterColumns.RemoveAll(entityColumnInfo =>
                {
                    var bigStringArray = StaticConfig.CodeFirst_BigString.Replace("varcharmax", "nvarchar(max)").Split(',');
                    var dbColumnInfo = dbColumns.FirstOrDefault(dc => dc.DbColumnName.EqualCase(entityColumnInfo.DbColumnName));
                    var isMaxString = (dbColumnInfo?.Length == -1 && dbColumnInfo?.DataType?.EqualCase("nvarchar") == true);
                    var isRemove =
                           dbColumnInfo != null
                           && bigStringArray.Contains(entityColumnInfo.DataType)
                           && isMaxString;
                    return isRemove;
                });
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
                if (entityInfo.IsDisabledDelete == false)
                {
                    foreach (var item in dropColumns)
                    {
                        this.Context.DbMaintenance.DropColumn(tableName, item.DbColumnName);
                        isChange = true;
                    }
                }
                foreach (var item in alterColumns)
                {

                    if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    {
                        var entityColumnItem = entityColumns.FirstOrDefault(y => y.DbColumnName == item.DbColumnName);
                        if (entityColumnItem != null && !string.IsNullOrEmpty(entityColumnItem.DataType))
                        {
                            continue;
                        }
                    }

                    this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
                    isChange = true;
                }
                foreach (var item in renameColumns)
                {
                    this.Context.DbMaintenance.RenameColumn(tableName, item.OldDbColumnName, item.DbColumnName);
                    isChange = true;
                }
                var isAddPrimaryKey = false;
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
                            isAddPrimaryKey = true;
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
                if (isAddPrimaryKey == false && entityColumns.Count(it => it.IsPrimarykey) == 1 && dbColumns.Count(it => it.IsPrimarykey) == 0)
                {
                    var addPk = entityColumns.First(it => it.IsPrimarykey);
                    this.Context.DbMaintenance.AddPrimaryKey(tableName, addPk.DbColumnName);
                }
                if (isMultiplePrimaryKey)
                {
                    var oldPkNames = dbColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                    var newPkNames = entityColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                    if (oldPkNames.Count == 0 && newPkNames.Count > 1)
                    {
                        try
                        {
                            this.Context.DbMaintenance.AddPrimaryKeys(tableName, newPkNames.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Check.Exception(true, ErrorMessage.GetThrowMessage("The current database does not support changing multiple primary keys. " + ex.Message, "当前数据库不支持修改多主键," + ex.Message));
                            throw ex;
                        }
                    }
                    else if (!Enumerable.SequenceEqual(oldPkNames, newPkNames))
                    {
                        Check.Exception(true, ErrorMessage.GetThrowMessage("Modification of multiple primary key tables is not supported. Delete tables while creating", "不支持修改多主键表，请删除表在创建"));
                    }

                }
                if (isChange && IsBackupTable)
                {
                    this.Context.DbMaintenance.BackupTable(tableName, tableName + DateTime.Now.ToString("yyyyMMddHHmmss"), MaxBackupDataRows);
                }
                ExistLogicEnd(entityColumns);
            }
        }
        private bool IsNoSamePrecision(EntityColumnInfo ec, DbColumnInfo dc)
        {
            if (this.Context.CurrentConnectionConfig.MoreSettings?.EnableCodeFirstUpdatePrecision == true)
            {
                return ec.DecimalDigits != dc.DecimalDigits && ec.UnderType.IsIn(UtilConstants.DobType, UtilConstants.DecType);
            }
            return false;
        }
        protected override void ChangeKey(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            if (!item.IsPrimarykey)
                this.Context.DbMaintenance.DropConstraint(tableName,null);
            if (item.IsPrimarykey)
                this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }

    }
}
