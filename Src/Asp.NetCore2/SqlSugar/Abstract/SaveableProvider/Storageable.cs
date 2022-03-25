using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
        private string asname { get; set; }
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

        public IStorageable<T> SplitOther(Func<StorageableInfo<T>, bool> conditions, string message = null)
        {
            whereFuncs.Add(new KeyValuePair<StorageType, Func<StorageableInfo<T>, bool>, string>(StorageType.Other, conditions, message));
            return this;
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
                Check.Exception(true, "Need primary key or WhereColumn");
            }
            if (whereExpression == null && pkInfos.Any())
            {
                this.Context.Utilities.PageEach(allDatas, 300, item => {
                 var addItems=this.Context.Queryable<T>().AS(asname).WhereClassByPrimaryKey(item.Select(it => it.Item).ToList()).ToList();
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
                result.AsDeleteable.WhereColumns(whereExpression);
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
        public IStorageable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            if (columns == null)
                return this;
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
                    this.Context.Utilities.PageEach(allDatas, 200, itemList =>
                    {
                        List<IConditionalModel> conditList = new List<IConditionalModel>();
                        SetConditList(itemList, whereColumns, conditList);
                        var addItem = this.Context.Queryable<T>().AS(asname).Where(conditList).ToList();
                        this.dbDataList.AddRange(addItem);
                    });
                }
                this.whereExpression = columns;
                return this;
            }
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
                    condition.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(i==0?WhereType.Or :WhereType.And, new ConditionalModel()
                    {
                        FieldName = item.DbColumnName,
                        ConditionalType = ConditionalType.Equal,
                        CSharpTypeName=UtilMethods.GetTypeName(value),
                        FieldValue = value + "",
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
