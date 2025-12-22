using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar.TDengine
{
    public class TDengineCodeFirst : CodeFirstProvider
    {
        protected override void Execute(Type entityType, EntityInfo entityInfo)
        {
            var attr =GetCommonSTableAttribute( entityInfo.Type.GetCustomAttribute<STableAttribute>());
            if (attr?.STableName!=null&&attr?.Tag1!=null) 
            {
                entityInfo.DbTableName = attr.STableName;
                Context.MappingTables.Add(entityInfo.EntityName, entityInfo.DbTableName);
            }
            //var entityInfo = this.Context.EntityMaintenance.GetEntityInfoNoCache(entityType);
            if (entityInfo.Discrimator.HasValue())
            {
                Check.ExceptionEasy(!Regex.IsMatch(entityInfo.Discrimator, @"^(?:\w+:\w+)(?:,\w+:\w+)*$"), "The format should be type:cat for this type, and if there are multiple, it can be FieldName:cat,FieldName2:dog ", "格式错误应该是type:cat这种格式，如果是多个可以FieldName:cat,FieldName2:dog，不要有空格");
                var array = entityInfo.Discrimator.Split(',');
                foreach (var disItem in array)
                {
                    var name = disItem.Split(':').First();
                    var value = disItem.Split(':').Last();
                    entityInfo.Columns.Add(new EntityColumnInfo() { PropertyInfo = typeof(DiscriminatorObject).GetProperty(nameof(DiscriminatorObject.FieldName)), IsOnlyIgnoreUpdate = true, DbColumnName = name, UnderType = typeof(string), PropertyName = name, Length = 50 });
                }
            }
            if (this.MappingTables.ContainsKey(entityType))
            {
                entityInfo.DbTableName = this.MappingTables[entityType];
                this.Context.MappingTables.Add(entityInfo.EntityName, entityInfo.DbTableName);
            }
            if (this.DefultLength > 0)
            {
                foreach (var item in entityInfo.Columns)
                {
                    if (item.PropertyInfo.PropertyType == UtilConstants.StringType && item.DataType.IsNullOrEmpty() && item.Length == 0)
                    {
                        item.Length = DefultLength;
                    }
                    if (item.DataType != null && item.DataType.Contains(",") && !Regex.IsMatch(item.DataType, @"\d\,\d"))
                    {
                        var types = item.DataType.Split(',').Select(it => it.ToLower()).ToList();
                        var mapingTypes = this.Context.Ado.DbBind.MappingTypes.Select(it => it.Key.ToLower()).ToList();
                        var mappingType = types.FirstOrDefault(it => mapingTypes.Contains(it));
                        if (mappingType != null)
                        {
                            item.DataType = mappingType;
                        }
                        if (item.DataType == "varcharmax")
                        {
                            item.DataType = "nvarchar(max)";
                        }
                    }
                }
            }
            var tableName = GetTableName(entityInfo);
            this.Context.MappingTables.Add(entityInfo.EntityName, tableName);
            entityInfo.DbTableName = tableName;
            entityInfo.Columns.ForEach(it => {
                it.DbTableName = tableName;
                if (it.UnderType?.Name == "DateOnly" && it.DataType == null)
                {
                    it.DataType = "Date";
                }
                if (it.UnderType?.Name == "TimeOnly" && it.DataType == null)
                {
                    it.DataType = "Time";
                }
            });
            var isAny = this.Context.DbMaintenance.IsAnyTable(tableName, false);
            if (isAny && entityInfo.IsDisabledUpdateAll)
            {
                return;
            }
            if (isAny)
                ExistLogic(entityInfo);
            else
                NoExistLogic(entityInfo);

            this.Context.DbMaintenance.AddRemark(entityInfo);
            //this.Context.DbMaintenance.AddIndex(entityInfo);
            //base.CreateIndex(entityInfo);
            this.Context.DbMaintenance.AddDefaultValue(entityInfo);

            STable.Tags = null;
        }

        public override void ExistLogic(EntityInfo entityInfo)
        {
            if (entityInfo.Columns.HasValue() && entityInfo.IsDisabledUpdateAll == false)
            {
                //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Multiple primary keys do not support modifications");

                var tableName = GetTableName(entityInfo);
                var dbColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName, false);
                ConvertColumns(dbColumns);
                var attr =GetCommonSTableAttribute( entityInfo.Type.GetCustomAttribute<STableAttribute>());
                var entityColumns = entityInfo.Columns.Where(it => it.IsIgnore == false).ToList();
                if (attr != null && attr.Tag1 != null)
                {
                    entityColumns = entityInfo.Columns.Where(it => it.IsIgnore == false || string.IsNullOrEmpty(it.DbColumnName) ==false &&
                    ( it.DbColumnName?.ToLower() == attr.Tag1?.ToLower()
                     || it.DbColumnName?.ToLower() == attr.Tag2?.ToLower()
                      || it.DbColumnName?.ToLower() == attr.Tag3?.ToLower()
                       || it.DbColumnName?.ToLower() == attr.Tag4?.ToLower())  
                  ).ToList();
                    foreach (var item in entityColumns)
                    {
                        if (item.DbColumnName == null) 
                        {
                            item.DbColumnName = item.PropertyName;
                        }
                    }
                }
                var dropColumns = dbColumns
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(dc => !entityColumns.Any(ec => dc.DbColumnName.Equals(ec.DbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .ToList();
                var addColumns = entityColumns
                                          .Where(ec => ec.OldDbColumnName.IsNullOrEmpty() || !dbColumns.Any(dc => dc.DbColumnName.Equals(ec.OldDbColumnName, StringComparison.CurrentCultureIgnoreCase)))
                                          .Where(ec => !dbColumns.Any(dc => ec.DbColumnName.Equals(dc.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
  
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
                //foreach (var item in alterColumns)
                //{

                //    if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                //    {
                //        var entityColumnItem = entityColumns.FirstOrDefault(y => y.DbColumnName == item.DbColumnName);
                //        if (entityColumnItem != null && !string.IsNullOrEmpty(entityColumnItem.DataType))
                //        {
                //            continue;
                //        }
                //    }

                //    this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
                //    isChange = true;
                //}
                foreach (var item in renameColumns)
                {
                    this.Context.DbMaintenance.RenameColumn(tableName, item.OldDbColumnName, item.DbColumnName);
                    isChange = true;
                }
                //var isAddPrimaryKey = false;
                //foreach (var item in entityColumns)
                //{
                //    var dbColumn = dbColumns.FirstOrDefault(dc => dc.DbColumnName.Equals(item.DbColumnName, StringComparison.CurrentCultureIgnoreCase));
                //    if (dbColumn == null) continue;
                //    bool pkDiff, idEntityDiff;
                //    KeyAction(item, dbColumn, out pkDiff, out idEntityDiff);
                //    if (dbColumn != null && pkDiff && !idEntityDiff && isMultiplePrimaryKey == false)
                //    {
                //        var isAdd = item.IsPrimarykey;
                //        if (isAdd)
                //        {
                //            isAddPrimaryKey = true;
                //            this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
                //        }
                //        else
                //        {
                //            this.Context.DbMaintenance.DropConstraint(tableName, string.Format("PK_{0}_{1}", tableName, item.DbColumnName));
                //        }
                //    }
                //    else if ((pkDiff || idEntityDiff) && isMultiplePrimaryKey == false)
                //    {
                //        ChangeKey(entityInfo, tableName, item);
                //    }
                //}
                //if (isAddPrimaryKey == false && entityColumns.Count(it => it.IsPrimarykey) == 1 && dbColumns.Count(it => it.IsPrimarykey) == 0)
                //{
                //    //var addPk = entityColumns.First(it => it.IsPrimarykey);
                //    //this.Context.DbMaintenance.AddPrimaryKey(tableName, addPk.DbColumnName);
                //}
                //if (isMultiplePrimaryKey)
                //{
                //    var oldPkNames = dbColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                //    var newPkNames = entityColumns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName.ToLower()).OrderBy(it => it).ToList();
                //    if (!Enumerable.SequenceEqual(oldPkNames, newPkNames))
                //    {
                //        Check.Exception(true, ErrorMessage.GetThrowMessage("Modification of multiple primary key tables is not supported. Delete tables while creating", "不支持修改多主键表，请删除表在创建"));
                //    }

                //}
                if (isChange && IsBackupTable)
                {
                    this.Context.DbMaintenance.BackupTable(tableName, tableName + DateTime.Now.ToString("yyyyMMddHHmmss"), MaxBackupDataRows);
                }
                ExistLogicEnd(entityColumns);
            }
        }
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            List<DbColumnInfo> dbColumns = new List<DbColumnInfo>();
            foreach (var item in entityInfo.Columns.Where(it=>it.IsIgnore!=true).Where(it=>it.PropertyName!= "TagsTypeId").OrderBy(it=>it.UnderType==typeof(DateTime)?0:1))
            {
                var addItem = EntityColumnToDbColumn(entityInfo, entityInfo.DbTableName, item);
                dbColumns.Add(addItem);
            }
            var attr =GetCommonSTableAttribute( entityInfo.Type.GetCustomAttribute<STableAttribute>());
            var oldTableName = entityInfo.DbTableName;
            if (attr != null) 
            {
                entityInfo.DbTableName += ("{stable}"+this.Context.Utilities.SerializeObject(attr));
            }
            if (attr != null && attr.Tag1 != null)
            {
                dbColumns = dbColumns.Where(it =>
                 it.DbColumnName?.ToLower() != attr.Tag1?.ToLower()
                   && it.DbColumnName?.ToLower() != attr.Tag2?.ToLower()
                   && it.DbColumnName?.ToLower() != attr.Tag3?.ToLower()
                   && it.DbColumnName?.ToLower() != attr.Tag4?.ToLower()
                ).ToList();
            }
            var dbMain = (TDengineDbMaintenance)this.Context.DbMaintenance;
            dbMain.EntityInfo = entityInfo;
            dbMain.CreateTable(entityInfo.DbTableName, dbColumns);
            entityInfo.DbTableName = oldTableName;
        }
        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            DbColumnInfo result = new DbColumnInfo() { Length=item.Length,DecimalDigits=item.DecimalDigits,Scale=item.DecimalDigits ,TableName = tableName, DbColumnName = item.DbColumnName, DataType = item.DataType };
            if (result.DataType.IsNullOrEmpty())
            {
                result.DataType = GetDatabaseTypeName(item.UnderType.Name);
            }
            return result;
        }

        private STableAttribute GetCommonSTableAttribute(STableAttribute sTableAttribute)
        {
            return SqlSugar.TDengine.UtilMethods.GetCommonSTableAttribute(this.Context, sTableAttribute);
        }
        public string GetDatabaseTypeName(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bool":
                    return "BOOL";
                case "datetime":
                    return "TIMESTAMP";
                case "boolean":
                    return "BOOL";
                case "byte":
                    return "TINYINT UNSIGNED";
                case "sbyte":
                    return "TINYINT";
                case "char":
                    return "NCHAR";
                case "decimal":
                    return "FLOAT";
                case "double":
                    return "DOUBLE";
                case "float":
                case "single":
                    return "FLOAT";
                case "int":
                    return "INT";
                case "int32":
                    return "INT";
                case "int16":
                    return "INT";
                case "int64":
                    return "BIGINT";
                case "uint":
                case "uint32":
                    return "INT UNSIGNED";
                case "long":
                    return "BIGINT";
                case "ulong":
                case "uint64":
                    return "BIGINT UNSIGNED";
                case "short":
                    return "SMALLINT";
                case "ushort":
                case "uint16":
                    return "SMALLINT UNSIGNED";
                case "string":
                    return "VARCHAR";
                // 添加其他类型的映射关系

                default:
                    return "VARCHAR"; // 如果未识别到类型，则返回原始类型名称
            }
        }
    }
}