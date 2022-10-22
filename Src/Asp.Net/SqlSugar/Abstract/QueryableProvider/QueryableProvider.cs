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

    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
       
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
        public List<T> ToChildList(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself = true) 
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = this.ToList();
            return GetChildList(parentIdExpression, pk, list, primaryKeyValue, isContainOneself);
        }
        public async Task<List<T>> ToChildListAsync(Expression<Func<T, object>> parentIdExpression, object primaryKeyValue, bool isContainOneself=true) 
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var list = await this.ToListAsync();
            return GetChildList(parentIdExpression, pk, list, primaryKeyValue, isContainOneself);
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
                this.QueryBuilder.Offset = "true";
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
                this.QueryBuilder.Offset = "true";
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
                this.QueryBuilder.Offset = "true";
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
                this.QueryBuilder.Offset = "true";
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
            var countQueryable = this.Clone();
            if (countQueryable.QueryBuilder.Offset == "true") 
            {
                countQueryable.QueryBuilder.Offset = null;
            }
            totalNumber = countQueryable.Count();
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
            var countQueryable = this.Clone();
            if (countQueryable.QueryBuilder.Offset == "true")
            {
                countQueryable.QueryBuilder.Offset = null;
            }
            totalNumber.Value = await countQueryable.CountAsync();
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
    }

}
