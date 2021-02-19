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
            if (this.allDatas.Count == 0)
                return new StorageableResult<T>();
            var pkInfos = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey);
            if (whereExpression==null&&!pkInfos.Any())
            {
                Check.Exception(true, "Need primary key or WhereColumn");
            }
            if (whereExpression == null && pkInfos.Any())
            {
                this.Context.Utilities.PageEach(allDatas, 300, item => {
                 var addItems=this.Context.Queryable<T>().WhereClassByPrimaryKey(item.Select(it => it.Item).ToList()).ToList();
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
                AsDeleteable = this.Context.Deleteable<T>(),
                AsUpdateable = this.Context.Updateable(update.Select(it => it.Item).ToList()),
                AsInsertable = this.Context.Insertable(inset.Select(it => it.Item).ToList()),
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
                        var addItem = this.Context.Queryable<T>().Where(conditList).ToList();
                        this.dbDataList.AddRange(addItem);
                    });
                }
                this.whereExpression = columns;
                return this;
            }
        }

        private static void SetConditList(List<StorageableInfo<T>> itemList, List<EntityColumnInfo> whereColumns, List<IConditionalModel> conditList)
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
                    condition.ConditionalList.Add(new KeyValuePair<WhereType, ConditionalModel>(i==0?WhereType.Or :WhereType.And, new ConditionalModel()
                    {
                        FieldName = item.DbColumnName,
                        ConditionalType = ConditionalType.Equal,
                        FieldValue = item.PropertyInfo.GetValue(dataItem.Item, null) + ""
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
    }
}
