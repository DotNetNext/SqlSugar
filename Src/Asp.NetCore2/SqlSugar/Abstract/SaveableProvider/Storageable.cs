using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class Storageable<T> : IStorageable<T> where T : class, new()
    {
        SqlSugarProvider Context { get; set; }
        internal ISqlBuilder Builder;
        List<SugarParameter> Parameters;
        List<StorageableInfo<T>> allDatas = new List<StorageableInfo<T>>();
        List<T> dbDataList = new List<T>();
        List<KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>> whereFuncs = new List<KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>>();
        Expression<Func<T, object>> whereExpression;
        Func<DateTime, string> formatTime;
        DbLockType? lockType;
        private string asname { get; set; }
        private bool isDisableFilters = false;
        public Storageable(List<T> datas, SqlSugarProvider context)
        {
            this.Context = context;
            if (datas == null)
                datas = new List<T>();
            this.allDatas = datas.Select(it => new StorageableInfo<T>()
            {
                Item = it
            }).ToList();
        }

        Expression<Func<T, bool>> queryableWhereExp;
        public IStorageable<T> TableDataRange(Expression<Func<T, bool>> exp)
        {
            this.queryableWhereExp = exp;
            return this;
        }

        public IStorageable<T> SplitInsert(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Insert, conditions, message));
            return this;
        }
        public IStorageable<T> SplitDelete(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Delete, conditions, message));
            return this;
        }
        public IStorageable<T> SplitUpdate(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Update, conditions, message));
            return this;
        }

        public IStorageable<T> Saveable(string inserMessage = null,string updateMessage=null)
        {
            return this
                   .SplitUpdate(it => it.Any(),updateMessage)
                   .SplitInsert(it => true, inserMessage);
        }
        public IStorageable<T> SplitError(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Error, conditions, message));
            return this;
        }

        public IStorageable<T> SplitIgnore(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Ignore, conditions, message));
            return this;
        }

        public IStorageable<T> DisableFilters() 
        {
            this.isDisableFilters = true;
            return this;
        }

        public IStorageable<T> TranLock(DbLockType dbLockType = DbLockType.Wait) 
        {
            this.lockType = dbLockType;
            return this;
        }
        public IStorageable<T> TranLock(DbLockType? LockType)
        {
            if (LockType!=null)
            {
                this.lockType = LockType;
                return this;
            }
            else
            {
                return this;
            }
        }
        public IStorageable<T> SplitOther(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Other, conditions, message));
            return this;
        }
        public StorageablePage<T> PageSize(int PageSize,Action<int> ActionCallBack=null) 
        {
            if (PageSize > 10000) 
            {
                Check.ExceptionEasy("Advanced save page Settings should not exceed 10,000, and the reasonable number of pages is about 2000", "高级保存分页设置不要超过1万，合理分页数在2000左右");
            }
            StorageablePage<T> page = new StorageablePage<T>();
            page.Context = this.Context;
            page.PageSize = PageSize;
            page.Data = this.allDatas.Select(it => it.Item).ToList();
            page.ActionCallBack = ActionCallBack;
            page.TableName = this.asname;
            page.whereExpression = this.whereExpression;
            page.lockType = this.lockType;
            return page;
        }
        public StorageableSplitProvider<T> SplitTable() 
        {
            StorageableSplitProvider<T> result = new StorageableSplitProvider<T>();
            result.Context = this.Context;
            result.SaveInfo = this;
            result.List = allDatas.Select(it=>it.Item).ToList();
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfoWithAttr(typeof(T));
            return result;
        }
        public IStorageable<T> DefaultAddElseUpdate() 
        {
            var column = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.FirstOrDefault(it=>it.IsPrimarykey);
            if (column == null) Check.ExceptionEasy("DefaultAddElseUpdate() need primary key", "DefaultAddElseUpdate()这个方法只能用于主键");
            return this.SplitUpdate(it => 
            {
                var itemPkValue = column.PropertyInfo.GetValue(it.Item);
                var defaultValue =UtilMethods.GetDefaultValue(column.PropertyInfo.PropertyType);
                var result= itemPkValue != null && itemPkValue.ObjToString() != defaultValue.ObjToString();
                return result;

             }).SplitInsert(it => true);
        }

        public int ExecuteCommand() 
        {
            var result = 0;
            var x = this.ToStorage();
            result+=x.AsInsertable.ExecuteCommand();
            var updateRow = x.AsUpdateable.ExecuteCommand();
            if (updateRow < 0) updateRow = 0;
            result += updateRow;
            return result;
        }
        public Task<int> ExecuteCommandAsync(CancellationToken cancellationToken) 
        {
            this.Context.Ado.CancellationToken=cancellationToken;
            return ExecuteCommandAsync();
        }
        public async Task<int> ExecuteCommandAsync()
        {
            var result = 0;
            var x = await this.ToStorageAsync();
            result +=await x.AsInsertable.ExecuteCommandAsync();
            result +=await x.AsUpdateable.ExecuteCommandAsync();
            return result;
        }
        public int ExecuteSqlBulkCopy()
        {
            var storage = this.ToStorage();
            return storage.BulkCopy() + storage.BulkUpdate();
        }
        public async Task<int> ExecuteSqlBulkCopyAsync()
        {
            var storage =await this.ToStorageAsync();
            return await storage.BulkCopyAsync() + await storage.BulkUpdateAsync();
        }
        public StorageableResult<T> ToStorage()
        {
            if (whereFuncs == null || whereFuncs.Count == 0)
            {
                return this.Saveable().ToStorage();
            }
            if (this.allDatas.Count == 0)
                return new StorageableResult<T>() {
                    AsDeleteable = this.Context.Deleteable<T>().AS(asname).Where(it => false),
                    AsInsertable = this.Context.Insertable(new List<T>()).AS(asname),
                    AsUpdateable = this.Context.Updateable(new List<T>()).AS(asname),
                    InsertList = new List<StorageableMessage<T>>(),
                    UpdateList = new List<StorageableMessage<T>>(),
                    DeleteList = new List<StorageableMessage<T>>(),
                    ErrorList = new List<StorageableMessage<T>>(),
                    IgnoreList = new List<StorageableMessage<T>>(),
                    OtherList=new List<StorageableMessage<T>>(),
                    TotalList=new List<StorageableMessage<T>>()
                };
            var pkInfos = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey);
            if (whereExpression==null&&!pkInfos.Any())
            {
                Check.ExceptionEasy(true, "Need primary key or WhereColumn", "使用Storageable实体需要主键或者使用WhereColumn指定条件列");
            }
            if (whereExpression == null && pkInfos.Any())
            {
                this.Context.Utilities.PageEach(allDatas, 300, item => {
                 var addItems=this.Context.Queryable<T>().Filter(null, this.isDisableFilters).TranLock(this.lockType).AS(asname).WhereClassByPrimaryKey(item.Select(it => it.Item).ToList()).ToList();
                    dbDataList.AddRange(addItems);
                });
            }
            var pkProperties = GetPkProperties(pkInfos);
            var messageList = allDatas.Select(it => new StorageableMessage<T>()
            {
                Item = it.Item,
                Database = dbDataList,
                PkFields= pkProperties
            }).ToList();
            foreach (var item in whereFuncs.OrderByDescending(it => (int)it.key))
            {
                List<StorageableMessage<T>> whereList = messageList.Where(it => it.StorageType == null).ToList();
                Func<StorageableMessage<T>, bool> exp = item.value1;
                var list = whereList.Where(exp).ToList();
                foreach (var it in list)
                {
                    it.StorageType = item.key;
                    it.StorageMessage = item.value2;
                }
            }
            var delete = messageList.Where(it => it.StorageType == StorageType.Delete).ToList();
            var update = messageList.Where(it => it.StorageType == StorageType.Update).ToList();
            var inset = messageList.Where(it => it.StorageType == StorageType.Insert).ToList();
            var error = messageList.Where(it => it.StorageType == StorageType.Error).ToList();
            var ignore = messageList.Where(it => it.StorageType == StorageType.Ignore || it.StorageType == null).ToList();
            var other = messageList.Where(it => it.StorageType == StorageType.Other).ToList();
            StorageableResult<T> result = new StorageableResult<T>()
            {
                _WhereColumnList= wherecolumnList,
                _AsName =asname,
                _Context=this.Context,
                AsDeleteable = this.Context.Deleteable<T>().AS(asname),
                AsUpdateable = this.Context.Updateable(update.Select(it => it.Item).ToList()).AS(asname),
                AsInsertable = this.Context.Insertable(inset.Select(it => it.Item).ToList()).AS(asname),
                OtherList = other,
                InsertList = inset,
                DeleteList = delete,
                UpdateList = update,
                ErrorList = error,
                IgnoreList = ignore,
                TotalList = messageList
            };
            if (this.whereExpression != null)
            {
                result.AsUpdateable.WhereColumns(whereExpression);
                result.AsDeleteable.WhereColumns(update.Select(it => it.Item).ToList(),whereExpression);
            }
            if (this.whereExpression != null)
            {
                result.AsDeleteable.WhereColumns(delete.Select(it => it.Item).ToList(), whereExpression);
            }
            else
            {
                result.AsDeleteable.Where(delete.Select(it => it.Item).ToList());
            }
            return result;
        }

        public StorageableResult<T> GetStorageableResult()
        {
            if (whereFuncs == null || whereFuncs.Count == 0)
            {
                return this.Saveable().GetStorageableResult();
            }
            if (this.allDatas.Count == 0)
                return new StorageableResult<T>()
                {
                    //AsDeleteable = this.Context.Deleteable<T>().AS(asname).Where(it => false),
                    //AsInsertable = this.Context.Insertable(new List<T>()).AS(asname),
                    //AsUpdateable = this.Context.Updateable(new List<T>()).AS(asname),
                    InsertList = new List<StorageableMessage<T>>(),
                    UpdateList = new List<StorageableMessage<T>>(),
                    DeleteList = new List<StorageableMessage<T>>(),
                    ErrorList = new List<StorageableMessage<T>>(),
                    IgnoreList = new List<StorageableMessage<T>>(),
                    OtherList = new List<StorageableMessage<T>>(),
                    TotalList = new List<StorageableMessage<T>>()
                };
            var pkInfos = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey);
            if (whereExpression == null && !pkInfos.Any())
            {
                Check.ExceptionEasy(true, "Need primary key or WhereColumn", "使用Storageable实体需要主键或者使用WhereColumn指定条件列");
            }
            if (whereExpression == null && pkInfos.Any())
            {
                this.Context.Utilities.PageEach(allDatas, 300, item => {
                    var addItems = this.Context.Queryable<T>().Filter(null, this.isDisableFilters).TranLock(this.lockType).AS(asname).WhereClassByPrimaryKey(item.Select(it => it.Item).ToList()).ToList();
                    dbDataList.AddRange(addItems);
                });
            }
            var pkProperties = GetPkProperties(pkInfos);
            var messageList = allDatas.Select(it => new StorageableMessage<T>()
            {
                Item = it.Item,
                Database = dbDataList,
                PkFields = pkProperties
            }).ToList();
            foreach (var item in whereFuncs.OrderByDescending(it => (int)it.key))
            {
                List<StorageableMessage<T>> whereList = messageList.Where(it => it.StorageType == null).ToList();
                Func<StorageableMessage<T>, bool> exp = item.value1;
                var list = whereList.Where(exp).ToList();
                foreach (var it in list)
                {
                    it.StorageType = item.key;
                    it.StorageMessage = item.value2;
                }
            }
            var delete = messageList.Where(it => it.StorageType == StorageType.Delete).ToList();
            var update = messageList.Where(it => it.StorageType == StorageType.Update).ToList();
            var inset = messageList.Where(it => it.StorageType == StorageType.Insert).ToList();
            var error = messageList.Where(it => it.StorageType == StorageType.Error).ToList();
            var ignore = messageList.Where(it => it.StorageType == StorageType.Ignore || it.StorageType == null).ToList();
            var other = messageList.Where(it => it.StorageType == StorageType.Other).ToList();
            StorageableResult<T> result = new StorageableResult<T>()
            {
                _WhereColumnList = wherecolumnList,
                _AsName = asname,
                _Context = this.Context,
                //AsDeleteable = this.Context.Deleteable<T>().AS(asname),
                //AsUpdateable = this.Context.Updateable(update.Select(it => it.Item).ToList()).AS(asname),
                //AsInsertable = this.Context.Insertable(inset.Select(it => it.Item).ToList()).AS(asname),
                OtherList = other,
                InsertList = inset,
                DeleteList = delete,
                UpdateList = update,
                ErrorList = error,
                IgnoreList = ignore,
                TotalList = messageList
            };
            //if (this.whereExpression != null)
            //{
            //    result.AsUpdateable.WhereColumns(whereExpression);
            //    result.AsDeleteable.WhereColumns(update.Select(it => it.Item).ToList(), whereExpression);
            //}
            //result.AsDeleteable.Where(delete.Select(it => it.Item).ToList());
            return result;
        }

        public async Task<StorageableResult<T>> ToStorageAsync()
        {
            if (whereFuncs == null || whereFuncs.Count == 0)
            {
                return await this.Saveable().ToStorageAsync();
            }
            if (this.allDatas.Count == 0)
                return new StorageableResult<T>()
                {
                    AsDeleteable = this.Context.Deleteable<T>().AS(asname).Where(it => false),
                    AsInsertable = this.Context.Insertable(new List<T>()).AS(asname),
                    AsUpdateable = this.Context.Updateable(new List<T>()).AS(asname),
                    InsertList = new List<StorageableMessage<T>>(),
                    UpdateList = new List<StorageableMessage<T>>(),
                    DeleteList = new List<StorageableMessage<T>>(),
                    ErrorList = new List<StorageableMessage<T>>(),
                    IgnoreList = new List<StorageableMessage<T>>(),
                    OtherList = new List<StorageableMessage<T>>(),
                    TotalList = new List<StorageableMessage<T>>()
                };
            var pkInfos = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey);
            if (whereExpression == null && !pkInfos.Any())
            {
                Check.Exception(true, "Need primary key or WhereColumn");
            }
            if (whereExpression == null && pkInfos.Any())
            {
                await this.Context.Utilities.PageEachAsync(allDatas, 300,async item => {
                    var addItems =await this.Context.Queryable<T>().Filter(null,this.isDisableFilters).AS(asname).TranLock(this.lockType).WhereClassByPrimaryKey(item.Select(it => it.Item).ToList()).ToListAsync();
                    dbDataList.AddRange(addItems);
                });
            }
            var pkProperties = GetPkProperties(pkInfos);
            var messageList = allDatas.Select(it => new StorageableMessage<T>()
            {
                Item = it.Item,
                Database = dbDataList,
                PkFields = pkProperties
            }).ToList();
            foreach (var item in whereFuncs.OrderByDescending(it => (int)it.key))
            {
                List<StorageableMessage<T>> whereList = messageList.Where(it => it.StorageType == null).ToList();
                Func<StorageableMessage<T>, bool> exp = item.value1;
                var list = whereList.Where(exp).ToList();
                foreach (var it in list)
                {
                    it.StorageType = item.key;
                    it.StorageMessage = item.value2;
                }
            }
            var delete = messageList.Where(it => it.StorageType == StorageType.Delete).ToList();
            var update = messageList.Where(it => it.StorageType == StorageType.Update).ToList();
            var inset = messageList.Where(it => it.StorageType == StorageType.Insert).ToList();
            var error = messageList.Where(it => it.StorageType == StorageType.Error).ToList();
            var ignore = messageList.Where(it => it.StorageType == StorageType.Ignore || it.StorageType == null).ToList();
            var other = messageList.Where(it => it.StorageType == StorageType.Other).ToList();
            StorageableResult<T> result = new StorageableResult<T>()
            {
                _WhereColumnList = wherecolumnList,
                _AsName = asname,
                _Context = this.Context,
                AsDeleteable = this.Context.Deleteable<T>().AS(asname),
                AsUpdateable = this.Context.Updateable(update.Select(it => it.Item).ToList()).AS(asname),
                AsInsertable = this.Context.Insertable(inset.Select(it => it.Item).ToList()).AS(asname),
                OtherList = other,
                InsertList = inset,
                DeleteList = delete,
                UpdateList = update,
                ErrorList = error,
                IgnoreList = ignore,
                TotalList = messageList
            };
            if (this.whereExpression != null)
            {
                result.AsUpdateable.WhereColumns(whereExpression);
                result.AsDeleteable.WhereColumns(delete.Select(it => it.Item).ToList(),whereExpression);
            }
            result.AsDeleteable.Where(delete.Select(it => it.Item).ToList());
            return result;
        }


        private string[] GetPkProperties(IEnumerable<EntityColumnInfo> pkInfos)
        {
            if (whereExpression == null)
            {
                return pkInfos.Select(it => it.PropertyName).ToArray();
            }
            else
            {
                return wherecolumnList.Select(it => it.PropertyName).ToArray();
            }
        }
        List<EntityColumnInfo> wherecolumnList;
        public IStorageable<T> WhereColumns(Expression<Func<T, object>> columns, Func<DateTime, string> formatTime) 
        {
            this.formatTime = formatTime;
            return WhereColumns(columns);
        }
        public IStorageable<T> WhereColumns(Expression<Func<T, object>> columns)
        {

            if (columns == null)
                return this;
            else if (asname == null && typeof(T).GetCustomAttribute<SplitTableAttribute>() != null)
            {
                whereExpression = columns;
                return this;
            }
            else
            {
                List<string> list = GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => Builder.GetNoTranslationColumnName(it)).ToList();
                var dbColumns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsIgnore == false);
                var whereColumns = dbColumns.Where(it => list.Any(y =>
                                      it.DbColumnName.Equals(y, StringComparison.CurrentCultureIgnoreCase) ||
                                      it.PropertyName.Equals(y, StringComparison.CurrentCultureIgnoreCase))
                                  ).ToList();
                wherecolumnList = whereColumns;
                if (whereColumns.Count == 0)
                {
                    whereColumns = dbColumns.Where(it => it.IsPrimarykey).ToList();
                }
                if (whereColumns.Count > 0)
                {
                    if (queryableWhereExp == null)
                    {
                        this.Context.Utilities.PageEach(allDatas, 200, itemList =>
                        {
                            List<IConditionalModel> conditList = new List<IConditionalModel>();
                            SetConditList(itemList, whereColumns, conditList);
                            var addItem = this.Context.Queryable<T>().AS(asname)
                            .Filter(null, this.isDisableFilters)
                            .TranLock(this.lockType)
                            .Where(conditList,true).ToList();
                            this.dbDataList.AddRange(addItem);
                        });
                    }
                    else
                    {
                        this.dbDataList.AddRange(this.Context.Queryable<T>().AS(asname).Where(queryableWhereExp).ToList());
                    }
                }
                this.whereExpression = columns;
                return this;
            }
        }


        public IStorageable<T> WhereColumns(string [] columns) 
        {
            var list = columns.Select(it=>this.Context.EntityMaintenance.GetPropertyName<T>(it)).ToList();
            var exp=ExpressionBuilderHelper.CreateNewFields<T>(this.Context.EntityMaintenance.GetEntityInfo<T>(), list);
            return this.WhereColumns(exp);
        }
        public IStorageable<T> WhereColumns(string[] columns, Func<DateTime, string> formatTime)
        {
            this.formatTime = formatTime;
            return WhereColumns(columns);
        }
        private  void SetConditList(List<StorageableInfo<T>> itemList, List<EntityColumnInfo> whereColumns, List<IConditionalModel> conditList)
        {
           ;
            foreach (var dataItem in itemList)
            {
                var condition = new ConditionalCollections()
                {
                    ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()
                };
                conditList.Add(condition);
                int i = 0;
                foreach (var item in whereColumns)
                {
                    var value = item.PropertyInfo.GetValue(dataItem.Item, null);
                    if (value != null&&value.GetType().IsEnum()) 
                    {
                        if (this.Context.CurrentConnectionConfig.MoreSettings?.TableEnumIsString == true)
                        {
                            value = value.ToString();
                        }
                        else
                        {
                            value = Convert.ToInt64(value);
                        }
                    }
                    if (item.SqlParameterDbType != null && item.SqlParameterDbType is Type && UtilMethods.HasInterface((Type)item.SqlParameterDbType, typeof(ISugarDataConverter)))
                    {
                        var columnInfo = item;
                        var type = columnInfo.SqlParameterDbType as Type;
                        var ParameterConverter = type.GetMethod("ParameterConverter").MakeGenericMethod(columnInfo.PropertyInfo.PropertyType);
                        var obj = Activator.CreateInstance(type);
                        var p = ParameterConverter.Invoke(obj, new object[] { value, 100 }) as SugarParameter;
                        value = p.Value;
                    }
                    condition.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(i==0?WhereType.Or :WhereType.And, new ConditionalModel()
                    {
                        FieldName = item.DbColumnName,
                        ConditionalType = ConditionalType.Equal,
                        CSharpTypeName=UtilMethods.GetTypeName(value),
                        FieldValue = value==null?"null":value.ObjToString(formatTime),
                        FieldValueConvertFunc=this.Context.CurrentConnectionConfig.DbType==DbType.PostgreSQL? 
                                               UtilMethods.GetTypeConvert(value):null
                    }));
                    ++i;
                }
            }
        }

        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig); ;
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                resolveExpress.TableEnumIsString = this.Context.CurrentConnectionConfig.MoreSettings.TableEnumIsString;
                resolveExpress.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                resolveExpress.PgSqlIsAutoToLower = true;
            }
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            resolveExpress.InitMappingInfo = Context.InitMappingInfo;
            resolveExpress.RefreshMapping = () =>
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
                resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
                resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            };
            resolveExpress.Resolve(expression, resolveType);
            if (this.Parameters == null)
                this.Parameters = new List<SugarParameter>();
            this.Parameters.AddRange(resolveExpress.Parameters);
            var result = resolveExpress.Result;
            return result;
        }

        public IStorageable<T> As(string tableName)
        {
            this.asname = tableName;
            return this;
        }
    }
}
