using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class CodeFirstProvider : ICodeFirst
    {
        #region Properties
        internal static object LockObject = new object();
        public virtual SqlSugarProvider Context { get; set; }
        protected bool IsBackupTable { get; set; }
        protected int MaxBackupDataRows { get; set; }
        protected virtual int DefultLength { get; set; }
        protected Dictionary<Type, string> MappingTables = new Dictionary<Type, string>();
        public CodeFirstProvider()
        {
            if (DefultLength == 0)
            {
                DefultLength = 255;
            }
        }
        #endregion

        #region Public methods
        public SplitCodeFirstProvider SplitTables()
        {
            var result = new SplitCodeFirstProvider();
            result.Context = this.Context;
            return result;
        }

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
        public void InitTablesWithAttr(params Type[] entityTypes) 
        {
            foreach (var item in entityTypes)
            {
                var attr = item.GetCustomAttribute<TenantAttribute>();
                if (attr==null||this.Context?.Root == null)
                {
                    this.Context.CodeFirst.InitTables(item);
                }
                else
                {
                    var newDb = this.Context.Root.GetConnectionWithAttr(item);
                    newDb.CodeFirst.InitTables(item);
                }
            }
        }
        public virtual void InitTables(Type entityType)
        {
            var splitTableAttribute = entityType.GetCustomAttribute<SplitTableAttribute>();
            if (splitTableAttribute != null) 
            {
                var mappingInfo=this.Context.MappingTables.FirstOrDefault(it => it.EntityName == entityType.Name);
                if (mappingInfo == null) 
                {
                    UtilMethods.StartCustomSplitTable(this.Context,entityType);
                    this.Context.CodeFirst.SplitTables().InitTables(entityType);
                    this.Context.MappingTables.RemoveAll(it=>it.EntityName==entityType.Name);
                    UtilMethods.EndCustomSplitTable(this.Context, entityType);
                    return;
                }
            }
            //Prevent concurrent requests if used in your program
            lock (CodeFirstProvider.LockObject)
            {
                MappingTableList oldTableList = CopyMappingTalbe();
                //this.Context.Utilities.RemoveCacheAll();
                this.Context.InitMappingInfoNoCache(entityType);
                if (!this.Context.DbMaintenance.IsAnySystemTablePermissions())
                {
                    Check.Exception(true, "Dbfirst and  Codefirst requires system table permissions");
                }
                Check.Exception(this.Context.IsSystemTablesConfig, "Please set SqlSugarClent Parameter ConnectionConfig.InitKeyType=InitKeyType.Attribute ");

                if (this.Context.Ado.Transaction == null)
                {
                    var executeResult = Context.Ado.UseTran(() =>
                    {
                        Execute(entityType);
                    });
                    Check.Exception(!executeResult.IsSuccess, executeResult.ErrorMessage);
                }
                else
                {
                    Execute(entityType);
                }

                RestMappingTables(oldTableList);
            }

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
        public void InitTables<T, T2, T3, T4,T5>()
        {
            InitTables(typeof(T), typeof(T2), typeof(T3), typeof(T4),typeof(T5));
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
        public ICodeFirst As(Type type, string newTableName) 
        {
            if (!MappingTables.ContainsKey(type)) 
            {
                MappingTables.Add(type,newTableName);
            }
            else  
            {
                MappingTables[type]= newTableName;
            }
            return this;
        }
        public ICodeFirst As<T>(string newTableName)
        {
            return As(typeof(T),newTableName);
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
        public TableDifferenceProvider GetDifferenceTables<T>()
        {
            var type = typeof(T);
            return GetDifferenceTables(type);
        }

        public TableDifferenceProvider GetDifferenceTables(params Type[] types)
        {
            TableDifferenceProvider result = new TableDifferenceProvider();
            foreach (var type in types)
            {
                GetDifferenceTables(result, type);
            }
            return result;
        }
        #endregion

        #region Core Logic
        private void GetDifferenceTables(TableDifferenceProvider result, Type type)
        {
            var tempTableName = "TempDiff" + DateTime.Now.ToString("yyMMssHHmmssfff");
            var oldTableName = this.Context.EntityMaintenance.GetEntityInfo(type).DbTableName;
            var db = new SqlSugarProvider(UtilMethods.CopyConfig(this.Context.CurrentConnectionConfig));
            UtilMethods.IsNullReturnNew(db.CurrentConnectionConfig.ConfigureExternalServices);
            db.CurrentConnectionConfig.ConfigureExternalServices.EntityNameService += (x, p) =>
            {
                p.IsDisabledUpdateAll = true;//Disabled update
            };
            db.MappingTables = new MappingTableList();
            db.MappingTables.Add(type.Name, tempTableName);
            try
            {

                var codeFirst=db.CodeFirst;
                codeFirst.SetStringDefaultLength(this.DefultLength);
                codeFirst.InitTables(type);
                var tables = db.DbMaintenance.GetTableInfoList(false);
                var oldTableInfo = tables.FirstOrDefault(it=>it.Name.EqualCase(oldTableName));
                var newTableInfo = tables.FirstOrDefault(it => it.Name.EqualCase(oldTableName));
                var oldTable = db.DbMaintenance.GetColumnInfosByTableName(oldTableName, false);
                var tempTable = db.DbMaintenance.GetColumnInfosByTableName(tempTableName, false);
                if (oldTableInfo == null)
                {
                    oldTableInfo =new DbTableInfo() { Name = "还未创建:" + oldTableName };
                    newTableInfo = new DbTableInfo() { Name = "还未创建:" + oldTableName };
                }
                result.tableInfos.Add(new DiffTableInfo()
                {
                     OldTableInfo= oldTableInfo,
                     NewTableInfo = newTableInfo,
                     OldColumnInfos =  oldTable,
                     NewColumnInfos = tempTable
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                db.DbMaintenance.DropTable(tempTableName);
            }
        }
        protected virtual void Execute(Type entityType)
        {
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfoNoCache(entityType);
            if (entityInfo.Discrimator.HasValue())
            {
                Check.ExceptionEasy(!Regex.IsMatch(entityInfo.Discrimator, @"^(?:\w+:\w+)(?:,\w+:\w+)*$"), "The format should be type:cat for this type, and if there are multiple, it can be FieldName:cat,FieldName2:dog ", "格式错误应该是type:cat这种格式，如果是多个可以FieldName:cat,FieldName2:dog，不要有空格");
                var array = entityInfo.Discrimator.Split(',');
                foreach (var disItem in array)
                {
                    var name = disItem.Split(':').First();
                    var value = disItem.Split(':').Last();
                    entityInfo.Columns.Add(new EntityColumnInfo() {  PropertyInfo=typeof(DiscriminatorObject).GetProperty(nameof(DiscriminatorObject.FieldName)),IsOnlyIgnoreUpdate = true, DbColumnName = name, UnderType = typeof(string), PropertyName = name, Length = 50 });
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
            this.Context.DbMaintenance.AddIndex(entityInfo);
            CreateIndex(entityInfo);
            this.Context.DbMaintenance.AddDefaultValue(entityInfo);
        }

        private void CreateIndex(EntityInfo entityInfo)
        {
            if (entityInfo.Indexs.HasValue())
            {
                foreach (var item in entityInfo.Indexs)
                {
                    if (entityInfo.Type.GetCustomAttribute<SplitTableAttribute>() != null) 
                    {
                        item.IndexName = item.IndexName + entityInfo.DbTableName;
                    }
                    if (this.Context.CurrentConnectionConfig.IndexSuffix.HasValue()) 
                    {
                        item.IndexName = (this.Context.CurrentConnectionConfig.IndexSuffix+ item.IndexName);
                    }
                    var include = "";
                    if (item.IndexName != null)
                    {
                        var database = "{db}";
                        if (item.IndexName.Contains(database))
                        {
                            item.IndexName = item.IndexName.Replace(database, this.Context.Ado.Connection.Database);
                        }
                        var table = "{table}";
                        if (item.IndexName.Contains(table))
                        {
                            item.IndexName = item.IndexName.Replace(table, entityInfo.DbTableName);
                        }
                        if (item.IndexName.ToLower().Contains("{include:")) 
                        {
                            include=Regex.Match( item.IndexName,@"\{include\:.+$").Value;
                            item.IndexName = item.IndexName.Replace(include, "");
                        }
                        if (item.IndexName.Contains(".")&& item.IndexName.Contains("["))
                        {
                            item.IndexName = item.IndexName.Replace(".","_");
                            item.IndexName = item.IndexName.Replace("[", "").Replace("]", "");
                        }
                    }
                    if (!this.Context.DbMaintenance.IsAnyIndex(item.IndexName))
                    {
                        var querybulder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
                        querybulder.Context = this.Context;
                        var fileds = item.IndexFields
                            .Select(it =>
                            {
                                var dbColumn = entityInfo.Columns.FirstOrDefault(z => z.PropertyName == it.Key);
                                if (dbColumn == null&&entityInfo.Discrimator==null)
                                {
                                    Check.ExceptionEasy($"{entityInfo.EntityName} no   SugarIndex[ {it.Key} ]  found", $"类{entityInfo.EntityName} 索引特性没找到列 ：{it.Key}");
                                }
                                return new KeyValuePair<string, OrderByType>(dbColumn.DbColumnName, it.Value);
                            })
                            .Select(it => querybulder.GetTranslationColumnName(it.Key) + " " + it.Value).ToArray();
                        this.Context.DbMaintenance.CreateIndex(entityInfo.DbTableName, fileds, item.IndexName+ include, item.IsUnique);
                    }
                }
            }
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
                if (entityInfo.IsCreateTableFiledSort)
                {
                    columns = columns.OrderBy(c => c.CreateTableFieldSort).ToList();
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
                var dbColumns = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName,false);
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
                                                          dbColumns.Any(dc => dc.DbColumnName.EqualCase(ec.DbColumnName)
                                                               && ((ec.Length != dc.Length && !UtilMethods.GetUnderType(ec.PropertyInfo).IsEnum() && UtilMethods.GetUnderType(ec.PropertyInfo).IsIn(UtilConstants.StringType)) ||
                                                                    ec.IsNullable != dc.IsNullable ||
                                                                    IsNoSamePrecision(ec, dc) ||
                                                                    IsNoSamgeType(ec, dc)))).ToList();
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

                    if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle) 
                    {
                        var entityColumnItem = entityColumns.FirstOrDefault(y => y.DbColumnName == item.DbColumnName);
                        if (entityColumnItem!=null&&!string.IsNullOrEmpty(entityColumnItem.DataType)) 
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
                if (isAddPrimaryKey==false&&entityColumns.Count(it => it.IsPrimarykey)==1&&dbColumns.Count(it => it.IsPrimarykey) ==0) 
                {
                    var addPk=entityColumns.First(it => it.IsPrimarykey);
                    this.Context.DbMaintenance.AddPrimaryKey(tableName, addPk.DbColumnName);
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
                ExistLogicEnd(entityColumns);
            }
        }

        private bool IsNoSamePrecision(EntityColumnInfo ec, DbColumnInfo dc)
        {
            if (this.Context.CurrentConnectionConfig.MoreSettings?.EnableCodeFirstUpdatePrecision == true) 
            {
                return ec.DecimalDigits != dc.DecimalDigits && ec.UnderType.IsIn(UtilConstants.DobType,UtilConstants.DecType);
            }
            return false;
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

        protected virtual void ExistLogicEnd(List<EntityColumnInfo> dbColumns) 
        {

        }
        protected virtual void ConvertColumns(List<DbColumnInfo> dbColumns)
        {

        }
        #endregion

        #region Helper methods
        private void RestMappingTables(MappingTableList oldTableList)
        {
            this.Context.MappingTables.Clear();
            foreach (var table in oldTableList)
            {
                this.Context.MappingTables.Add(table.EntityName, table.DbTableName);
            }
        }
        private MappingTableList CopyMappingTalbe()
        {
            MappingTableList oldTableList = new MappingTableList();
            if (this.Context.MappingTables == null) 
            {
                this.Context.MappingTables = new MappingTableList();
            }
            foreach (var table in this.Context.MappingTables)
            {
                oldTableList.Add(table.EntityName, table.DbTableName);
            }
            return oldTableList;
        }

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
                DecimalDigits = item.DecimalDigits,
                CreateTableFieldSort = item.CreateTableFieldSort
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

        protected virtual bool IsNoSamgeType(EntityColumnInfo ec, DbColumnInfo dc)
        {
            if (!string.IsNullOrEmpty(ec.DataType))
            {
                if (ec.IsIdentity && dc.IsIdentity)
                {
                    return false;
                }
                else
                {
                    return ec.DataType != dc.DataType;
                }
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
            if (properyTypeName?.ToLower() == "varchar" && dataType?.ToLower() == "string") 
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "varchar" && dataType?.ToLower() == "nvarchar")
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "number" && dataType?.ToLower() == "decimal")
            {
                return false;
            }
            if (this.Context.CurrentConnectionConfig?.MoreSettings?.EnableOracleIdentity==true&&properyTypeName?.ToLower() == "int" && dataType?.ToLower() == "decimal")
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "int" && dataType?.ToLower() == "decimal"&&dc.Length==22&&dc.Scale==0&&this.Context.CurrentConnectionConfig.DbType==DbType.Oracle)
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "int" && dataType?.ToLower() == "int32")
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "date" && dataType?.ToLower() == "datetime")
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "bigint" && dataType?.ToLower() == "int64")
            {
                return false;
            }
            if (properyTypeName?.ToLower() == "blob" && dataType?.ToLower() == "byte[]")
            {
                return false;
            }
            if (properyTypeName == null || dataType == null)
            {
                return properyTypeName != dataType;
            }
            else if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer && dataType.EqualCase("timestamp") && properyTypeName.EqualCase("varbinary"))
            {
                return false;
            }
            else if (properyTypeName.IsIn("int", "long") && dataType.EqualCase("decimal") && dc.Length == 38 && dc.DecimalDigits == 127)
            {
                return false;
            }
            else if (dataType.EqualCase("numeric") && properyTypeName.EqualCase("decimal"))
            {
                return false;
            }
            else if (ec.UnderType == UtilConstants.BoolType && dc.OracleDataType?.EqualCase("number")==true) 
            {
                return false;
            }
            else
            {
                return properyTypeName.ToLower() != dataType.ToLower();
            }
        }
        protected  string GetType(string name)
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
