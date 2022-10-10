using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Dynamic;
using System.Threading.Tasks;

namespace SqlSugar
{
    #region T1
    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public SqlSugarProvider Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public IDbBind Bind { get { return this.Db.DbBind; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public MappingTableList QueryableMappingTableList { get; set; }
        public List<Action<T>> MapperAction { get; set; }
        public Action<T, MapperCache<T>> MapperActionWithCache { get; set; }
        public List<Action<List<T>>> Mappers { get; set; }
        public bool IsCache { get; set; }
        public int CacheTime { get; set; }
        public string CacheKey { get; set; }
        public bool IsAs { get; set; }
        public QueryBuilder QueryBuilder
        {
            get
            {
                return this.SqlBuilder.QueryBuilder;
            }
            set
            {
                this.SqlBuilder.QueryBuilder = value;
            }
        }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        //public ISugarQueryable<T> CrossQueryWithAttr() 
        //{
        //    this.QueryBuilder.IsCrossQueryWithAttr = true;
        //    return this;
        //}
        public ISugarQueryable<T> CrossQuery<Type>(string configId)
        {
            return this.CrossQuery(typeof(Type),configId);
        }
        public ISugarQueryable<T> CrossQuery(Type type, string configId)
        {
            if (this.QueryBuilder.CrossQueryItems == null) 
            {
                this.QueryBuilder.CrossQueryItems = new Dictionary<string, string>();
            }
            if(!this.QueryBuilder.CrossQueryItems.ContainsKey(type.FullName))
               this.QueryBuilder.CrossQueryItems.Add(type.FullName, configId);
            return this;
        }
        public ISugarQueryable<T> IncludeLeftJoin(Expression<Func<T, object>> LeftObject)
        {
            MemberExpression memberExpression;
            string navObjectName;
            EntityColumnInfo navColumn, navPkColumn;
            EntityInfo navEntityInfo;
            ExpressionTool.GetOneToOneInfo(this.Context,LeftObject, out memberExpression, out navObjectName, out navColumn, out navEntityInfo, out navPkColumn);
            var shortName = $"pnv_{navObjectName}";
            var mainShortName = memberExpression.Expression.ToString();
            this.QueryBuilder.TableShortName = mainShortName;
            var onWhere = $"{shortName}.{navPkColumn.DbColumnName}={mainShortName}.{navColumn.DbColumnName}";
            UtilMethods.IsNullReturnNew(this.Context.TempItems);
            this.AddJoinInfo(navEntityInfo.DbTableName, shortName, onWhere, JoinType.Left);
            return this;
        }

        public ISugarQueryable<T, T2> LeftJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> InnerJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> RightJoin<T2>(ISugarQueryable<T2> joinQueryable, Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            this.QueryBuilder.IsSqlQuery = false;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression,JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2> FullJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpression) 
        {
            this.Context.InitMappingInfo<T2>();
            var result = InstanceFactory.GetQueryable<T, T2>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        public void Clear()
        {
            QueryBuilder.Clear();
        }
        public ISugarQueryable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = QueryBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it).ToLower()).ToList();
            return IgnoreColumns(ignoreColumns.ToArray());
        }
        public ISugarQueryable<T> IgnoreColumns(params string[] columns)
        {
            if (QueryBuilder.IgnoreColumns.IsNullOrEmpty())
            {
                QueryBuilder.IgnoreColumns = new List<string>();
            }
            QueryBuilder.IgnoreColumns.AddRange(columns);
            return this;
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public ISugarQueryable<T> Clone()
        {
            var queryable = this.Context.Queryable<object>().Select<T>().WithCacheIF(IsCache, CacheTime);
            CopyQueryBuilder(queryable.QueryBuilder);
            ((QueryableProvider<T>)queryable).CacheKey = this.CacheKey;
            ((QueryableProvider<T>)queryable).MapperAction = this.MapperAction;
            ((QueryableProvider<T>)queryable).MapperActionWithCache = this.MapperActionWithCache;
            ((QueryableProvider<T>)queryable).Mappers = this.Mappers;
            return queryable;
        }
        public virtual ISugarQueryable<T> AS<T2>(string tableName)
        {
            var entityName = typeof(T2).Name;
            return _As(tableName, entityName);
        }
        public ISugarQueryable<T> AS(string tableName)
        {
            if (tableName == null) return this;
            var entityName = typeof(T).Name;
            return _As(tableName, entityName);
        }
        public virtual ISugarQueryable<T> With(string withString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
            {
                QueryBuilder.TableWithString = withString;
            }
            return this;
        }

        public virtual ISugarQueryable<T> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public ISugarQueryable<T> Filter(Type type) 
        {
            this.Context.InitMappingInfo(type);
            var whereString= QueryBuilder.GetFilters(type);
            if (whereString.HasValue()) 
            {
                this.Where(whereString);
            }
            return this;
        }
        public virtual ISugarQueryable<T> Mapper(Action<T> mapperAction)
        {
            this.MapperAction=UtilMethods.IsNullReturnNew(this.MapperAction);
            this.MapperAction.Add(mapperAction);
            return this;
        }
        public ISugarQueryable<T> Mapper<AType, BType, MappingType>(Expression<Func<MappingType, ManyToMany>> expression)
        {
            var args = ((expression as LambdaExpression).Body as MethodCallExpression).Arguments;

            Type aType = typeof(AType);
            Type bType = typeof(BType);
            Type bListType = typeof(List<BType>);

            this.Context.InitMappingInfo(aType);
            this.Context.InitMappingInfo(bType);
            this.Context.InitMappingInfo(typeof(MappingType));

            //Mapping 
            var mappingEntity = this.Context.EntityMaintenance.GetEntityInfo(typeof(MappingType));
            string m_aPropertyName = (args[0] as MemberExpression).Member.Name;
            string m_bPropertyName= (args[1] as MemberExpression).Member.Name;
            var m_aDbField = mappingEntity.Columns.First(it => it.PropertyName == m_aPropertyName).DbColumnName;
            var m_bDbField = mappingEntity.Columns.First(it => it.PropertyName == m_bPropertyName).DbColumnName;

            //A
            var aEntity = this.Context.EntityMaintenance.GetEntityInfo(aType);
            var aPropertyName = aEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.PropertyName;
            Check.Exception(aPropertyName == null, aEntity.EntityName + " no primary key");

            //B
            var bEntity = this.Context.EntityMaintenance.GetEntityInfo(bType);
            var bProperty = bEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true)?.PropertyName;
            Check.Exception(bProperty == null, bEntity.EntityName + " no primary key");
            var bDbFiled = bEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true).DbColumnName;
            this.Mapper((it,cache) =>
            {
               var list= cache.Get<Dictionary<object, List<BType>>>(oldList=> {


                   //query mapping by a
                   var cons = new List<IConditionalModel>() {
                     new ConditionalModel(){      
                                                   ConditionalType=ConditionalType.In,
                                                   FieldName= m_aDbField,
                                                   FieldValue=string.Join(",",oldList.Select(z=>UtilMethods.GetPropertyValue(z,aPropertyName)).Distinct())
                                                  }
                   };
                   var mappingList = this.Context.Queryable<MappingType>().Where(cons).ToList();
                   var bids = mappingList.Select(z => UtilMethods.GetPropertyValue(z, m_bPropertyName)).Distinct().ToList();

                   //queryable b by mapping
                   cons = new List<IConditionalModel>() {
                     new ConditionalModel(){
                                                   ConditionalType=ConditionalType.In,
                                                   FieldName= bDbFiled,
                                                   FieldValue=string.Join(",",mappingList.Select(z=>UtilMethods.GetPropertyValue(z,m_bPropertyName)).Distinct())
                                                  }
                   };
                   List<BType> bList = new List<BType>();
                   if (mappingList.Any())
                   {
                       bList=this.Context.Queryable<BType>().Where(cons).ToList();
                   }

                   //get result
                   Dictionary<object, List<BType>> result = new Dictionary<object, List<BType>>();
                   var group = mappingList.GroupBy(z => UtilMethods.GetPropertyValue(z, m_aPropertyName));
                   foreach (var item in group)
                   {
                       var currentBids = item.Select(z => UtilMethods.GetPropertyValue(z, m_bPropertyName)).ToList();
                       result.Add(item.Key, bList.Where(z => currentBids.Contains(UtilMethods.GetPropertyValue(z, bProperty))).ToList());
                   }
                   return result;

               }, expression.ToString());
               foreach (var item in aEntity.Columns)
               {
                    var aid = UtilMethods.GetPropertyValue(it, aPropertyName);
                    if (list.ContainsKey(aid))
                    {
                        if (item.PropertyInfo.PropertyType == bType)
                        {
                           var b=UtilMethods.ChangeType<BType>(list[aid].FirstOrDefault());
                            item.PropertyInfo.SetValue(it, b);
                        }
                        else if (item.PropertyInfo.PropertyType == bListType)
                        {
                            var bList = UtilMethods.ChangeType<List<BType>>(list[aid]);
                            item.PropertyInfo.SetValue(it, bList);
                        }
                    }
               }
            });
            return this;
        }

        public virtual ISugarQueryable<T> Mapper(Action<T, MapperCache<T>> mapperAction)
        {
            this.MapperActionWithCache += mapperAction;
            return this;
        }
        public ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField)
        {
            Check.Exception(mapperObject.ReturnType.Name == "IList`1", "Mapper no support IList , Use List<T>");
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToOne, FillExpression = mapperObject, MappingField1Expression = mainField, MappingField2Expression = childField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mainField, childField);
        }
        public ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mainField, Expression<Func<T, object>> childField)
        {
            return _Mapper<TObject>(mapperObject, mainField, childField);
        }
        public virtual ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, List<TObject>>> mapperObject, Expression<Func<T, object>> mapperField)
        {
            Check.Exception(mapperObject.ReturnType.Name == "IList`1", "Mapper no support IList , Use List<T>");
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToN, FillExpression = mapperObject, MappingField1Expression = mapperField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mapperField);
        }
        public virtual ISugarQueryable<T> Mapper<TObject>(Expression<Func<T, TObject>> mapperObject, Expression<Func<T, object>> mapperField)
        {
            if (CallContext.MapperExpression.Value == null)
            {
                CallContext.MapperExpression.Value = new List<MapperExpression>();
            }
            CallContext.MapperExpression.Value.Add(new MapperExpression() { SqlBuilder = SqlBuilder, QueryBuilder = this.QueryBuilder, Type = MapperExpressionType.oneToOne, FillExpression = mapperObject, MappingField1Expression = mapperField, Context = this.Context });
            return _Mapper<TObject>(mapperObject, mapperField);
        }
        public ISugarQueryable<T> Where(string fieldName, string conditionalType, object fieldValue)
        {
            string parameterName = fieldName+ this.QueryBuilder.WhereIndex;
            var whereSql = this.SqlBuilder.GetWhere(fieldName, conditionalType, this.QueryBuilder.WhereIndex);
            this.Where(whereSql);
            this.QueryBuilder.WhereIndex++;
            this.QueryBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public virtual ISugarQueryable<T> AddParameters(SugarParameter parameter)
        {
            if (parameter != null)
                QueryBuilder.Parameters.Add(parameter);
            return this;
        }

        public virtual ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {

            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public ISugarQueryable<T> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            return WhereClass(new List<ClassType>() { whereClass }, ignoreDefaultValue);
        }
        public ISugarQueryable<T> WhereClassByPrimaryKey(List<T> list)
        {
            _WhereClassByPrimaryKey(list);
            return this;
        }
        public ISugarQueryable<T> WhereClassByPrimaryKey(T data)
        {
            _WhereClassByPrimaryKey(new List<T>() { data });
            return this;
        }
        public ISugarQueryable<T> TranLock(DbLockType? LockType = DbLockType.Wait) 
        {
            if (LockType == null) return this;
            Check.ExceptionEasy(this.Context.Ado.Transaction == null, "need BeginTran", "需要事务才能使用TranLock");
            Check.ExceptionEasy(this.QueryBuilder.IsSingle()==false, "TranLock, can only be used for single table query", "TranLock只能用在单表查询");
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
            {
                if (LockType == DbLockType.Wait)
                {
                    this.With("WITH(UpdLock,RowLock)");
                }
                else
                {
                    this.With("WITH(UpdLock,RowLock,NoWait)");
                }
            }
            else 
            {
                this.QueryBuilder.TranLock = (LockType == DbLockType.Error? " for update nowait" : " for update");
            }
            return this;
        }
        public ISugarQueryable<T> WhereColumns(Dictionary<string, object>  dictionary) 
        {
            return WhereColumns(new List<Dictionary<string, object>> { dictionary });
        }
        public ISugarQueryable<T> WhereColumns(Dictionary<string, object> columns, bool ignoreDefaultValue) 
        {
            if (ignoreDefaultValue == false || columns == null)
            {
                return WhereColumns(columns);
            }
            else 
            {
                var newColumns = new Dictionary<string, object>();
                foreach (var item in columns)
                {
                    if (!UtilMethods.IsDefaultValue(item.Value)) 
                    {
                        newColumns.Add(item.Key, item.Value);
                    }
                }
                return WhereColumns(newColumns);
            }
        }
        public ISugarQueryable<T> WhereColumns(List<Dictionary<string, object>> list)
        {
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            foreach (var model in list)
            {
                int i = 0;
                var clist = new List<KeyValuePair<WhereType, ConditionalModel>>();
                foreach (var item in model.Keys)
                {
                    clist.Add(new KeyValuePair<WhereType, ConditionalModel>(i == 0 ? WhereType.Or : WhereType.And, new ConditionalModel()
                    {
                        FieldName = item,
                        ConditionalType = ConditionalType.Equal,
                        FieldValue = model[item]==null?"null" : model[item].ObjToString(),
                        CSharpTypeName = model[item] == null ? null : model[item].GetType().Name
                    }));
                    i++;
                }
                conditionalModels.Add(new ConditionalCollections()
                {
                    ConditionalList = clist
                });
            }
            return this.Where(conditionalModels);
        }

        /// <summary>
        ///  if a property that is primary key is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public ISugarQueryable<T> _WhereClassByPrimaryKey(List<T> whereClassTypes)
        {

            if (whereClassTypes.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsIgnore == false && it.IsPrimarykey == true).ToList();
                Check.Exception(columns == null || columns.Count == 0, "{0} no primary key, Can not use WhereClassByPrimaryKey ", typeof(T).Name);
                Check.Exception(this.QueryBuilder.IsSingle() == false, "No support join query");
                List<IConditionalModel> whereModels = new List<IConditionalModel>();
                foreach (var item in whereClassTypes)
                {
                    var cons = new ConditionalCollections();
                    foreach (var column in columns)
                    {
                        WhereType WhereType = WhereType.And;
                        var value = column.PropertyInfo.GetValue(item, null);
                        if (cons.ConditionalList == null)
                        {
                            cons.ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
                            if (QueryBuilder.WhereInfos.IsNullOrEmpty() && whereModels.IsNullOrEmpty())
                            {

                            }
                            else
                            {
                                WhereType = WhereType.Or;
                            }
                        }
                        var data = new KeyValuePair<WhereType, ConditionalModel>(WhereType, new ConditionalModel()
                        {
                            ConditionalType = ConditionalType.Equal,
                            FieldName = this.QueryBuilder.Builder.GetTranslationColumnName(column.DbColumnName),
                            FieldValue = value.ObjToStringNew(),
                            CSharpTypeName = column.PropertyInfo.PropertyType.Name
                        });
                        //if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL) 
                        //{
                        //    data.Value.FieldValueConvertFunc = it =>
                        //    {
                        //        return UtilMethods.ChangeType2(it, value.GetType());
                        //    };
                        //}
                        cons.ConditionalList.Add(data);
                    }
                    if (cons.HasValue())
                    {
                        whereModels.Add(cons);
                    }
                }
                this.Where(whereModels);
            }
            else
            {
                this.Where(" 1=2 ");
            }
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public ISugarQueryable<T> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            if (whereClassTypes.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<ClassType>().Columns.Where(it => it.IsIgnore == false).ToList();
                List<IConditionalModel> whereModels = new List<IConditionalModel>();
                foreach (var item in whereClassTypes)
                {
                    var cons = new ConditionalCollections();
                    foreach (var column in columns)
                    {

                        var value = column.PropertyInfo.GetValue(item, null);
                        WhereType WhereType = WhereType.And;
                        var isNotNull = ignoreDefaultValue == false && value != null;
                        var isNotNullAndDefault = ignoreDefaultValue && value != null && value.ObjToString() != UtilMethods.DefaultForType(column.PropertyInfo.PropertyType).ObjToString();
                        if (isNotNull || isNotNullAndDefault)
                        {
                            if (cons.ConditionalList == null)
                            {
                                cons.ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>();
                                if (QueryBuilder.WhereInfos.IsNullOrEmpty() && whereModels.IsNullOrEmpty())
                                {

                                }
                                else
                                {
                                    WhereType = WhereType.Or;
                                }

                            }
                            var data = new KeyValuePair<WhereType, ConditionalModel>(WhereType, new ConditionalModel()
                            {
                                ConditionalType = ConditionalType.Equal,
                                FieldName = column.DbColumnName,
                                FieldValue = value.ObjToString(),
                                CSharpTypeName=column.UnderType.Name
                            });
                            if (value != null && value.GetType().IsEnum())
                            {
                                if (this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString == true)
                                {

                                }
                                else
                                {
                                    data.Value.FieldValue = Convert.ToInt64(value).ObjToString();
                                }

                            }
                            else if (value != null && column.PropertyInfo.PropertyType == UtilConstants.DateType) 
                            {
                                data.Value.FieldValue = value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.ffffff");
                            }
                            cons.ConditionalList.Add(data);
                            if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                            {
                                data.Value.FieldValueConvertFunc = it =>
                                {
                                    return UtilMethods.ChangeType2(it, value.GetType());
                                };
                            }
                        }
                    }
                    if (cons.HasValue())
                    {
                        whereModels.Add(cons);
                    }
                }
                this.Where(whereModels);
            }
            return this;
        }
        public virtual ISugarQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            this._Where(expression);
            return this;
        }
        public virtual ISugarQueryable<T> Where(string whereString, object whereObj = null)
        {
            if (whereString.HasValue())
                this.Where<T>(whereString, whereObj);
            return this;
        }
        public virtual ISugarQueryable<T> Where(IFuncModel funcModel) 
        {
            var obj= this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public virtual ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels)
        {
            if (conditionalModels.IsNullOrEmpty()) return this;
            var sqlObj = this.SqlBuilder.ConditionalModelToSql(conditionalModels,0);
            return this.Where(sqlObj.Key, sqlObj.Value);
        }
        public ISugarQueryable<T> Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            if (conditionalModels.IsNullOrEmpty()) return this;
            var sqlObj = this.SqlBuilder.ConditionalModelToSql(conditionalModels, 0);
            if (isWrap)
            {
                return this.Where("("+sqlObj.Key+")", sqlObj.Value);
            }
            else 
            {
                return this.Where(sqlObj.Key, sqlObj.Value);
            }
        }
        public virtual ISugarQueryable<T> Where<T2>(string whereString, object whereObj = null)
        {
            var whereValue = QueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString + UtilConstants.Space));
            if (whereObj != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            return this;
        }

        public virtual ISugarQueryable<T> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T> HavingIF(bool isHaving,Expression<Func<T, bool>> expression)
        {
            if(isHaving)
             this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T> Having(string whereString, object parameters = null)
        {

            QueryBuilder.HavingInfos = SqlBuilder.AppendHaving(whereString);
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }

        public virtual ISugarQueryable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (!isWhere) return this;
            _Where(expression);
            return this;
        }
        public virtual ISugarQueryable<T> WhereIF(bool isWhere, string whereString, object whereObj = null)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        public virtual T InSingle(object pkValue)
        {
            if (pkValue == null) 
            {
                pkValue = -1;
            }
            Check.Exception(this.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            var list = In(pkValue).ToList();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }
        public ISugarQueryable<T> InIF<TParamter>(bool isIn, params TParamter[] pkValues) 
        {
            if (isIn) 
            {
                In(pkValues);
            }
            return this;
        }
        public virtual ISugarQueryable<T> In<TParamter>(params TParamter[] pkValues)
        {
            if (pkValues == null || pkValues.Length == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            if (pkValues.Length == 1 && pkValues.First().GetType().FullName.IsCollectionsList() || (pkValues.First() is IEnumerable && pkValues.First().GetType() != UtilConstants.StringType))
            {
                var newValues = new List<object>();
                foreach (var item in pkValues.First() as IEnumerable)
                {
                    newValues.Add(item);
                }
                return In(newValues);
            }
            var pks = GetPrimaryKeys().Select(it => SqlBuilder.GetTranslationTableName(it)).ToList();
            Check.Exception(pks == null || pks.Count != 1, "Queryable.In(params object[] pkValues): Only one primary key");
            string filed = pks.FirstOrDefault();
            string shortName = QueryBuilder.TableShortName == null ? null : (QueryBuilder.TableShortName + ".");
            filed = shortName + filed;
            return In(filed, pkValues);
        }
        public virtual ISugarQueryable<T> In<FieldType>(string filed, params FieldType[] inValues)
        {
            if (inValues.Length == 1)
            {
                if (inValues.GetType().IsArray)
                {
                    var whereIndex = QueryBuilder.WhereIndex;
                    string parameterName = this.SqlBuilder.SqlParameterKeyWord + "InPara" + whereIndex;
                    this.AddParameters(new SugarParameter(parameterName, inValues[0]));
                    this.Where(string.Format(QueryBuilder.EqualTemplate, filed, parameterName));
                    QueryBuilder.WhereIndex++;
                }
                else
                {
                    var values = new List<object>();
                    foreach (var item in ((IEnumerable)inValues[0]))
                    {
                        if (item != null)
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                    this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));
                }
            }
            else
            {
                var values = new List<object>();
                foreach (var item in inValues)
                {
                    if (item != null)
                    {
                        if (UtilMethods.IsNumber(item.GetType().Name))
                        {
                            values.Add(item.ToString());
                        }
                        else
                        {
                            values.Add(item.ToString().ToSqlValue());
                        }
                    }
                }
                this.Where(string.Format(QueryBuilder.InTemplate, filed, string.Join(",", values)));

            }
            return this;
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            return In(fieldName, inValues);
        }
        public virtual ISugarQueryable<T> In<TParamter>(List<TParamter> pkValues)
        {
            if (pkValues == null || pkValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(pkValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(string InFieldName, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(InFieldName, inValues.ToArray());
        }
        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            if (inValues == null || inValues.Count == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In(expression, inValues.ToArray());
        }

        public virtual ISugarQueryable<T> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(string orderFileds)
        {
            orderFileds = orderFileds.ToCheckField();
            var orderByValue = QueryBuilder.OrderByValue;
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.OrderByTemplate;
            }
            QueryBuilder.OrderByValue += string.IsNullOrEmpty(orderByValue) ? orderFileds : ("," + orderFileds);
            return this;
        }
        public virtual ISugarQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            this._OrderBy(expression, type);
            return this;
        }
        public virtual ISugarQueryable<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public virtual ISugarQueryable<T> GroupByIF(bool isGroupBy,Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                return this.OrderBy(orderFileds);
            else
                return this;
        }
        public virtual ISugarQueryable<T> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                return this.OrderBy(expression, type);
            else
                return this;
        }

        public virtual ISugarQueryable<T> GroupBy(string groupFileds)
        {
            groupFileds = groupFileds.ToCheckField();
            var croupByValue = QueryBuilder.GroupByValue;
            if (QueryBuilder.GroupByValue.IsNullOrEmpty())
            {
                QueryBuilder.GroupByValue = QueryBuilder.GroupByTemplate;
            }
            QueryBuilder.GroupByValue += string.IsNullOrEmpty(croupByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> PartitionBy(Expression<Func<T, object>> expression)
        {
            if (QueryBuilder.Take == null)
                QueryBuilder.Take = 1;
            _PartitionBy(expression);
            QueryBuilder.DisableTop = true;
            return this;
        }
        public virtual ISugarQueryable<T> PartitionBy(string groupFileds)
        {
            var partitionByValue = QueryBuilder.PartitionByValue;
            if (QueryBuilder.PartitionByValue.IsNullOrEmpty())
            {
                QueryBuilder.PartitionByValue = QueryBuilder.PartitionByTemplate;
            }
            QueryBuilder.PartitionByValue += string.IsNullOrEmpty(partitionByValue) ? groupFileds : ("," + groupFileds);
            return this;
        }

        public virtual ISugarQueryable<T> Skip(int num)
        {
            QueryBuilder.Skip = num;
            return this;
        }
        public virtual ISugarQueryable<T> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }

        public virtual T Single()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            var oldSkip = QueryBuilder.Skip;
            var oldTake = QueryBuilder.Take;
            var oldOrderBy = QueryBuilder.OrderByValue;
            QueryBuilder.Skip = null;
            QueryBuilder.Take = null;
            QueryBuilder.OrderByValue = null;
            var result = this.ToList();
            QueryBuilder.Skip = oldSkip;
            QueryBuilder.Take = oldTake;
            QueryBuilder.OrderByValue = oldOrderBy;
            if (result == null || result.Count == 0)
            {
                return default(T);
            }
            else if (result.Count >= 2)
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage(".Single()  result must not exceed one . You can use.First()", "使用single查询结果集不能大于1，适合主键查询，如果大于1你可以使用Queryable.First"));
                return default(T);
            }
            else
            {
                return result.SingleOrDefault();
            }
        }
        public virtual T Single(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Single();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual T First()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            if (QueryBuilder.Skip.HasValue)
            {
                QueryBuilder.Take = 1;
                return this.ToList().FirstOrDefault();
            }
            else
            {
                QueryBuilder.Skip = 0;
                QueryBuilder.Take = 1;
                var result = this.ToList();
                if (result.HasValue())
                    return result.FirstOrDefault();
                else
                    return default(T);
            }
        }
        public virtual T First(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = First();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual bool Any(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public virtual bool Any()
        {
            return this.Select("1").ToList().Count() > 0;
        }

        public virtual List<TResult> ToList<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                return NavSelectHelper.GetList(expression,this);
               // var list = this.ToList().Select(expression.Compile()).ToList();
               // return list;
            }
            else 
            {
                var list = this.Select(expression).ToList();
                return list;
            }
        }
        public async virtual Task<List<TResult>> ToListAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes != null && this.QueryBuilder.Includes.Count > 0)
            {
                return await NavSelectHelper.GetListAsync(expression, this);
            }
            else
            {
                var list = await this.Select(expression).ToListAsync();
                return list;
            }
        }
        public virtual ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            Check.ExceptionEasy(this.QueryBuilder.Includes.HasValue(), $"use Includes(...).ToList(it=>new {typeof(TResult).Name} {{...}} )", $"Includes()后面禁使用Select，正确写法: ToList(it=>new {typeof(TResult).Name}{{....}})");
            return _Select<TResult>(expression);
        }

        public virtual ISugarQueryable<TResult> Select<TResult>()
        {
            var isJoin = this.QueryBuilder.JoinExpression != null;
            if (isJoin)
            {
                var selectValue = new SugarMapper(this.Context).GetSelectValue<TResult>(this.QueryBuilder);
                return this.Select<TResult>(selectValue);
            }
            else if (this.QueryBuilder.EntityType == UtilConstants.ObjType || (this.QueryBuilder.AsTables != null && this.QueryBuilder.AsTables.Count == 1)||this.QueryBuilder.EntityName!=this.QueryBuilder.EntityType.Name) 
            {
                if (this.QueryBuilder.SelectValue.HasValue()&& this.QueryBuilder.SelectValue.ObjToString().Contains("AS"))
                {
                    return this.Select<TResult>(this.QueryBuilder.SelectValue+"");
                }
                else
                {
                    return this.Select<TResult>(this.SqlBuilder.SqlSelectAll);
                }
            }
            else
            {
                var selects = this.QueryBuilder.GetSelectValueByString();
                if (selects.ObjToString().ToLower().IsContainsIn(".","("," as ")) 
                {
                    return this.Select<TResult>(selects);
                }
                var resultColumns=this.Context.EntityMaintenance.GetEntityInfo<TResult>().Columns;
                var dbColumns = this.EntityInfo.Columns.Where(it=>!it.IsIgnore);
                StringBuilder sb = new StringBuilder();
                foreach (var item in resultColumns)
                {
                    var firstColumn= dbColumns.FirstOrDefault(z =>
                    z.PropertyName.EqualCase(item.PropertyName) ||
                    z.DbColumnName.EqualCase(item.PropertyName));
                    if (firstColumn != null)
                    {
                        var dbColumnName = firstColumn.DbColumnName;
                        var AsName = item.PropertyName;
                        sb.Append($"{this.SqlBuilder.GetTranslationColumnName(dbColumnName)} AS {this.SqlBuilder.GetTranslationColumnName(AsName)} ,");
                    }
                }
                selects = sb.ToString().TrimEnd(',');
                if (selects == "")
                {
                    selects = "*";
                }
                return this.Select<TResult>(selects);
            }
        }

        public virtual ISugarQueryable<TResult> Select<TResult>(string selectValue)
        {
            var result = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = this.SqlBuilder;
            result.QueryBuilder.ResultType = this.QueryBuilder.ResultType;
            result.IsCache = this.IsCache;
            result.CacheTime = this.CacheTime;
            QueryBuilder.SelectValue = selectValue;
            if (this.IsAs)
            {
                ((QueryableProvider<TResult>)result).IsAs = true;
            }
            return result;
        }
        public virtual ISugarQueryable<T> Select(string selectValue)
        {
            QueryBuilder.SelectValue = selectValue;
            return this;
        }
        public virtual ISugarQueryable<T> MergeTable()
        {
            Check.Exception(this.MapperAction != null || this.MapperActionWithCache != null,ErrorMessage.GetThrowMessage( "'Mapper’ needs to be written after ‘MergeTable’ ", "Mapper 只能在 MergeTable 之后使用"));
            Check.Exception(this.QueryBuilder.SelectValue.IsNullOrEmpty(),ErrorMessage.GetThrowMessage( "MergeTable need to use Queryable.Select Method .", "使用MergeTable之前必须要有Queryable.Select方法"));
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0 || this.QueryBuilder.OrderByValue.HasValue(),ErrorMessage.GetThrowMessage( "MergeTable  Queryable cannot Take Skip OrderBy PageToList  ", "使用 MergeTable不能有 Take Skip OrderBy PageToList 等操作,你可以在Mergetable之后操作"));
            var sqlobj = this._ToSql();
            var index = QueryBuilder.WhereIndex + 1;
            var result = this.Context.Queryable<T>().AS(SqlBuilder.GetPackTable(sqlobj.Key, "MergeTable")).AddParameters(sqlobj.Value).Select("*").With(SqlWith.Null);
            result.QueryBuilder.WhereIndex = index;
            result.QueryBuilder.LambdaExpressions.ParameterIndex = QueryBuilder.LambdaExpressions.ParameterIndex++;
            result.QueryBuilder.LambdaExpressions.Index = QueryBuilder.LambdaExpressions.Index++;
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle) 
            {
                result.Select("MergeTable.*");
            }
            return result;
        }
        public ISugarQueryable<T> SplitTable(DateTime beginTime, DateTime endTime) 
        {
            var splitColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            var columnName = this.SqlBuilder.GetTranslationColumnName(splitColumn.DbColumnName);
            var sqlParameterKeyWord = this.SqlBuilder.SqlParameterKeyWord;
            return this.Where($" {columnName}>={sqlParameterKeyWord}spBeginTime AND {columnName}<= {sqlParameterKeyWord}spEndTime",new { spBeginTime = beginTime , spEndTime = endTime}).SplitTable(tas => {
                var result = tas;
                var  type= this.EntityInfo.Type.GetCustomAttribute<SplitTableAttribute>();
                Check.ExceptionEasy(type == null, $"{this.EntityInfo.EntityName}need SplitTableAttribute", $"{this.EntityInfo.EntityName}需要特性 SplitTableAttribute");
                if (SplitType.Month == type.SplitType)
                {
                    result = result.Where(y => y.Date >= beginTime.ToString("yyyy-MM").ObjToDate() && y.Date <= endTime.Date.ToString("yyyy-MM").ObjToDate().AddMonths(1).AddDays(-1)).ToList();
                }
                else if (SplitType.Year == type.SplitType)
                {
                    result = result.Where(y => y.Date.Year >= beginTime.Year && y.Date.Year <= endTime.Year).ToList();
                }
                else if (SplitType.Week == type.SplitType)
                {
                    var begtinWeek = UtilMethods.GetWeekFirstDayMon(beginTime).Date;
                    var endWeek = UtilMethods.GetWeekLastDaySun(endTime).Date;
                    result = result.Where(y => 
                        y.Date >= begtinWeek&& y.Date  <= endWeek).ToList();
                }
                else if (SplitType.Season == type.SplitType)
                {

                    var beginSeason= Convert.ToDateTime( beginTime.AddMonths(0 - ((beginTime.Month - 1) % 3)).ToString("yyyy-MM-01")).Date;
                    var endSeason= DateTime.Parse(endTime.AddMonths(3 - ((endTime.Month - 1) % 3)).ToString("yyyy-MM-01")).AddDays(-1).Date;
                    result = result.Where(y =>
                        y.Date >= beginSeason && y.Date <= endSeason).ToList();
                }
                else 
                {
                    result = result.Where(y => y.Date >= beginTime.Date && y.Date <= endTime.Date).ToList();
                }
                return result;
                });
        }
        public ISugarQueryable<T> SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc) 
        {
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.EntityInfo
            };
            this.Context.MappingTables.Add(this.EntityInfo.EntityName, this.EntityInfo.DbTableName);
            var tables = getTableNamesFunc(helper.GetTables());
            List<ISugarQueryable<T>> tableQueryables = new List<ISugarQueryable<T>>();
            foreach (var item in tables)
            {
                tableQueryables.Add(this.Clone().AS(item.TableName));
            }
            if (tableQueryables.Count == 0)
            {
                var result= this.Context.SqlQueryable<object>("-- No table ").Select<T>();
                result.QueryBuilder.SelectValue = null;
                return result;
            }
            else
            {
                var unionall = this.Context._UnionAll(tableQueryables.ToArray());
                return unionall;
            }
            //var values= unionall.QueryBuilder.GetSelectValue;
            //unionall.QueryBuilder.SelectValue = values;
        }
        public ISugarQueryable<T> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        public virtual int Count()
        {
            if (this.QueryBuilder.Skip == null&& 
                this.QueryBuilder.Take == null&& 
                this.QueryBuilder.OrderByValue == null && 
                this.QueryBuilder.PartitionByValue == null&&
                this.QueryBuilder.SelectValue==null&&
                this.QueryBuilder.Includes == null&&
                this.QueryBuilder.IsDistinct==false) 
            {

                return this.Clone().Select<int>(" COUNT(1) ").ToList().FirstOrDefault();
            }
            MappingTableList expMapping;
            int result;
            _CountBegin(out expMapping, out result);
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<int>(cacheService, this.QueryBuilder, () => { return GetCount(); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result = GetCount();
            }
            _CountEnd(expMapping);
            return result;
        }

        public virtual int Count(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = Count();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public virtual TResult Max<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }

        public virtual TResult Min<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }

        public virtual TResult Sum<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }

        public virtual TResult Avg<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var result = this._ToList<TResult>().SingleOrDefault();
            return result;
        }
        public virtual TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public virtual T[] ToArray()
        {

            var result = this.ToList();
            if (result.HasValue())
                return result.ToArray();
            else
                return null;
        }
        public async virtual Task<T[]> ToArrayAsync()
        {

            var result =await this.ToListAsync();
            if (result.HasValue())
                return result.ToArray();
            else
                return null;
        }
        public virtual string ToJson()
        {
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                var result = CacheSchemeMain.GetOrCreate<string>(cacheService, this.QueryBuilder, () =>
                {
                    return this.Context.Utilities.SerializeObject(this.ToList(), typeof(T));
                }, CacheTime, this.Context,CacheKey);
                return result;
            }
            else
            {
                return this.Context.Utilities.SerializeObject(this.ToList(), typeof(T));
            }
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize), typeof(T));
        }
        public virtual string ToJsonPage(int pageIndex, int pageSize, ref int totalNumber)
        {
            return this.Context.Utilities.SerializeObject(this.ToPageList(pageIndex, pageSize, ref totalNumber), typeof(T));
        }
        public virtual DataTable ToPivotTable<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector) 
        {
            return this.ToList().ToPivotTable(columnSelector,rowSelector,dataSelector);
        }
        public virtual List<dynamic> ToPivotList<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            return this.ToList().ToPivotList(columnSelector, rowSelector, dataSelector);
        }
        public virtual string ToPivotJson<TColumn, TRow, TData>(Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            var list= this.ToPivotList(columnSelector, rowSelector, dataSelector);
            return this.Context.Utilities.SerializeObject(list);
        }
        public List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue,bool isContainOneself = true) 
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            if (isContainOneself)
            {
                var result= GetChildList(parentIdExpression, pk, list, primaryKeyValue);
                var addItem = this.Context.Queryable<T>().In(pk, primaryKeyValue).First();
                if (addItem != null)
                {
                    result.Add(addItem);
                }
                return result;
            }
            else
            {
                return GetChildList(parentIdExpression, pk, list, primaryKeyValue);
            }
        }
        public async Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue,bool isContainOneself=true) 
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = await this.ToListAsync();
            if (isContainOneself)
            {
                var result = GetChildList(parentIdExpression, pk, list, primaryKeyValue);
                var addItem = this.Context.Queryable<T>().In(pk, primaryKeyValue).First();
                if (addItem != null)
                {
                    result.Add(addItem);
                }
                return result;
            }
            else
            {
                return GetChildList(parentIdExpression, pk, list, primaryKeyValue);
            }
        }
        public List<T> ToParentList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey) 
            {
                return _ToParentListByTreeKey(parentIdExpression,primaryKeyValue);
            }
            List<T> result = new List<T>() { };
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey).Count() == 0, "No Primary key");
            var parentIdName =UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName= ParentInfo.DbColumnName;
            var tableName= this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false) 
            {
                if (this.QueryBuilder.JoinQueryInfos.Count>0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count>0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current,null);
                int i = 0;
                while (parentId!=null&&this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).Any())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingle(parentId);
                    result.Add(parent);
                    parentId= ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }

        private List<T> _ToParentListByTreeKey(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var treeKey = entity.Columns.FirstOrDefault(it => it.IsTreeKey);
            List<T> result = new List<T>() { };
            var parentIdName = UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName = ParentInfo.DbColumnName;
            var tableName = this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false)
            {
                if (this.QueryBuilder.JoinQueryInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = primaryKeyValue + "",
                    FieldName = treeKey.DbColumnName
                } }).First();
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = parentId + "",
                    FieldName = treeKey.DbColumnName
                } }).Any())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = parentId + "",
                    FieldName = treeKey.DbColumnName
                } }).First();
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }

        public async Task<List<T>> ToParentListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            List<T> result = new List<T>() { };
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var isTreeKey = entity.Columns.Any(it => it.IsTreeKey);
            if (isTreeKey)
            {
                return await _ToParentListByTreeKeyAsync(parentIdExpression, primaryKeyValue);
            }
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey).Count() == 0, "No Primary key");
            var parentIdName = UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName = ParentInfo.DbColumnName;
            var tableName = this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false)
            {
                if (this.QueryBuilder.JoinQueryInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current =await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(primaryKeyValue);
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null &&await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).In(parentId).AnyAsync())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent =await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).InSingleAsync(parentId);
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }
        private async Task<List<T>> _ToParentListByTreeKeyAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var treeKey = entity.Columns.FirstOrDefault(it => it.IsTreeKey);
            List<T> result = new List<T>() { };
            var parentIdName = UtilConvert.ToMemberExpression((parentIdExpression as LambdaExpression).Body).Member.Name;
            var ParentInfo = entity.Columns.First(it => it.PropertyName == parentIdName);
            var parentPropertyName = ParentInfo.DbColumnName;
            var tableName = this.QueryBuilder.GetTableNameString;
            if (this.QueryBuilder.IsSingle() == false)
            {
                if (this.QueryBuilder.JoinQueryInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
                if (this.QueryBuilder.EasyJoinInfos.Count > 0)
                {
                    tableName = this.QueryBuilder.JoinQueryInfos.First().TableName;
                }
            }
            var current = await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = primaryKeyValue + "",
                    FieldName = treeKey.DbColumnName
                } }).FirstAsync();
            if (current != null)
            {
                result.Add(current);
                object parentId = ParentInfo.PropertyInfo.GetValue(current, null);
                int i = 0;
                while (parentId != null && await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = parentId + "",
                    FieldName = treeKey.DbColumnName
                } }).AnyAsync())
                {
                    Check.Exception(i > 100, ErrorMessage.GetThrowMessage("Dead cycle", "出现死循环或超出循环上限（100），检查最顶层的ParentId是否是null或者0"));
                    var parent = await this.Context.Queryable<T>().AS(tableName).Filter(null, this.QueryBuilder.IsDisabledGobalFilter).Where(new List<IConditionalModel>() {
                new ConditionalModel()
                {
                    ConditionalType = ConditionalType.Equal,
                    CSharpTypeName = treeKey.PropertyInfo.PropertyType.Name,
                    FieldValue = parentId + "",
                    FieldName = treeKey.DbColumnName
                } }).FirstAsync();
                    result.Add(parent);
                    parentId = ParentInfo.PropertyInfo.GetValue(parent, null);
                    ++i;
                }
            }
            return result;
        }

        public List<T> ToTree(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue);
        }

        public async Task<List<T>> ToTreeAsync(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity); ;
            var list =await this.ToListAsync();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list,rootValue);
        }

        public virtual DataTable ToDataTable()
        {
            QueryBuilder.ResultType = typeof(SugarCacheDataTable);
            InitMapping();
            var sqlObj = this.ToSql();
            RestoreMapping();
            DataTable result = null;
            bool isChangeQueryableMasterSlave = GetIsMasterQuery();
            bool isChangeQueryableSlave = GetIsSlaveQuery();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<DataTable>(cacheService, this.QueryBuilder, () => { return this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray()); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result = this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray());
            }
            RestChangeMasterQuery(isChangeQueryableMasterSlave);
            RestChangeSlaveQuery(isChangeQueryableSlave);
            return result;
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            if (QueryBuilder.PartitionByValue.HasValue())
            {
                QueryBuilder.ExternalPageIndex = pageIndex;
                QueryBuilder.ExternalPageSize = pageSize;
            }
            else
            {
                QueryBuilder.Skip = (pageIndex - 1) * pageSize;
                QueryBuilder.Take = pageSize;
            }
            return ToDataTable();
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber)
        {
            _RestoreMapping = false;
            totalNumber = this.Clone().Count();
            _RestoreMapping = true;
            var result = this.Clone().ToDataTablePage(pageIndex, pageSize);
            return result;
        }
        public virtual DataTable ToDataTablePage(int pageIndex, int pageSize, ref int totalNumber, ref int totalPage)
        {
            var result = ToDataTablePage(pageIndex, pageSize, ref totalNumber);
            totalPage = (totalNumber + pageSize - 1) / pageSize;
            return result;
        }

        public Dictionary<string, object> ToDictionary(Expression<Func<T, object>> key, Expression<Func<T, object>> value)
        {
            this.QueryBuilder.ResultType = typeof(SugarCacheDictionary);
            var keyName = QueryBuilder.GetExpressionValue(key, ResolveExpressType.FieldSingle).GetResultString();
            var valueName = QueryBuilder.GetExpressionValue(value, ResolveExpressType.FieldSingle).GetResultString();
            if (this.QueryBuilder.IsSingle() == false) 
            {
                keyName = this.QueryBuilder.TableShortName+ "." + keyName;
                valueName = this.QueryBuilder.TableShortName + "." + valueName;
            }
            var result = this.Select<KeyValuePair<string, object>>(keyName + "," + valueName).ToList().ToDictionary(it => it.Key.ObjToString(), it => it.Value);
            return result;
        }
        public async Task<Dictionary<string, object>> ToDictionaryAsync(Expression<Func<T, object>> key, Expression<Func<T, object>> value)
        {
            this.QueryBuilder.ResultType = typeof(SugarCacheDictionary);
            var keyName = QueryBuilder.GetExpressionValue(key, ResolveExpressType.FieldSingle).GetResultString();
            var valueName = QueryBuilder.GetExpressionValue(value, ResolveExpressType.FieldSingle).GetResultString();
            var list = await this.Select<KeyValuePair<string, object>>(keyName + "," + valueName).ToListAsync();
            var result =list.ToDictionary(it => it.Key.ObjToString(), it => it.Value);
            return result;
        }
        public List<Dictionary<string, object>> ToDictionaryList()
        {
            var list = this.ToList();
            if (list == null)
                return null;
            else
                return this.Context.Utilities.DeserializeObject<List<Dictionary<string, object>>>(this.Context.Utilities.SerializeObject(list));
        }
        public async Task<List<Dictionary<string, object>>> ToDictionaryListAsync()
        {
            var list =await this.ToListAsync();
            if (list == null)
                return null;
            else
                return this.Context.Utilities.DeserializeObject<List<Dictionary<string, object>>>(this.Context.Utilities.SerializeObject(list));
        }

        public virtual List<T> ToList()
        {
            InitMapping();
            return _ToList<T>();
        }
        public List<T> SetContext<ParameterT>(Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter) 
        {
            if (parameter == null) 
            {
                return new List<T>();
            }
            List<T> result = new List<T>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var pkName = "";
            if ((mappingFiled as LambdaExpression).Body is UnaryExpression)
            {
                pkName = (((mappingFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            } 
            else  
            {
                pkName = ((mappingFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var key = thisFiled.ToString()+mappingFiled.ToString() +typeof(ParameterT).FullName + typeof(T).FullName;
            var ids = list.Where(it=>it!=null).Select(it => it.GetType().GetProperty(pkName).GetValue(it)).Distinct().ToArray();
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null &&  queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                if (queryableContext.TempChildLists == null)
                    queryableContext.TempChildLists = new Dictionary<string, object>();
                this.Context.Utilities.PageEach(ids, 200, pageIds =>
                {
                     result.AddRange(this.Clone().In(thisFiled, pageIds).ToList());
                });
                queryableContext.TempChildLists[key]= result;
            }
            var name = "";
            if((thisFiled as LambdaExpression).Body is UnaryExpression) 
            {
                name = (((thisFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                name = ((thisFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var pkValue = parameter.GetType().GetProperty(pkName).GetValue(parameter);
            result = result.Where(it => it.GetType().GetProperty(name).GetValue(it).ObjToString() == pkValue.ObjToString()).ToList();
            return result;
        }
        public List<T> SetContext<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1,
            Expression<Func<T, object>> thisFiled2, Expression<Func<object>> mappingFiled2,
            ParameterT parameter)
        {
            if (parameter == null)
            {
                return new List<T>();
            }
            var rightEntity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var leftEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            List<T> result = new List<T>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var key = thisFiled1.ToString() + mappingFiled1.ToString()+
                      thisFiled2.ToString() + mappingFiled2.ToString()+
                       typeof(ParameterT).FullName + typeof(T).FullName;
            MappingFieldsHelper<ParameterT> fieldsHelper = new MappingFieldsHelper<ParameterT>();
            var mappings = new List<MappingFieldsExpression>() {
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled1,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled1)),
               RightColumnExpression=mappingFiled1,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled1))
             },
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled2,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled2)),
               RightColumnExpression=mappingFiled2,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled2))
             }
            };
            var conditionals=fieldsHelper.GetMppingSql(list.Cast<object>().ToList(), mappings);
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                result = this.Clone().Where(conditionals,true).ToList();
                queryableContext.TempChildLists[key] = result;
            }
            List<object> listObj = result.Select(it => (object)it).ToList();
            object obj = (object)parameter;
            var newResult = fieldsHelper.GetSetList(obj, listObj, mappings).Select(it=>(T)it ).ToList();
            return newResult;
        }
        public async Task<List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled, Expression<Func<object>> mappingFiled, ParameterT parameter)
        {
            List<T> result = new List<T>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var pkName = "";
            if ((mappingFiled as LambdaExpression).Body is UnaryExpression)
            {
                pkName = (((mappingFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                pkName = ((mappingFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var key = thisFiled.ToString()+ mappingFiled.ToString() + typeof(ParameterT).FullName + typeof(T).FullName;
            var ids = list.Select(it => it.GetType().GetProperty(pkName).GetValue(it)).ToArray();
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                if (queryableContext.TempChildLists == null)
                    queryableContext.TempChildLists = new Dictionary<string, object>();
                await this.Context.Utilities.PageEachAsync(ids, 200, async pageIds =>
                {
                    result.AddRange(await this.Clone().In(thisFiled, pageIds).ToListAsync());
                });
                queryableContext.TempChildLists[key] = result;
            }
            var name = "";
            if ((thisFiled as LambdaExpression).Body is UnaryExpression)
            {
                name = (((thisFiled as LambdaExpression).Body as UnaryExpression).Operand as MemberExpression).Member.Name;
            }
            else
            {
                name = ((thisFiled as LambdaExpression).Body as MemberExpression).Member.Name;
            }
            var pkValue = parameter.GetType().GetProperty(pkName).GetValue(parameter);
            result = result.Where(it => it.GetType().GetProperty(name).GetValue(it).ObjToString() == pkValue.ObjToString()).ToList();
            return result;
        }
        public async Task<List<T>> SetContextAsync<ParameterT>(Expression<Func<T, object>> thisFiled1, Expression<Func<object>> mappingFiled1,
    Expression<Func<T, object>> thisFiled2, Expression<Func<object>> mappingFiled2,
    ParameterT parameter)
        {
            if (parameter == null)
            {
                return new List<T>();
            }
            var rightEntity = this.Context.EntityMaintenance.GetEntityInfo<ParameterT>();
            var leftEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            List<T> result = new List<T>();
            var queryableContext = this.Context.TempItems["Queryable_To_Context"] as MapperContext<ParameterT>;
            var list = queryableContext.list;
            var key = thisFiled1.ToString() + mappingFiled1.ToString() +
                      thisFiled2.ToString() + mappingFiled2.ToString() +
                       typeof(ParameterT).FullName + typeof(T).FullName;
            MappingFieldsHelper<ParameterT> fieldsHelper = new MappingFieldsHelper<ParameterT>();
            var mappings = new List<MappingFieldsExpression>() {
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled1,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled1)),
               RightColumnExpression=mappingFiled1,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled1))
             },
               new MappingFieldsExpression(){
               LeftColumnExpression=thisFiled2,
               LeftEntityColumn=leftEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(thisFiled2)),
               RightColumnExpression=mappingFiled2,
               RightEntityColumn=rightEntity.Columns.First(it=>it.PropertyName==ExpressionTool.GetMemberName(mappingFiled2))
             }
            };
            var conditionals = fieldsHelper.GetMppingSql(list.Cast<object>().ToList(), mappings);
            if (queryableContext.TempChildLists == null)
                queryableContext.TempChildLists = new Dictionary<string, object>();
            if (list != null && queryableContext.TempChildLists.ContainsKey(key))
            {
                result = (List<T>)queryableContext.TempChildLists[key];
            }
            else
            {
                result =await this.Clone().Where(conditionals, true).ToListAsync();
                queryableContext.TempChildLists[key] = result;
            }
            List<object> listObj = result.Select(it => (object)it).ToList();
            object obj = (object)parameter;
            var newResult = fieldsHelper.GetSetList(obj, listObj, mappings).Select(it => (T)it).ToList();
            return newResult;
        }
        public virtual void ForEach(Action<T> action, int singleMaxReads = 300,System.Threading.CancellationTokenSource cancellationTokenSource = null) 
        {
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0, ErrorMessage.GetThrowMessage("no support Skip take, use PageForEach", "不支持Skip Take,请使用 Queryale.PageForEach"));
            var totalNumber = 0;
            var totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true) return;
                var queryable = this.Clone();
                var page =
                    totalPage==1?
                    queryable.ToPageList(i, singleMaxReads, ref totalNumber, ref totalPage):
                    queryable.ToPageList(i, singleMaxReads);
                foreach (var item in page)
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    action.Invoke(item);
                }
            }
        }
        public virtual async Task ForEachAsync(Action<T> action, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            Check.Exception(this.QueryBuilder.Skip > 0 || this.QueryBuilder.Take > 0, ErrorMessage.GetThrowMessage("no support Skip take, use PageForEach", "不支持Skip Take,请使用 Queryale.PageForEach"));
            RefAsync<int> totalNumber = 0;
            RefAsync<int> totalPage = 1;
            for (int i = 1; i <= totalPage; i++)
            {
                if (cancellationTokenSource?.IsCancellationRequested == true) return;
                var queryable = this.Clone();
                var page =
                    totalPage == 1 ?
                    await  queryable.ToPageListAsync(i, singleMaxReads,  totalNumber,  totalPage) :
                    await queryable.ToPageListAsync(i, singleMaxReads);
                foreach (var item in page)
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    action.Invoke(item);
                }
            }
        }
        public virtual void ForEachByPage(Action<T> action, int pageIndex, int pageSize, ref int totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            int count = this.Clone().Count();
            if (count > 0)
            {
                if (pageSize > singleMaxReads && count - ((pageIndex - 1) * pageSize) > singleMaxReads)
                {
                    Int32 Skip = (pageIndex - 1) * pageSize;
                    Int32 NowCount = count - Skip;
                    Int32 number = 0;
                    if (NowCount > pageSize) NowCount = pageSize;
                    while (NowCount > 0)
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        if (number + singleMaxReads > pageSize) singleMaxReads = NowCount;
                        foreach (var item in this.Clone().Skip(Skip).Take(singleMaxReads).ToList())
                        {
                            if (cancellationTokenSource?.IsCancellationRequested == true) return;
                            action.Invoke(item);
                        }
                        NowCount -= singleMaxReads;
                        Skip += singleMaxReads;
                        number += singleMaxReads;
                    }
                }
                else
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    foreach (var item in this.Clone().ToPageList(pageIndex, pageSize))
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        action.Invoke(item);
                    }
                }
            }
            totalNumber = count;
        }
        public virtual async Task ForEachByPageAsync(Action<T> action, int pageIndex, int pageSize,RefAsync<int> totalNumber, int singleMaxReads = 300, System.Threading.CancellationTokenSource cancellationTokenSource = null)
        {
            int count = this.Clone().Count();
            if (count > 0)
            {
                if (pageSize > singleMaxReads && count - ((pageIndex - 1) * pageSize) > singleMaxReads)
                {
                    Int32 Skip = (pageIndex - 1) * pageSize;
                    Int32 NowCount = count - Skip;
                    Int32 number = 0;
                    if (NowCount > pageSize) NowCount = pageSize;
                    while (NowCount > 0)
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        if (number + singleMaxReads > pageSize) singleMaxReads = NowCount;
                        foreach (var item in await this.Clone().Skip(Skip).Take(singleMaxReads).ToListAsync())
                        {
                            if (cancellationTokenSource?.IsCancellationRequested == true) return;
                            action.Invoke(item);
                        }
                        NowCount -= singleMaxReads;
                        Skip += singleMaxReads;
                        number += singleMaxReads;
                    }
                }
                else
                {
                    if (cancellationTokenSource?.IsCancellationRequested == true) return;
                    foreach (var item in this.Clone().ToPageList(pageIndex, pageSize))
                    {
                        if (cancellationTokenSource?.IsCancellationRequested == true) return;
                        action.Invoke(item);
                    }
                }
            }
            totalNumber = count;
        }

        public List<T> ToOffsetPage(int pageIndex, int pageSize) 
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                return this.ToPageList(pageIndex, pageSize);
            }
            else
            {
                _ToOffsetPage(pageIndex, pageSize);
                return this.ToList();
            }
        }
        public List<T> ToOffsetPage(int pageIndex, int pageSize, ref int totalNumber) 
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                return this.ToPageList(pageIndex, pageSize, ref totalNumber);
            }
            else 
            {
                totalNumber = this.Clone().Count();
               _ToOffsetPage(pageIndex, pageSize);
                return this.Clone().ToList();
            }
        }
        public Task<List<T>> ToOffsetPageAsync(int pageIndex, int pageSize) 
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                return this.ToPageListAsync(pageIndex, pageSize);
            }
            else
            {
                _ToOffsetPage(pageIndex, pageSize);
                return this.ToListAsync();
            }
        }
        public async Task<List<T>> ToOffsetPageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber) 
        {
            if (this.Context.CurrentConnectionConfig.DbType != DbType.SqlServer)
            {
                return await this.ToPageListAsync(pageIndex, pageSize, totalNumber);
            }
            else 
            {
                totalNumber.Value =await this.Clone().CountAsync();
                _ToOffsetPage(pageIndex, pageSize);
                return await this.Clone().ToListAsync();
            }
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToList();
        }
        public virtual List<TResult> ToPageList<TResult>(int pageIndex, int pageSize, ref int totalNumber,Expression<Func<T, TResult>> expression) 
        {
            if (this.QueryBuilder.Includes!=null&&this.QueryBuilder.Includes.Count > 0)
            {
                if (pageIndex == 0)
                    pageIndex = 1;
                var list = this.Clone().Skip((pageIndex-1)*pageSize).Take(pageSize).ToList(expression);
                var countQueryable = this.Clone();
                countQueryable.QueryBuilder.Includes = null;
                totalNumber = countQueryable.Count();
                return list;
            }
            else
            {
                var list = this.Select(expression).ToPageList(pageIndex, pageSize, ref totalNumber).ToList();
                return list;
            }
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber =  this.Clone().Count();
            this.Context.MappingTables = oldMapping;
            return  this.Clone().ToPageList(pageIndex, pageSize);
        }
        public virtual List<T> ToPageList(int pageIndex, int pageSize, ref int totalNumber, ref int totalPage)
        {
            var result = ToPageList(pageIndex, pageSize, ref totalNumber);
            totalPage = (totalNumber + pageSize - 1) / pageSize;
            return result;
        }
        public virtual string ToSqlString()
        {
            var sqlObj = this.Clone().ToSql();
            var result = sqlObj.Key;
            if (result == null) return null;
            result = UtilMethods.GetSqlString(this.Context.CurrentConnectionConfig,sqlObj);
            return result;
        }


        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            if (!QueryBuilder.IsClone) 
            {
                var newQueryable = this.Clone();
                newQueryable.QueryBuilder.IsClone = true;
                return newQueryable.ToSql();
            }
            else
            {
                return _ToSql();
            }
        }
        public ISugarQueryable<T> WithCache(string cacheKey, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            Check.ArgumentNullException(this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService, "Use Cache ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is required ");
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            this.CacheKey = cacheKey;
            return this;
        }
        public ISugarQueryable<T> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            Check.ArgumentNullException(this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService, "Use Cache ConnectionConfig.ConfigureExternalServices.DataInfoCacheService is required ");
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }

        public ISugarQueryable<T> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (isCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        public string ToClassString(string className)
        {
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            var properties = typeof(T).GetProperties();
            foreach (var item in properties)
            {
                columns.Add(new DbColumnInfo()
                {
                    DbColumnName = item.Name,
                    PropertyName = UtilMethods.GetUnderType(item.PropertyType).Name,
                    PropertyType = UtilMethods.GetUnderType(item.PropertyType)
                });
            }
            var result = ((this.Context.DbFirst) as DbFirstProvider).GetClassString(columns, ref className);
            return result;
        }
        #region Async methods
        public virtual async Task<T> InSingleAsync(object pkValue)
        {
            Check.Exception(this.QueryBuilder.SelectValue.HasValue(), "'InSingle' and' Select' can't be used together,You can use .Select(it=>...).Single(it.id==1)");
            var list =await In(pkValue).ToListAsync();
            if (list == null) return default(T);
            else return list.SingleOrDefault();
        }
        public async Task<T> SingleAsync()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            var oldSkip = QueryBuilder.Skip;
            var oldTake = QueryBuilder.Take;
            var oldOrderBy = QueryBuilder.OrderByValue;
            QueryBuilder.Skip = null;
            QueryBuilder.Take = null;
            QueryBuilder.OrderByValue = null;
            var result =await this.ToListAsync();
            QueryBuilder.Skip = oldSkip;
            QueryBuilder.Take = oldTake;
            QueryBuilder.OrderByValue = oldOrderBy;
            if (result == null || result.Count == 0)
            {
                return default(T);
            }
            else if (result.Count == 2)
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage(".Single()  result must not exceed one . You can use.First()", "使用single查询结果集不能大于1，适合主键查询，如果大于1你可以使用Queryable.First"));
                return default(T);
            }
            else
            {
                return result.SingleOrDefault();
            }
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result =await SingleAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public async Task<T> FirstAsync()
        {
            if (QueryBuilder.OrderByValue.IsNullOrEmpty())
            {
                QueryBuilder.OrderByValue = QueryBuilder.DefaultOrderByTemplate;
            }
            if (QueryBuilder.Skip.HasValue)
            {
                QueryBuilder.Take = 1;
                var list = await this.ToListAsync();
                return list.FirstOrDefault();
            }
            else
            {
                QueryBuilder.Skip = 0;
                QueryBuilder.Take = 1;
                var result =await this.ToListAsync();
                if (result.HasValue())
                    return result.FirstOrDefault();
                else
                    return default(T);
            }
        }

        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result = await FirstAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result =await AnyAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }

        public async Task<bool> AnyAsync()
        {
            return await this.CountAsync() > 0;
        }

        public async Task<int> CountAsync()
        {
            if (this.QueryBuilder.Skip == null &&
             this.QueryBuilder.Take == null &&
             this.QueryBuilder.OrderByValue == null &&
             this.QueryBuilder.PartitionByValue == null &&
             this.QueryBuilder.SelectValue == null &&
             this.QueryBuilder.Includes == null&&
             this.QueryBuilder.IsDistinct==false)
            {
                var list = await this.Clone().Select<int>(" COUNT(1) ").ToListAsync();
                return list.FirstOrDefault();
            }
            MappingTableList expMapping;
            int result;
            _CountBegin(out expMapping, out result);
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<int>(cacheService, this.QueryBuilder, () => { return GetCount(); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result =await GetCountAsync();
            }
            _CountEnd(expMapping);
            return result;
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            var result =await CountAsync();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public  async Task<TResult> MaxAsync<TResult>(string maxField)
        {
            this.Select(string.Format(QueryBuilder.MaxTemplate, maxField));
            var list = await this._ToListAsync<TResult>();
            var result =list.SingleOrDefault();
            return result;
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }

        public async Task<TResult> MinAsync<TResult>(string minField)
        {
            this.Select(string.Format(QueryBuilder.MinTemplate, minField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }

        public async Task<TResult> SumAsync<TResult>(string sumField)
        {
            this.Select(string.Format(QueryBuilder.SumTemplate, sumField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }

        public Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }

        public async Task<TResult> AvgAsync<TResult>(string avgField)
        {
            this.Select(string.Format(QueryBuilder.AvgTemplate, avgField));
            var list = await this._ToListAsync<TResult>();
            var result = list.SingleOrDefault();
            return result;
        }

        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }

        public Task<List<T>> ToListAsync()
        {
            InitMapping();
            return _ToListAsync<T>();
        }
        public Task<List<T>> ToPageListAsync(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToListAsync();
        }
        public async virtual Task<List<TResult>> ToPageListAsync<TResult>(int pageIndex, int pageSize, RefAsync<int> totalNumber, Expression<Func<T, TResult>> expression)
        {
            if (this.QueryBuilder.Includes!=null&&this.QueryBuilder.Includes.Count > 0)
            {
                if (pageIndex == 0)
                    pageIndex = 1;
                var list =await this.Clone().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(expression);
                var countQueryable = this.Clone();
                countQueryable.QueryBuilder.Includes = null;
                totalNumber.Value =await countQueryable.CountAsync();
                return list;
            }
            else
            {
                var list = await this.Select(expression).ToPageListAsync(pageIndex, pageSize, totalNumber) ;
                return list;
            }
        }
        public async Task<List<T>> ToPageListAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber.Value = await this.Clone().CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToPageListAsync(pageIndex, pageSize);
        }
        public async Task<List<T>> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> totalNumber, RefAsync<int> totalPage) 
        {
            var result =await ToPageListAsync(pageNumber, pageSize, totalNumber);
            totalPage.Value = (totalNumber.Value + pageSize - 1) / pageSize;
            return result;
        }
        public async Task<string> ToJsonAsync()
        {
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                var result = CacheSchemeMain.GetOrCreate<string>(cacheService, this.QueryBuilder, () =>
                {
                    return this.Context.Utilities.SerializeObject(this.ToList(), typeof(T));
                }, CacheTime, this.Context,CacheKey);
                return result;
            }
            else
            {
                return  this.Context.Utilities.SerializeObject(await this.ToListAsync(), typeof(T));
            }
        }
        public async Task<string> ToJsonPageAsync(int pageIndex, int pageSize)
        {
            return this.Context.Utilities.SerializeObject(await this.ToPageListAsync(pageIndex, pageSize), typeof(T));
        }
        public async Task<string> ToJsonPageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber.Value = await this.Clone().CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToJsonPageAsync(pageIndex, pageSize);
        }
        public async Task<DataTable> ToDataTableAsync()
        {
            QueryBuilder.ResultType = typeof(SugarCacheDataTable);
            InitMapping();
            var sqlObj = this._ToSql();
            RestoreMapping();
            DataTable result = null;
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<DataTable>(cacheService, this.QueryBuilder, () => { return this.Db.GetDataTable(sqlObj.Key, sqlObj.Value.ToArray()); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result = await this.Db.GetDataTableAsync(sqlObj.Key, sqlObj.Value.ToArray());
            }
            return result;
        }
        public Task<DataTable> ToDataTablePageAsync(int pageIndex, int pageSize)
        {
            pageIndex = _PageList(pageIndex, pageSize);
            return ToDataTableAsync();
        }
        public async Task<DataTable> ToDataTablePageAsync(int pageIndex, int pageSize, RefAsync<int> totalNumber)
        {
            var oldMapping = this.Context.MappingTables;
            totalNumber.Value = await this.Clone().CountAsync();
            this.Context.MappingTables = oldMapping;
            return await this.Clone().ToDataTablePageAsync(pageIndex, pageSize);
        }

        #endregion

        #region Private Methods
        public virtual KeyValuePair<string, List<SugarParameter>> _ToSql()
        {
            InitMapping();
            ToSqlBefore();
            string sql = QueryBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, QueryBuilder.Parameters);
        }
        private List<T> GetChildList(Expression<Func<T, object>> parentIdExpression, string pkName, List<T> list, object rootValue,bool isRoot=true)
        {
            var exp = (parentIdExpression as LambdaExpression).Body;
            if (exp is UnaryExpression)
            {
                exp = (exp as UnaryExpression).Operand;
            }
            var parentIdName = (exp as MemberExpression).Member.Name;
            var result = BuildChildList(list, pkName, parentIdName, rootValue);
            return result;
        }

        private static List<T> BuildChildList(List<T> list, string idName, string pIdName, object rootValue)
        {
            var type = typeof(T);
            var idProp = type.GetProperty(idName);
            var pIdProp = type.GetProperty(pIdName);

            var kvpList = list.ToDictionary(x => x, v => idProp.GetValue(v).ObjToString());
            var groupKv = list.GroupBy(x => pIdProp.GetValue(x).ObjToString()).ToDictionary(k => k.Key, v => v.ToList());

            Func<string, List<T>> fc = null;
            fc = (rootVal) =>
            {
                var finalList = new List<T>();
                if (groupKv.TryGetValue(rootVal, out var nextChildList))
                {
                    finalList.AddRange(nextChildList);
                    foreach (var child in nextChildList)
                    {
                        finalList.AddRange(fc(kvpList[child]));
                    }
                }
                return finalList;
            };

            return fc(rootValue.ObjToString());
        }

        private List<T> GetTreeRoot(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, string pk, List<T> list,object rootValue)
        {
            var childName = ((childListExpression as LambdaExpression).Body as MemberExpression).Member.Name;
            var exp = (parentIdExpression as LambdaExpression).Body;
            if (exp is UnaryExpression)
            {
                exp = (exp as UnaryExpression).Operand;
            }
            var parentIdName = (exp as MemberExpression).Member.Name;

            return BuildTree(list, pk, parentIdName, childName, rootValue)?.ToList() ?? default;
        }

        private static IEnumerable<T> BuildTree(IEnumerable<T> list, string idName, string pIdName, string childName, object rootValue)
        {
            var type = typeof(T);
            var mainIdProp = type.GetProperty(idName);
            var pIdProp = type.GetProperty(pIdName);
            var childProp = type.GetProperty(childName);

            var kvList = list.ToDictionary(x => mainIdProp.GetValue(x).ObjToString());
            var group = list.GroupBy(x => pIdProp.GetValue(x).ObjToString());

            var root = rootValue != null ? group.FirstOrDefault(x => x.Key == rootValue.ObjToString()) : group.FirstOrDefault(x => x.Key == null || x.Key == "" || x.Key == "0" || x.Key == Guid.Empty.ToString());

            if (root != null)
            {
                foreach (var item in group)
                {
                    if (kvList.TryGetValue(item.Key, out var parent))
                    {
                        childProp.SetValue(parent, item.ToList());
                    }
                }
            }

            return root;
        }

        public List<T> GetTreeChildList(List<T> alllist, object pkValue, string pkName, string childName, string parentIdName)
        {
            var result = alllist.Where(it =>
            {

                var value = it.GetType().GetProperty(parentIdName).GetValue(it);
                return value.ObjToString() == pkValue.ObjToString();

            }).ToList();
            if (result != null && result.Count > 0)
            {
                foreach (var item in result)
                {
                    var itemPkValue = item.GetType().GetProperty(pkName).GetValue(item);
                    item.GetType().GetProperty(childName).SetValue(item, GetTreeChildList(alllist, itemPkValue, pkName, childName, parentIdName));
                }
            }
            return result;
        }
        protected JoinQueryInfo GetJoinInfo(Expression joinExpression,JoinType joinType)
        {
            QueryBuilder.CheckExpressionNew(joinExpression, "Join");
            QueryBuilder.JoinExpression = joinExpression;
            var express= LambdaExpression.Lambda(joinExpression).Body;
            var lastPareamter= (express as LambdaExpression).Parameters.Last();
            var expResult = this.QueryBuilder.GetExpressionValue(joinExpression, ResolveExpressType.WhereMultiple);
            this.Context.InitMappingInfo(lastPareamter.Type);
            var result= new JoinQueryInfo()
            {
                JoinIndex = QueryBuilder.JoinQueryInfos.Count,
                JoinType = joinType,
                JoinWhere = expResult.GetResultString(),
                ShortName= lastPareamter.Name,
                TableName=this.Context.EntityMaintenance.GetTableName(lastPareamter.Type)
            };
            if (result.JoinIndex == 0) 
            {
                var firstPareamter = (express as LambdaExpression).Parameters.First();
                this.QueryBuilder.TableShortName = firstPareamter.Name;
                if (this.QueryBuilder.AsTables != null && this.QueryBuilder.AsTables.Count==1) 
                {
                    var tableinfo = this.QueryBuilder.AsTables.First();
                    if (this.QueryBuilder.TableWithString!=SqlWith.Null&&this.Context.CurrentConnectionConfig?.MoreSettings?.IsWithNoLockQuery == true&& this.QueryBuilder.AsTables.First().Value.ObjToString().Contains(SqlWith.NoLock) ==false)
                    {
                        this.QueryBuilder.AsTables[tableinfo.Key] = " (SELECT * FROM " + this.QueryBuilder.AsTables.First().Value + $" {SqlWith.NoLock} )";
                    }
                    else 
                    {
                        this.QueryBuilder.AsTables[tableinfo.Key] = " (SELECT * FROM " + this.QueryBuilder.AsTables.First().Value + ")";
                    }
                    this.QueryBuilder.SelectValue = this.QueryBuilder.TableShortName +".*";
                }
            }
            Check.Exception(result.JoinIndex > 10, ErrorMessage.GetThrowMessage("只支持12个表", "Only 12 tables are supported"));
            return result;
        }

        private void _CountEnd(MappingTableList expMapping)
        {
            RestoreMapping();
            QueryBuilder.IsCount = false;
            if (expMapping.Count > 0)
            {
                if (this.QueryableMappingTableList == null)
                {
                    this.QueryableMappingTableList = new MappingTableList();
                }
                this.QueryableMappingTableList.Add(expMapping.First());
            }
        }

        private void _CountBegin(out MappingTableList expMapping, out int result)
        {
            expMapping = new MappingTableList();
            if (QueryBuilder.EntityName == "ExpandoObject" && this.Context.MappingTables.Any(it => it.EntityName == "ExpandoObject"))
            {
                expMapping.Add("ExpandoObject", this.Context.MappingTables.First(it => it.EntityName == "ExpandoObject").DbTableName);
            }
            InitMapping();
            QueryBuilder.IsCount = true;
            result = 0;
        }
        private static string GetTreeKey(EntityInfo entity)
        {
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey || it.IsTreeKey).Count() == 0, "need IsPrimary=true Or IsTreeKey=true");
            string pk = entity.Columns.Where(it => it.IsTreeKey).FirstOrDefault()?.PropertyName;
            if (pk == null)
                pk = entity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault()?.PropertyName;
            return pk;
        }
        protected ISugarQueryable<TResult> _Select<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Select");
            this.Context.InitMappingInfo(typeof(TResult));
            var result = InstanceFactory.GetQueryable<TResult>(this.Context.CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = this.SqlBuilder;
            result.SqlBuilder.QueryBuilder.Parameters = QueryBuilder.Parameters;
            result.SqlBuilder.QueryBuilder.SelectValue = expression;
            result.SqlBuilder.QueryBuilder.IsSelectSingleFiledJson = UtilMethods.IsJsonMember(expression,this.Context);
            if (this.IsCache) 
            {
                result.WithCache(this.CacheTime);
            }
            if (this.QueryBuilder.IsSqlQuery) 
            {
                this.QueryBuilder.IsSqlQuerySelect = true;
            }
            return result;
        }
        protected void _Where(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Where");
            var isSingle = QueryBuilder.IsSingle();
            var result = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(QueryBuilder.WhereInfos.IsNullOrEmpty(), result.GetResultString()));
        }
        protected ISugarQueryable<T> _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            QueryBuilder.CheckExpression(expression, "OrderBy");
            var isSingle = QueryBuilder.IsSingle();
            if (expression.ToString().IsContainsIn("Desc(", "Asc("))
            {
                var orderValue = "";
                var newExp = (expression as LambdaExpression).Body as NewExpression;
                foreach (var item in newExp.Arguments)
                {
                    if (item is MemberExpression)
                    {
                        orderValue +=
                          QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple).GetResultString() + ",";
                    }
                    else
                    {
                        orderValue +=
                            QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple).GetResultString() + ",";
                    }
                }
                orderValue = orderValue.TrimEnd(',');
                OrderBy(orderValue);
                return this;
            }
            else if ((expression as LambdaExpression).Body is NewExpression)
            {
                var newExp = (expression as LambdaExpression).Body as NewExpression;
                var result = "";
                foreach (var item in newExp.Arguments)
                {
                    if (item is MemberExpression)
                    {
                        result +=
                          QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple).GetResultString() + ",";
                    }
                    else
                    {
                        result +=
                            QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple).GetResultString() + ",";
                    }
                }
                result = result.TrimEnd(',');
                OrderBy(result);
                return this;
            }
            else
            {
                var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                OrderBy(lamResult.GetResultString() + UtilConstants.Space + type.ToString().ToUpper());
                return this;
            }
        }
        private void _ToOffsetPage(int pageIndex, int pageSize)
        {
            QueryBuilder.Offset = $" OFFSET {(pageIndex - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }

        private int _PageList(int pageIndex, int pageSize)
        {
            if (pageIndex == 0)
                pageIndex = 1;
            if (QueryBuilder.PartitionByValue.HasValue())
            {
                QueryBuilder.ExternalPageIndex = pageIndex;
                QueryBuilder.ExternalPageSize = pageSize;
            }
            else
            {
                QueryBuilder.Skip = (pageIndex - 1) * pageSize;
                QueryBuilder.Take = pageSize;
            }

            return pageIndex;
        }
        protected ISugarQueryable<T> _GroupBy(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "GroupBy");
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression) 
            {
                var newExp=expression as  NewExpression;
                foreach (var item in newExp.Arguments)
                {
                    if (item is MemberExpression)
                    {
                        result +=
                          QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple).GetResultString() + ",";
                    }
                    else
                    {
                        result +=
                            QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple).GetResultString() + ",";
                    }
                }
                result = result.TrimEnd(',');
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            GroupBy(result);
            return this;
        }
        protected TResult _Min<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Main");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var result = Min<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return result;
        }
        protected async Task<TResult> _MinAsync<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Main");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var result = await MinAsync<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return result;
        }
        protected TResult _Avg<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Avg");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return Avg<TResult>(lamResult.GetResultString());
        }
        protected async Task<TResult> _AvgAsync<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Avg");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            return  await AvgAsync<TResult>(lamResult.GetResultString());
        }
        protected TResult _Max<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Max");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut = Max<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected async Task<TResult> _MaxAsync<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Max");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut =await MaxAsync<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected TResult _Sum<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Sum");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut = Sum<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected async Task<TResult> _SumAsync<TResult>(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Sum");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var reslut =await SumAsync<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        protected ISugarQueryable<T> _As(string tableName, string entityName)
        {
            if (this.QueryBuilder.AsTables != null && this.QueryBuilder.AsTables.Any(it => it.Key == entityName))
            {
                Check.Exception(true, ErrorMessage.GetThrowMessage($"use As<{tableName}>(\"{tableName}\")", $"请把 As(\"{tableName}\"), 改成 As<{tableName}实体>(\"{tableName}\")"));
            }
            else
            {
                this.QueryBuilder.AsTables.Add(entityName, tableName);
            }
            return this;
        }
        protected void _Filter(string FilterName, bool isDisabledGobalFilter)
        {
            QueryBuilder.IsDisabledGobalFilter = isDisabledGobalFilter;
            if (this.Context.QueryFilter.GeFilterList.HasValue() && FilterName.HasValue())
            {
                var list = this.Context.QueryFilter.GeFilterList.Where(it => it.FilterName == FilterName && it.IsJoinQuery == !QueryBuilder.IsSingle());
                foreach (var item in list)
                {
                    var filterResult = item.FilterValue(this.Context);
                    Where(filterResult.Sql + UtilConstants.Space, filterResult.Parameters);
                }
            }
        }
        public ISugarQueryable<T> _PartitionBy(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "PartitionBy");
            LambdaExpression lambda = expression as LambdaExpression;
            expression = lambda.Body;
            var isSingle = QueryBuilder.IsSingle();
            ExpressionResult lamResult = null;
            string result = null;
            if (expression is NewExpression)
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.ArraySingle : ResolveExpressType.ArrayMultiple);
                result = string.Join(",", lamResult.GetResultArray());
            }
            else
            {
                lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
                result = lamResult.GetResultString();
            }
            PartitionBy(result);
            return this;
        }
        protected ISugarQueryable<T> _Having(Expression expression)
        {
            QueryBuilder.CheckExpression(expression, "Having");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            Having(lamResult.GetResultString());
            return this;
        }
        protected List<TResult> _ToList<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this._ToSql();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<List<TResult>>(cacheService, this.QueryBuilder, () => { return GetData<TResult>(sqlObj); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result = GetData<TResult>(sqlObj);
            }
            RestoreMapping();
            _Mapper(result);
            _InitNavigat(result);
            return result;
        }
        private async Task _InitNavigatAsync<TResult>(List<TResult> result)
        {
            if (this.QueryBuilder.Includes != null)
            {
                await Task.Run(() => { _InitNavigat(result); });
            }
        }

        private void _InitNavigat<TResult>(List<TResult> result)
        {
            if (this.QueryBuilder.Includes != null) 
            {
                var managers=(this.QueryBuilder.Includes  as List<object>);
                if (this.QueryBuilder.SelectValue.HasValue()&& this.QueryBuilder.NoCheckInclude==false) 
                {
                    Check.ExceptionEasy("To use includes, use select after tolist()", "使用Includes请在ToList()之后在使用Select");
                }
                foreach (var it in managers)
                {
                    var manager = it as NavigatManager<TResult>;
                    manager.RootList = result;
                    manager.Execute();
                }
            }
        }

        protected async Task<List<TResult>> _ToListAsync<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this._ToSql();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<List<TResult>>(cacheService, this.QueryBuilder, () => { return GetData<TResult>(sqlObj); }, CacheTime, this.Context,CacheKey);
            }
            else
            {
                result =await GetDataAsync<TResult>(sqlObj);
            }
            RestoreMapping();
            _Mapper(result);
            await _InitNavigatAsync(result);
            return result;
        }

        protected void _Mapper<TResult>(List<TResult> result)
        {
            if (this.EntityInfo.Columns.Any(it => it.IsTranscoding))
            {
                foreach (var item in result)
                {
                    foreach (var column in this.EntityInfo.Columns.Where(it => it.IsTranscoding))
                    {
                        var value = column.PropertyInfo.GetValue(item, null);
                        if (value != null)
                        {
                            column.PropertyInfo.SetValue(item, UtilMethods.DecodeBase64(value.ToString()), null);
                        }
                    }
                }
            }
            if (this.Mappers.HasValue())
            {
                foreach (var mapper in this.Mappers)
                {
                    if (typeof(TResult) == typeof(T))
                    {
                        mapper(result.Select(it => (T)Convert.ChangeType(it, typeof(T))).ToList());
                    }
                    else
                    {
                        Check.Exception(true, "{0} and {1} are not a type, Try .select().mapper().ToList", typeof(TResult).FullName, typeof(T).FullName);
                    }
                }
            }
            if (this.MapperAction != null)
            {
                foreach (TResult item in result)
                {
                    if (typeof(TResult) == typeof(T))
                    {
                        foreach (var mapper in this.MapperAction)
                        {
                            mapper((T)(item as object));
                        }
                    }
                    else
                    {
                        Check.Exception(true, "{0} and {1} are not a type, Try .select().mapper().ToList", typeof(TResult).FullName, typeof(T).FullName);
                    }
                }
            }
            if (this.MapperActionWithCache != null)
            {
                if (typeof(TResult) == typeof(T))
                {
                    var list = (List<T>)Convert.ChangeType(result, typeof(List<T>));
                    var mapperCache = new MapperCache<T>(list, this.Context);
                    foreach (T item in list)
                    {
                        mapperCache.GetIndex = 0;
                        this.MapperActionWithCache(item, mapperCache);
                    }
                }
                else
                {
                    Check.Exception(true, "{0} and {1} are not a type, Try .select().mapper().ToList", typeof(TResult).FullName, typeof(T).FullName);
                }
            }
        }

        private ISugarQueryable<T> _Mapper<TObject>(Expression mapperObject, Expression mapperField)
        {
            if ((mapperObject as LambdaExpression).Body is UnaryExpression)
            {
                mapperObject = ((mapperObject as LambdaExpression).Body as UnaryExpression).Operand;
            }
            else
            {
                mapperObject = (mapperObject as LambdaExpression).Body;
            }
            if ((mapperField as LambdaExpression).Body is UnaryExpression)
            {
                mapperField = ((mapperField as LambdaExpression).Body as UnaryExpression).Operand;
            }
            else
            {
                mapperField = (mapperField as LambdaExpression).Body;
            }
            Check.Exception(mapperObject is MemberExpression == false || mapperField is MemberExpression == false, ".Mapper() parameter error");
            var mapperObjectExp = mapperObject as MemberExpression;
            var mapperFieldExp = mapperField as MemberExpression;
            Check.Exception(mapperFieldExp.Type.IsClass(), ".Mapper() parameter error");
            var objType = mapperObjectExp.Type;
            var filedType = mapperFieldExp.Expression.Type;
            Check.Exception(objType != typeof(TObject) && objType != typeof(List<TObject>), ".Mapper() parameter error");
            if (objType == typeof(List<TObject>))
            {
                objType = typeof(TObject);
            }
            var filedName = mapperFieldExp.Member.Name;
            var objName = mapperObjectExp.Member.Name;
            var filedEntity = this.Context.EntityMaintenance.GetEntityInfo(objType);
            var objEntity = this.Context.EntityMaintenance.GetEntityInfo(filedType);
            var isSelf = filedType == typeof(T);
            if (Mappers == null)
                Mappers = new List<Action<List<T>>>();
            if (isSelf)
            {
                Action<List<T>> mapper = (entitys) =>
                {
                    if (entitys.IsNullOrEmpty() || !entitys.Any()) return;
                    var entity = entitys.First();
                    var whereCol = filedEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals(filedName, StringComparison.CurrentCultureIgnoreCase));
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => (it.PropertyName).Equals(it.EntityName + "id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        Check.Exception(true, ".Mapper() parameter error");
                    }
                    List<string> inValues = entitys.Select(it => it.GetType().GetProperty(filedName).GetValue(it, null).ObjToString()).ToList();
                    if (inValues!=null&& inValues.Any()&&UtilMethods.GetUnderType(entitys.First().GetType().GetProperty(filedName).PropertyType) == UtilConstants.GuidType) 
                    {
                        inValues = inValues.Select(x => x == "" ? "null" : x).Distinct().ToList();
                    }
                    List<IConditionalModel> wheres = new List<IConditionalModel>()
                    {
                       new ConditionalModel()
                      {
                           FieldName=this.SqlBuilder.GetTranslationColumnName(whereCol.DbColumnName),
                           ConditionalType= ConditionalType.In,
                           FieldValue=string.Join(",",inValues.Distinct()),
                           CSharpTypeName=whereCol.PropertyInfo.PropertyType.Name
                      }
                    };
                    var list = this.Context.Queryable<TObject>().Where(wheres).ToList();
                    foreach (var item in entitys)
                    {
                        var whereValue = item.GetType().GetProperty(filedName).GetValue(item, null);
                        var setValue = list.Where(x => x.GetType().GetProperty(whereCol.PropertyName).GetValue(x, null).ObjToString() == whereValue.ObjToString()).ToList();
                        var setObject = item.GetType().GetProperty(objName);
                        if (setObject.PropertyType.FullName.IsCollectionsList())
                        {
                            setObject.SetValue(item, setValue.ToList(), null);
                        }
                        else
                        {
                            setObject.SetValue(item, setValue.FirstOrDefault(), null);
                        }
                    }
                };
                Mappers.Add(mapper);
            }
            else
            {
                Action<List<T>> mapper = (entitys) =>
                {
                    if (entitys.IsNullOrEmpty() || !entitys.Any()) return;
                    var entity = entitys.First();
                    var tEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                    var whereCol = tEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals(filedName, StringComparison.CurrentCultureIgnoreCase));
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => (it.PropertyName).Equals(it.EntityName + "id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        Check.Exception(true, ".Mapper() parameter error");
                    }
                    List<string> inValues = entitys.Select(it => it.GetType().GetProperty(whereCol.PropertyName).GetValue(it, null).ObjToString()).ToList();
                    var dbColumnName = filedEntity.Columns.FirstOrDefault(it => it.PropertyName == filedName).DbColumnName;
                    List<IConditionalModel> wheres = new List<IConditionalModel>()
                    {
                       new ConditionalModel()
                      {
                           FieldName=dbColumnName,
                           ConditionalType= ConditionalType.In,
                           FieldValue=string.Join(",",inValues)
                      }
                    };
                    var list = this.Context.Queryable<TObject>().Where(wheres).ToList();
                    foreach (var item in entitys)
                    {
                        var whereValue = item.GetType().GetProperty(whereCol.PropertyName).GetValue(item, null);
                        var setValue = list.Where(x => x.GetType().GetProperty(filedName).GetValue(x, null).ObjToString() == whereValue.ObjToString()).ToList();
                        var setObject = item.GetType().GetProperty(objName);
                        if (setObject.PropertyType.FullName.IsCollectionsList())
                        {
                            setObject.SetValue(item, setValue.ToList(), null);
                        }
                        else
                        {
                            setObject.SetValue(item, setValue.FirstOrDefault(), null);
                        }
                    }
                };
                Mappers.Add(mapper);
            }

            return this;
        }

        private ISugarQueryable<T> _Mapper<TObject>(Expression mapperObject, Expression mainField, Expression childField)
        {
            if ((mapperObject as LambdaExpression).Body is UnaryExpression)
            {
                mapperObject = ((mapperObject as LambdaExpression).Body as UnaryExpression).Operand;
            }
            else
            {
                mapperObject = (mapperObject as LambdaExpression).Body;
            }
            if ((mainField as LambdaExpression).Body is UnaryExpression)
            {
                mainField = ((mainField as LambdaExpression).Body as UnaryExpression).Operand;
            }
            else
            {
                mainField = (mainField as LambdaExpression).Body;
            }
            if ((childField as LambdaExpression).Body is UnaryExpression)
            {
                childField = ((childField as LambdaExpression).Body as UnaryExpression).Operand;
            }
            else
            {
                childField = (childField as LambdaExpression).Body;
            }
            Check.Exception(mapperObject is MemberExpression == false || mainField is MemberExpression == false, ".Mapper() parameter error");
            var mapperObjectExp = mapperObject as MemberExpression;
            var mainFieldExp = mainField as MemberExpression;
            var childFieldExp = childField as MemberExpression;
            Check.Exception(mainFieldExp.Type.IsClass(), ".Mapper() parameter error");
            Check.Exception(childFieldExp.Type.IsClass(), ".Mapper() parameter error");
            var objType = mapperObjectExp.Type;
            var filedType = mainFieldExp.Expression.Type;
            Check.Exception(objType != typeof(TObject) && objType != typeof(List<TObject>), ".Mapper() parameter error");
            if (objType == typeof(List<TObject>))
            {
                objType = typeof(TObject);
            }
            var mainFiledName = mainFieldExp.Member.Name;
            var childFiledName = childFieldExp.Member.Name;
            var objName = mapperObjectExp.Member.Name;
            var filedEntity = this.Context.EntityMaintenance.GetEntityInfo(objType);
            var objEntity = this.Context.EntityMaintenance.GetEntityInfo(filedType);
            var isSelf = filedType == typeof(T);
            if (Mappers == null)
                Mappers = new List<Action<List<T>>>();
            if (isSelf)
            {
                Action<List<T>> mapper = (entitys) =>
                {
                    if (entitys.IsNullOrEmpty() || !entitys.Any()) return;
                    var entity = entitys.First();
                    var whereCol = filedEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals(childFiledName, StringComparison.CurrentCultureIgnoreCase));
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        whereCol = filedEntity.Columns.FirstOrDefault(it => (it.PropertyName).Equals(it.EntityName + "id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        Check.Exception(true, ".Mapper() parameter error");
                    }
                    List<string> inValues = entitys.Select(it => it.GetType().GetProperty(mainFiledName).GetValue(it, null).ObjToString()).ToList();
                    List<IConditionalModel> wheres = new List<IConditionalModel>()
                    {
                       new ConditionalModel()
                      {
                           FieldName=whereCol.DbColumnName,
                           ConditionalType= ConditionalType.In,
                           FieldValue=string.Join(",",inValues.Distinct())
                      }
                    };
                    var list = this.Context.Queryable<TObject>().Where(wheres).ToList();
                    foreach (var item in entitys)
                    {
                        var whereValue = item.GetType().GetProperty(mainFiledName).GetValue(item, null);
                        var setValue = list.Where(x => x.GetType().GetProperty(whereCol.PropertyName).GetValue(x, null).ObjToString() == whereValue.ObjToString()).ToList();
                        var setObject = item.GetType().GetProperty(objName);
                        if (setObject.PropertyType.FullName.IsCollectionsList())
                        {
                            setObject.SetValue(item, setValue.ToList(), null);
                        }
                        else
                        {
                            setObject.SetValue(item, setValue.FirstOrDefault(), null);
                        }
                    }
                };
                Mappers.Add(mapper);
            }
            else
            {
                Action<List<T>> mapper = (entitys) =>
                {
                    if (entitys.IsNullOrEmpty() || !entitys.Any()) return;
                    var entity = entitys.First();
                    var tEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                    var whereCol = tEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals(childFiledName, StringComparison.CurrentCultureIgnoreCase));
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => GetPrimaryKeys().Any(pk => pk.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)));
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => it.PropertyName.Equals("id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        whereCol = tEntity.Columns.FirstOrDefault(it => (it.PropertyName).Equals(it.EntityName + "id", StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (whereCol == null)
                    {
                        Check.Exception(true, ".Mapper() parameter error");
                    }
                    List<string> inValues = entitys.Select(it => it.GetType().GetProperty(whereCol.PropertyName).GetValue(it, null).ObjToString()).ToList();
                    var dbColumnName = filedEntity.Columns.FirstOrDefault(it => it.PropertyName == mainFiledName).DbColumnName;
                    List<IConditionalModel> wheres = new List<IConditionalModel>()
                    {
                       new ConditionalModel()
                      {
                           FieldName=dbColumnName,
                           ConditionalType= ConditionalType.In,
                           FieldValue=string.Join(",",inValues)
                      }
                    };
                    var list = this.Context.Queryable<TObject>().Where(wheres).ToList();
                    foreach (var item in entitys)
                    {
                        var whereValue = item.GetType().GetProperty(whereCol.PropertyName).GetValue(item, null);
                        var setValue = list.Where(x => x.GetType().GetProperty(mainFiledName).GetValue(x, null).ObjToString() == whereValue.ObjToString()).ToList();
                        var setObject = item.GetType().GetProperty(objName);
                        if (setObject.PropertyType.FullName.IsCollectionsList())
                        {
                            setObject.SetValue(item, setValue.ToList(), null);
                        }
                        else
                        {
                            setObject.SetValue(item, setValue.FirstOrDefault(), null);
                        }
                    }
                };
                Mappers.Add(mapper);
            }

            return this;
        }
        protected int GetCount()
        {
            var sql = string.Empty;
            ToSqlBefore();
            sql = QueryBuilder.ToSqlString();
            sql = QueryBuilder.ToCountSql(sql);
            var result = Context.Ado.GetInt(sql, QueryBuilder.Parameters.ToArray());
            return result;
        }
        protected async Task<int> GetCountAsync()
        {
            var sql = string.Empty;
            ToSqlBefore();
            sql = QueryBuilder.ToSqlString();
            sql = QueryBuilder.ToCountSql(sql);
            var result =Convert.ToInt32(await Context.Ado.GetScalarAsync(sql, QueryBuilder.Parameters.ToArray()));
            return result;
        }

        private void ToSqlBefore()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            if (moreSetts != null && moreSetts.IsWithNoLockQuery && string.IsNullOrEmpty(QueryBuilder.TableWithString))
            {
                this.With(SqlWith.NoLock);
            }
        }

        protected List<TResult> GetData<TResult>(KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            List<TResult> result;
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            bool isChangeQueryableSlave = GetIsSlaveQuery();
            bool isChangeQueryableMasterSlave = GetIsMasterQuery();
            var dataReader = this.Db.GetDataReader(sqlObj.Key, sqlObj.Value.ToArray());
            result = GetData<TResult>(isComplexModel, entityType, dataReader);
            RestChangeMasterQuery(isChangeQueryableMasterSlave);
            RestChangeSlaveQuery(isChangeQueryableSlave);
            return result;
        }

        private void RestChangeMasterQuery(bool isChangeQueryableMasterSlave)
        {
            if (isChangeQueryableMasterSlave)
                this.Context.Ado.IsDisableMasterSlaveSeparation = false;
        }

        private bool GetIsMasterQuery()
        {
            var isChangeQueryableMasterSlave =
                                   this.QueryBuilder.IsDisableMasterSlaveSeparation == true &&
                                   this.Context.Ado.IsDisableMasterSlaveSeparation == false &&
                                   this.Context.Ado.Transaction == null;
            if (isChangeQueryableMasterSlave)
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            return isChangeQueryableMasterSlave;
        }

        private void RestChangeSlaveQuery(bool isChangeQueryableSlaveSlave)
        {
            if (isChangeQueryableSlaveSlave)
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
        }

        private bool GetIsSlaveQuery()
        {
            var isChangeQueryableMasterSlave =
                                   this.QueryBuilder.IsEnableMasterSlaveSeparation == true &&
                                   this.Context.Ado.IsDisableMasterSlaveSeparation == true &&
                                   this.Context.Ado.Transaction == null;
            if (isChangeQueryableMasterSlave)
                this.Context.Ado.IsDisableMasterSlaveSeparation = false;
            return isChangeQueryableMasterSlave;
        }

        protected async Task<List<TResult>> GetDataAsync<TResult>(KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            List<TResult> result;
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            bool isChangeQueryableSlave = GetIsSlaveQuery();
            bool isChangeQueryableMasterSlave = GetIsMasterQuery();
            var dataReader = await this.Db.GetDataReaderAsync(sqlObj.Key, sqlObj.Value.ToArray());
            result =await GetDataAsync<TResult>(isComplexModel, entityType, dataReader);
            RestChangeMasterQuery(isChangeQueryableMasterSlave);
            RestChangeSlaveQuery(isChangeQueryableSlave);
            return result;
        }

        private List<TResult> GetData<TResult>(bool isComplexModel, Type entityType, IDataReader dataReader)
        {
            List<TResult> result;
            if (entityType == UtilConstants.DynamicType)
            {
                result = this.Context.Utilities.DataReaderToExpandoObjectList(dataReader) as List<TResult>;
            }
            else if (entityType == UtilConstants.ObjType)
            {
                result = this.Context.Utilities.DataReaderToExpandoObjectList(dataReader).Select(it => ((TResult)(object)it)).ToList();
            }
            else if (QueryBuilder.IsSelectSingleFiledJson) 
            {
                result= this.Context.Utilities.DataReaderToSelectJsonList<TResult>(dataReader);
            }
            else if (entityType.IsAnonymousType() || isComplexModel)
            {
                result = this.Context.Utilities.DataReaderToList<TResult>(dataReader);
            }
            else
            {
                result = this.Bind.DataReaderToList<TResult>(entityType, dataReader);
            }
            SetContextModel(result, entityType);
            return result;
        }
        private async Task<List<TResult>> GetDataAsync<TResult>(bool isComplexModel, Type entityType, IDataReader dataReader)
        {
            List<TResult> result;
            if (entityType == UtilConstants.DynamicType)
            {
                result =await this.Context.Utilities.DataReaderToExpandoObjectListAsync(dataReader) as List<TResult>;
            }
            else if (entityType == UtilConstants.ObjType)
            {
                var expObj = await this.Context.Utilities.DataReaderToExpandoObjectListAsync(dataReader);
                result = expObj.Select(it => ((TResult)(object)it)).ToList();
            }
            else if (QueryBuilder.IsSelectSingleFiledJson)
            {
                result= await this.Context.Utilities.DataReaderToSelectJsonListAsync<TResult>(dataReader);
            }
            else if (entityType.IsAnonymousType() || isComplexModel)
            {
                result =await this.Context.Utilities.DataReaderToListAsync<TResult>(dataReader);
            }
            else
            {
                result =await this.Bind.DataReaderToListAsync<TResult>(entityType, dataReader);
            }
            SetContextModel(result, entityType);
            return result;
        }

        protected void _InQueryable(Expression expression, KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            QueryBuilder.CheckExpression(expression, "In");
            string sql = sqlObj.Key;
            if (sqlObj.Value.HasValue())
            {
                this.SqlBuilder.RepairReplicationParameters(ref sql, sqlObj.Value.ToArray(), 100);
                this.QueryBuilder.Parameters.AddRange(sqlObj.Value);
            }
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            var whereSql = string.Format(this.QueryBuilder.InTemplate, fieldName, sql);
            this.QueryBuilder.WhereInfos.Add(SqlBuilder.AppendWhereOrAnd(this.QueryBuilder.WhereInfos.IsNullOrEmpty(), whereSql));
            base._InQueryableIndex += 100;
        }

        protected List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        protected virtual List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.EntityInfo.DbTableName);
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            }
        }

        protected void RestoreMapping()
        {
            if (IsAs && _RestoreMapping)
            {
                this.Context.MappingTables = OldMappingTableList == null ? new MappingTableList() : OldMappingTableList;
            }
        }
        protected void InitMapping()
        {
            if (this.QueryableMappingTableList != null)
                this.Context.MappingTables = this.QueryableMappingTableList;
        }

        private void SetContextModel<TResult>(List<TResult> result, Type entityType)
        {
            if (result.HasValue())
            {
                if (UtilMethods.GetRootBaseType(entityType).HasValue() && UtilMethods.GetRootBaseType(entityType) == UtilConstants.ModelType)
                {
                    foreach (var item in result)
                    {
                        var contextProperty = item.GetType().GetProperty("Context");
                        SqlSugarProvider newClient = this.Context.Utilities.CopyContext();
                        newClient.Ado.IsDisableMasterSlaveSeparation = true;
                        if (newClient.CurrentConnectionConfig.AopEvents == null)
                            newClient.CurrentConnectionConfig.AopEvents = new AopEvents();
                        contextProperty.SetValue(item, newClient, null);
                    }
                }
            }
        }
        protected void CopyQueryBuilder(QueryBuilder asyncQueryableBuilder)
        {
            var pars = new List<SugarParameter>();
            if (this.QueryBuilder.Parameters != null)
            {
                pars=this.QueryBuilder.Parameters.Select(it=>new SugarParameter(it.ParameterName,it.Value) {
                       DbType=it.DbType,
                       Value=it.Value,
                       ParameterName=it.ParameterName,
                       Direction=it.Direction,
                       IsArray=it.IsArray,
                       IsJson=it.IsJson,
                       IsNullable=it.IsNullable,
                       IsRefCursor=it.IsRefCursor,
                       Size=it.Size,
                       SourceColumn=it.SourceColumn,
                       SourceColumnNullMapping=it.SourceColumnNullMapping,
                       SourceVersion=it.SourceVersion,
                       TempDate=it.TempDate,
                       TypeName=it.TypeName,
                       UdtTypeName=it.UdtTypeName,
                       _Size=it._Size
                }).ToList();
            }
            asyncQueryableBuilder.IsEnableMasterSlaveSeparation = this.QueryBuilder.IsEnableMasterSlaveSeparation;
            asyncQueryableBuilder.TranLock = this.QueryBuilder.TranLock;
            asyncQueryableBuilder.IsDisableMasterSlaveSeparation = this.QueryBuilder.IsDisableMasterSlaveSeparation;
            asyncQueryableBuilder.IsQueryInQuery = this.QueryBuilder.IsQueryInQuery;
            asyncQueryableBuilder.Includes = this.QueryBuilder.Includes;
            asyncQueryableBuilder.Take = this.QueryBuilder.Take;
            asyncQueryableBuilder.Skip = this.QueryBuilder.Skip;
            asyncQueryableBuilder.SelectValue = this.QueryBuilder.SelectValue;
            asyncQueryableBuilder.WhereInfos = this.Context.Utilities.TranslateCopy(this.QueryBuilder.WhereInfos);
            asyncQueryableBuilder.EasyJoinInfos = this.Context.Utilities.TranslateCopy(this.QueryBuilder.EasyJoinInfos);
            asyncQueryableBuilder.JoinQueryInfos = this.Context.Utilities.TranslateCopy(this.QueryBuilder.JoinQueryInfos);
            asyncQueryableBuilder.WhereIndex = this.QueryBuilder.WhereIndex;
            asyncQueryableBuilder.EntityType = this.QueryBuilder.EntityType;
            asyncQueryableBuilder.EntityName = this.QueryBuilder.EntityName;
            asyncQueryableBuilder.Parameters = pars;
            asyncQueryableBuilder.TableShortName = this.QueryBuilder.TableShortName;
            asyncQueryableBuilder.TableWithString = this.QueryBuilder.TableWithString;
            asyncQueryableBuilder.GroupByValue = this.QueryBuilder.GroupByValue;
            asyncQueryableBuilder.IsDistinct = this.QueryBuilder.IsDistinct;
            asyncQueryableBuilder.OrderByValue = this.QueryBuilder.OrderByValue;
            asyncQueryableBuilder.IsDisabledGobalFilter = this.QueryBuilder.IsDisabledGobalFilter;
            asyncQueryableBuilder.PartitionByValue = this.QueryBuilder.PartitionByValue;
            asyncQueryableBuilder.JoinExpression = this.QueryBuilder.JoinExpression;
            asyncQueryableBuilder.WhereIndex = this.QueryBuilder.WhereIndex;
            asyncQueryableBuilder.HavingInfos = this.QueryBuilder.HavingInfos;
            asyncQueryableBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            asyncQueryableBuilder.IgnoreColumns = this.Context.Utilities.TranslateCopy(this.QueryBuilder.IgnoreColumns);
            asyncQueryableBuilder.AsTables = this.Context.Utilities.TranslateCopy(this.QueryBuilder.AsTables);
            asyncQueryableBuilder.DisableTop = this.QueryBuilder.DisableTop;
            asyncQueryableBuilder.Offset = this.QueryBuilder.Offset;
            asyncQueryableBuilder.IsSqlQuery = this.QueryBuilder.IsSqlQuery;
            asyncQueryableBuilder.IsSqlQuerySelect = this.QueryBuilder.IsSqlQuerySelect;
            asyncQueryableBuilder.OldSql = this.QueryBuilder.OldSql;
            asyncQueryableBuilder.IsCrossQueryWithAttr = this.QueryBuilder.IsCrossQueryWithAttr;
            asyncQueryableBuilder.CrossQueryItems = this.QueryBuilder.CrossQueryItems;
        }
        protected int SetCacheTime(int cacheDurationInSeconds)
        {
            if (cacheDurationInSeconds == int.MaxValue && this.Context.CurrentConnectionConfig.MoreSettings != null && this.Context.CurrentConnectionConfig.MoreSettings.DefaultCacheDurationInSeconds > 0)
            {
                cacheDurationInSeconds = this.Context.CurrentConnectionConfig.MoreSettings.DefaultCacheDurationInSeconds;
            }

            return cacheDurationInSeconds;
        }

        #endregion
    }
    #endregion
    #region T2
    public partial class QueryableProvider<T, T2> : QueryableProvider<T>, ISugarQueryable<T, T2>
    {
        public ISugarQueryable<T, T2,T3> LeftJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2,T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2,T3> InnerJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2,T3> RightJoin<T3>(ISugarQueryable<T3> joinQueryable, Expression<Func<T, T2,T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2,T3> LeftJoin<T3>(Expression<Func<T, T2,T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2,T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3> FullJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2, T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2,T3> InnerJoin<T3>(Expression<Func<T, T2,T3, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T3>();
            var result = InstanceFactory.GetQueryable<T, T2,T3>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }
        public new ISugarQueryable<T, T2> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T,T2> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        #endregion

        #region Order
        public new ISugarQueryable<T, T2> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public ISugarQueryable<T, T2> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new virtual ISugarQueryable<T,T2> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2> OrderByDescending(Expression<Func<T,T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new virtual ISugarQueryable<T,T2> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new virtual ISugarQueryable<T,T2> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public  virtual ISugarQueryable<T, T2> HavingIF(bool isHaving, Expression<Func<T,T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public new ISugarQueryable<T, T2> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T,T2> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2> Clone()
        {
            var queryable = this.Context.Queryable<T, T2>((t, t2) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (isCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }

        public new ISugarQueryable<T, T2>  Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        #endregion
    }
    #endregion
    #region T3
    public partial class QueryableProvider<T, T2, T3> : QueryableProvider<T>, ISugarQueryable<T, T2, T3>
    {
        public ISugarQueryable<T, T2, T3,T4> LeftJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3,T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3,T4> InnerJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3,T4> RightJoin<T4>(ISugarQueryable<T4> joinQueryable, Expression<Func<T, T2, T3,T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3,T4> LeftJoin<T4>(Expression<Func<T, T2, T3,T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3,T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> FullJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }


        public ISugarQueryable<T, T2, T3,T4> InnerJoin<T4>(Expression<Func<T, T2, T3,T4, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T4>();
            var result = InstanceFactory.GetQueryable<T, T2, T3,T4>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        #region  Group 
        public new virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }
        public new virtual ISugarQueryable<T, T2,T3> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public  virtual ISugarQueryable<T, T2,T3> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public  virtual ISugarQueryable<T, T2, T3> HavingIF(bool isHaving, Expression<Func<T, T2,T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Order
        public new virtual ISugarQueryable<T, T2,T3> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2,T3> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2,T3> OrderByDescending(Expression<Func<T, T2,T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderBy(Expression<Func<T, object>> expression, OrderByType type)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }

        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        #endregion

        #region Where
        public ISugarQueryable<T, T2, T3> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2,T3> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            return _MaxAsync<TResult>(expression);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            return _MinAsync<TResult>(expression);
        }
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            return _SumAsync<TResult>(expression);
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            return _AvgAsync<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2,T3> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2,T3> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3>((t, t2, t3) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T4
    public partial class QueryableProvider<T, T2, T3, T4> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4>
    {
        public ISugarQueryable<T, T2, T3, T4,T5> LeftJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4,T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4,T5> InnerJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4,T5> RightJoin<T5>(ISugarQueryable<T5> joinQueryable, Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4,T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> FullJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T5>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2,T3,T4> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3,T4> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2,T3,T4> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> GroupByIF(bool isGroupBy, Expression<Func<T, T2, T3,T4, object>> expression)
        {
            if (isGroupBy)
            {
                GroupBy(expression);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public  new virtual ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public  virtual ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public  virtual ISugarQueryable<T, T2, T3,T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4> HavingIF(bool isHaving, Expression<Func<T, T2, T3,T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2,T3,T4> InIF<TParamter>(bool isIn, params TParamter[] pkValues)
        {
            if (isIn)
            {
                In(pkValues);
            }
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, params FieldType[] inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, List<FieldType> inValues)
        {
            QueryBuilder.CheckExpression(expression, "In");
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4> In<FieldType>(Expression<Func<T, T2, T3, T4, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion


        #region Other
        public new ISugarQueryable<T, T2,T3,T4> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4>((t, t2, t3, t4) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T5
    public partial class QueryableProvider<T, T2, T3, T4, T5> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5>
    {
        public ISugarQueryable<T, T2, T3, T4, T5,T6> LeftJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(ISugarQueryable<T6> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5,T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> FullJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T6>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }

        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4,T5> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }

        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4,T5> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4,T5> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4,T5> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4,T5> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public new virtual ISugarQueryable<T, T2, T3, T4,T5> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4,T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4,T5> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5>((t, t2, t3, t4, t5) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T6
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6,T7> LeftJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(ISugarQueryable<T7> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> FullJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T7>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> Where(IFuncModel funcModel)
        {
            var obj = this.SqlBuilder.FuncModelToSql(funcModel);
            return this.Where(obj.Key, obj.Value);
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }

        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5,T6> OrderByDescending(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            this._OrderBy(expression, OrderByType.Desc);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5,T6> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            this._Having(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6> Having(Expression<Func<T, T2, T3, T4, T5,T6, bool>> expression)
        {
            this._Having(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5,T6> Having(string whereString, object whereObj)
        {
            base.Having(whereString, whereObj);
            return this;
        }

        public new virtual ISugarQueryable<T, T2, T3, T4, T5,T6> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5,T6, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5,T6> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6>((t, t2, t3, t4, t5, T6) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T7
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(ISugarQueryable<T8> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> FullJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T8>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public new virtual ISugarQueryable<T, T2, T3, T4, T5, T6,T7> HavingIF(bool isHaving, Expression<Func<T, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        public virtual ISugarQueryable<T, T2, T3, T4, T5, T6, T7> HavingIF(bool isHaving, Expression<Func<T, T2, T3, T4, T5, T6,T7, bool>> expression)
        {
            if (isHaving)
                this._Having(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6,T7> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7>((t, t2, t3, t4, t5, T6, t7) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T8
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(ISugarQueryable<T9> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> FullJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T9>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        /// <summary>
        /// if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClass"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(ClassType whereClass, bool ignoreDefaultValue = false) where ClassType : class, new()
        {
            base.WhereClass(whereClass, ignoreDefaultValue);
            return this;
        }
        /// <summary>
        ///  if a property that is not empty is a condition
        /// </summary>
        /// <param name="whereClassTypes"></param>
        /// <returns></returns>
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WhereClass<ClassType>(List<ClassType> whereClassTypes, bool ignoreDefaultValue = false) where ClassType : class, new()
        {

            base.WhereClass(whereClassTypes, ignoreDefaultValue);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8>((t, t2, t3, t4, t5, T6, t7, t8) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }

        public bool Any(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            var result = Any();
            this.QueryBuilder.WhereInfos.Remove(this.QueryBuilder.WhereInfos.Last());
            return result;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> Distinct()
        {
            QueryBuilder.IsDistinct = true;
            return this;
        }
        #endregion
    }
    #endregion
    #region T9
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(ISugarQueryable<T10> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> FullJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T10>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }


        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7,T8,T9> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8,T9> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9>((t, t2, t3, t4, t5, T6, t7, t8, t9) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T10
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> InnerJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(ISugarQueryable<T11> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> LeftJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> FullJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> RightJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> InnerJoin<T11>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T11>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T11
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Left);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Inner);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(ISugarQueryable<T12> joinQueryable, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            var joinInfo = GetJoinInfo(joinExpression, JoinType.Right);
            var sqlObject = joinQueryable.ToSql();
            string sql = sqlObject.Key;
            this.QueryBuilder.LambdaExpressions.ParameterIndex += 100;
            UtilMethods.RepairReplicationParameters(ref sql, sqlObject.Value.ToArray(), this.QueryBuilder.LambdaExpressions.ParameterIndex, "");
            joinInfo.TableName = "(" + sql + ")";
            this.QueryBuilder.Parameters.AddRange(sqlObject.Value);
            result.QueryBuilder.JoinQueryInfos.Add(joinInfo);
            result.QueryBuilder.LambdaExpressions.ParameterIndex = this.QueryBuilder.LambdaExpressions.ParameterIndex;
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> LeftJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Left));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> FullJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Full));
            return result;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> RightJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Right));
            return result;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> InnerJoin<T12>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> joinExpression)
        {
            this.Context.InitMappingInfo<T12>();
            var result = InstanceFactory.GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Context.CurrentConnectionConfig);
            result.SqlBuilder = this.SqlBuilder;
            result.Context = this.Context;
            result.QueryBuilder.JoinQueryInfos.Add(GetJoinInfo(joinExpression, JoinType.Inner));
            return result;
        }
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10,T11> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10, t11) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
    #region T12
    public partial class QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T>, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        #region Where
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(List<IConditionalModel> conditionalModels)
        {
            base.Where(conditionalModels);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }

        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expression)
        {
            if (isWhere)
                _Where(expression);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Where(string whereString, object whereObj)
        {
            Where<T>(whereString, whereObj);
            return this;
        }

        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WhereIF(bool isWhere, string whereString, object whereObj)
        {
            if (!isWhere) return this;
            this.Where<T>(whereString, whereObj);
            return this;
        }
        #endregion

        #region Select
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        public ISugarQueryable<TResult> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Select<TResult>(expression);
        }
        #endregion

        #region OrderBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderBy(string orderFileds)
        {
            base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, string orderFileds)
        {
            if (isOrderBy)
                base.OrderBy(orderFileds);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OrderByIF(bool isOrderBy, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12, object>> expression, OrderByType type = OrderByType.Asc)
        {
            if (isOrderBy)
                _OrderBy(expression, type);
            return this;
        }
        #endregion

        #region GroupBy
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        public ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GroupBy(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }
        #endregion

        #region Aggr
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Max<TResult>(expression);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Min<TResult>(expression);
        }
        public TResult Sum<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Sum<TResult>(expression);
        }
        public TResult Avg<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            return _Avg<TResult>(expression);
        }
        #endregion

        #region In
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, params FieldType[] inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, List<FieldType> inValues)
        {
            var isSingle = QueryBuilder.IsSingle();
            var lamResult = QueryBuilder.GetExpressionValue(expression, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple);
            var fieldName = lamResult.GetResultString();
            In(fieldName, inValues);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> In<FieldType>(Expression<Func<T, object>> expression, ISugarQueryable<FieldType> childQueryExpression)
        {
            var sqlObj = childQueryExpression.ToSql();
            _InQueryable(expression, sqlObj);
            return this;
        }
        #endregion

        #region Other
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11,T12> Take(int num)
        {
            QueryBuilder.Take = num;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Clone()
        {
            var queryable = this.Context.Queryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>((t, t2, t3, t4, t5, T6, t7, t8, t9, t10, t11, t12) => new object[] { }).WithCacheIF(IsCache, CacheTime);
            base.CopyQueryBuilder(queryable.QueryBuilder);
            return queryable;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS<AsT>(string tableName)
        {
            var entityName = typeof(AsT).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            _As(tableName, entityName);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Filter(string FilterName, bool isDisabledGobalFilter = false)
        {
            _Filter(FilterName, isDisabledGobalFilter);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(object parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(SugarParameter[] parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddParameters(List<SugarParameter> parameters)
        {
            if (parameters != null)
                QueryBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> AddJoinInfo(string tableName, string shortName, string joinWhere, JoinType type = JoinType.Left)
        {
            QueryBuilder.JoinIndex = +1;
            QueryBuilder.JoinQueryInfos
                .Add(new JoinQueryInfo()
                {
                    JoinIndex = QueryBuilder.JoinIndex,
                    TableName = tableName,
                    ShortName = shortName,
                    JoinType = type,
                    JoinWhere = joinWhere
                });
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> With(string withString)
        {
            base.With(withString);
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCache(int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            this.IsCache = true;
            this.CacheTime = cacheDurationInSeconds;
            return this;
        }
        public new ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> WithCacheIF(bool isCache, int cacheDurationInSeconds = int.MaxValue)
        {
            cacheDurationInSeconds = SetCacheTime(cacheDurationInSeconds);
            if (IsCache)
            {
                this.IsCache = true;
                this.CacheTime = cacheDurationInSeconds;
            }
            return this;
        }
        #endregion
    }
    #endregion
}
