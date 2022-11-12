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
        #region Tree
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

        private List<T> GetChildList(Expression<Func<T, object>> parentIdExpression, string pkName, List<T> list, object rootValue, bool isContainOneself)
        {
            var exp = (parentIdExpression as LambdaExpression).Body;
            if (exp is UnaryExpression)
            {
                exp = (exp as UnaryExpression).Operand;
            }
            var parentIdName = (exp as MemberExpression).Member.Name;
            var result = BuildChildList(list, pkName, parentIdName, rootValue, isContainOneself);
            return result;
        }

        private static List<T> BuildChildList(List<T> list, string idName, string pIdName, object rootValue, bool isContainOneself)
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

            var result = fc(rootValue.ObjToString());

            if (isContainOneself)
            {
                var root = kvpList.FirstOrDefault(x => x.Value == rootValue.ObjToString()).Key;
                if (root != null)
                {
                    result.Insert(0, root);
                }
            }

            return result;
        }
        private List<object> GetPrentIds(List<T> list, object id, EntityColumnInfo pkName, EntityColumnInfo parentName)
        {
            var currentId = id;
            List<object> result = new List<object>();
            result.Add(id);
            while (list.Any(it => pkName.PropertyInfo.GetValue(it).ObjToString()==currentId.ObjToString()))
            {
                var data = list.First(it => pkName.PropertyInfo.GetValue(it).ObjToString() == currentId.ObjToString());
                currentId = parentName.PropertyInfo.GetValue(data);
                result.Add(currentId);
            }
            return result;
        }
        private List<T> TreeAndFilterIds(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, object rootValue, object[] childIds, ref List<T> list)
        {
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var pk = GetTreeKey(entity);
            var pkColumn = entity.Columns.FirstOrDefault(z => z.PropertyName == pk);
            var newIds = new List<object>();
            string parentIdName = GetParentName(parentIdExpression);
            var parentColumn = entity.Columns.FirstOrDefault(z => z.PropertyName == parentIdName);
            foreach (var id in childIds)
            {
                newIds.AddRange(GetPrentIds(list, id, pkColumn, parentColumn));
            }
            list = list.Where(z => newIds.Any(it => it.ObjToString()==pkColumn.PropertyInfo.GetValue(z).ObjToString())).ToList();
            return GetTreeRoot(childListExpression, parentIdExpression, pk, list, rootValue);
        }

        internal List<T> GetTreeRoot(Expression<Func<T, IEnumerable<object>>> childListExpression, Expression<Func<T, object>> parentIdExpression, string pk, List<T> list, object rootValue)
        {
            var childName = ((childListExpression as LambdaExpression).Body as MemberExpression).Member.Name;
            string parentIdName = GetParentName(parentIdExpression);
            return BuildTree(list, pk, parentIdName, childName, rootValue)?.ToList() ?? default;
        }

        private static string GetParentName(Expression<Func<T, object>> parentIdExpression)
        {
            var exp = (parentIdExpression as LambdaExpression).Body;
            if (exp is UnaryExpression)
            {
                exp = (exp as UnaryExpression).Operand;
            }
            var parentIdName = (exp as MemberExpression).Member.Name;
            return parentIdName;
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
        private static string GetTreeKey(EntityInfo entity)
        {
            Check.Exception(entity.Columns.Where(it => it.IsPrimarykey || it.IsTreeKey).Count() == 0, "need IsPrimary=true Or IsTreeKey=true");
            string pk = entity.Columns.Where(it => it.IsTreeKey).FirstOrDefault()?.PropertyName;
            if (pk == null)
                pk = entity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault()?.PropertyName;
            return pk;
        }
        #endregion

        #region Count
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
            var result = Convert.ToInt32(await Context.Ado.GetScalarAsync(sql, QueryBuilder.Parameters.ToArray()));
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
        #endregion

        #region Min Max Sum Gvg
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
            return await AvgAsync<TResult>(lamResult.GetResultString());
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
            var reslut = await MaxAsync<TResult>(lamResult.GetResultString());
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
            var reslut = await SumAsync<TResult>(lamResult.GetResultString());
            QueryBuilder.SelectValue = null;
            return reslut;
        }
        #endregion

        #region Master Slave
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
        #endregion

        #region Navigate
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
                var managers = (this.QueryBuilder.Includes as List<object>);
                if (this.QueryBuilder.SelectValue.HasValue() && this.QueryBuilder.NoCheckInclude == false)
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
                    if (inValues != null && inValues.Any() && UtilMethods.GetUnderType(entitys.First().GetType().GetProperty(filedName).PropertyType) == UtilConstants.GuidType)
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
        #endregion

        #region Mapping Type
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
        #endregion

        #region  Other
        protected JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            QueryBuilder.CheckExpressionNew(joinExpression, "Join");
            QueryBuilder.JoinExpression = joinExpression;
            var express = LambdaExpression.Lambda(joinExpression).Body;
            var lastPareamter = (express as LambdaExpression).Parameters.Last();
            var expResult = this.QueryBuilder.GetExpressionValue(joinExpression, ResolveExpressType.WhereMultiple);
            this.Context.InitMappingInfo(lastPareamter.Type);
            var result = new JoinQueryInfo()
            {
                JoinIndex = QueryBuilder.JoinQueryInfos.Count,
                JoinType = joinType,
                JoinWhere = expResult.GetResultString(),
                ShortName = lastPareamter.Name,
                TableName = this.Context.EntityMaintenance.GetTableName(lastPareamter.Type)
            };
            if (this.Context.CurrentConnectionConfig?.MoreSettings?.PgSqlIsAutoToLower == false) 
            {
                result.ShortName = this.SqlBuilder.GetTranslationColumnName(result.ShortName);
            }
            if (result.JoinIndex == 0)
            {
                var firstPareamter = (express as LambdaExpression).Parameters.First();
                this.QueryBuilder.TableShortName = firstPareamter.Name;
                if (this.QueryBuilder.AsTables != null && this.QueryBuilder.AsTables.Count == 1)
                {
                    var tableinfo = this.QueryBuilder.AsTables.First();
                    if (this.QueryBuilder.TableWithString != SqlWith.Null && this.Context.CurrentConnectionConfig?.MoreSettings?.IsWithNoLockQuery == true && this.QueryBuilder.AsTables.First().Value.ObjToString().Contains(SqlWith.NoLock) == false)
                    {
                        this.QueryBuilder.AsTables[tableinfo.Key] = " (SELECT * FROM " + this.QueryBuilder.AsTables.First().Value + $" {SqlWith.NoLock} )";
                    }
                    else
                    {
                        this.QueryBuilder.AsTables[tableinfo.Key] = " (SELECT * FROM " + this.QueryBuilder.AsTables.First().Value + ")";
                    }
                    this.QueryBuilder.SelectValue = this.QueryBuilder.TableShortName + ".*";
                }
            }
            Check.Exception(result.JoinIndex > 10, ErrorMessage.GetThrowMessage("只支持12个表", "Only 12 tables are supported"));
            return result;
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
            result.SqlBuilder.QueryBuilder.IsSelectSingleFiledJson = UtilMethods.IsJsonMember(expression, this.Context);
            result.SqlBuilder.QueryBuilder.IsSelectSingleFiledArray = UtilMethods.IsArrayMember(expression, this.Context);
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
                    if (item is MemberExpression&&type==OrderByType.Asc)
                    {
                        result +=
                          QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.FieldSingle : ResolveExpressType.FieldMultiple).GetResultString() + ",";
                    }
                    else if (type == OrderByType.Desc) 
                    {
                        result +=
                          QueryBuilder.GetExpressionValue(item, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple).GetResultString() + " Desc ,";
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
                var newExp = expression as NewExpression;
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
                result = CacheSchemeMain.GetOrCreate<List<TResult>>(cacheService, this.QueryBuilder, () => { return GetData<TResult>(sqlObj); }, CacheTime, this.Context, CacheKey);
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
        protected async Task<List<TResult>> _ToListAsync<TResult>()
        {
            List<TResult> result = null;
            var sqlObj = this._ToSql();
            if (IsCache)
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                result = CacheSchemeMain.GetOrCreate<List<TResult>>(cacheService, this.QueryBuilder, () => { return GetData<TResult>(sqlObj); }, CacheTime, this.Context, CacheKey);
            }
            else
            {
                result = await GetDataAsync<TResult>(sqlObj);
            }
            RestoreMapping();
            _Mapper(result);
            await _InitNavigatAsync(result);
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
        protected async Task<List<TResult>> GetDataAsync<TResult>(KeyValuePair<string, List<SugarParameter>> sqlObj)
        {
            List<TResult> result;
            var isComplexModel = QueryBuilder.IsComplexModel(sqlObj.Key);
            var entityType = typeof(TResult);
            bool isChangeQueryableSlave = GetIsSlaveQuery();
            bool isChangeQueryableMasterSlave = GetIsMasterQuery();
            var dataReader = await this.Db.GetDataReaderAsync(sqlObj.Key, sqlObj.Value.ToArray());
            result = await GetDataAsync<TResult>(isComplexModel, entityType, dataReader);
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
                result = this.Context.Utilities.DataReaderToSelectJsonList<TResult>(dataReader);
            }
            else if (QueryBuilder.IsSelectSingleFiledArray)
            {
                result = this.Context.Utilities.DataReaderToSelectArrayList<TResult>(dataReader);
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
                result = await this.Context.Utilities.DataReaderToExpandoObjectListAsync(dataReader) as List<TResult>;
            }
            else if (entityType == UtilConstants.ObjType)
            {
                var expObj = await this.Context.Utilities.DataReaderToExpandoObjectListAsync(dataReader);
                result = expObj.Select(it => ((TResult)(object)it)).ToList();
            }
            else if (QueryBuilder.IsSelectSingleFiledJson)
            {
                result = await this.Context.Utilities.DataReaderToSelectJsonListAsync<TResult>(dataReader);
            }
            else if (QueryBuilder.IsSelectSingleFiledArray)
            {
                result =await this.Context.Utilities.DataReaderToSelectArrayListAsync<TResult>(dataReader);
            }
            else if (entityType.IsAnonymousType() || isComplexModel)
            {
                result = await this.Context.Utilities.DataReaderToListAsync<TResult>(dataReader);
            }
            else
            {
                result = await this.Bind.DataReaderToListAsync<TResult>(entityType, dataReader);
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
        protected void CopyQueryBuilder(QueryBuilder asyncQueryableBuilder)
        {
            var pars = new List<SugarParameter>();
            if (this.QueryBuilder.Parameters != null)
            {
                pars = this.QueryBuilder.Parameters.Select(it => new SugarParameter(it.ParameterName, it.Value)
                {
                    DbType = it.DbType,
                    Value = it.Value,
                    ParameterName = it.ParameterName,
                    Direction = it.Direction,
                    IsArray = it.IsArray,
                    IsJson = it.IsJson,
                    IsNullable = it.IsNullable,
                    IsRefCursor = it.IsRefCursor,
                    Size = it.Size,
                    SourceColumn = it.SourceColumn,
                    SourceColumnNullMapping = it.SourceColumnNullMapping,
                    SourceVersion = it.SourceVersion,
                    TempDate = it.TempDate,
                    TypeName = it.TypeName,
                    UdtTypeName = it.UdtTypeName,
                    _Size = it._Size
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
        public virtual KeyValuePair<string, List<SugarParameter>> _ToSql()
        {
            InitMapping();
            ToSqlBefore();
            string sql = QueryBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, QueryBuilder.Parameters);
        }
        #endregion
    }
}
