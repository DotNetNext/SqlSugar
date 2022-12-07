using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NavigatManager<T>
    {
        public SqlSugarProvider Context { get; set; }
        public bool IsCrossQueryWithAttr { get; set; }
        public Dictionary<string, string> CrossQueryItems { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR1 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR2 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR3 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR4 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR5 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR6 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR7 { get; set; }
        public Func<ISugarQueryable<object>, List<object>> SelectR8 { get; set; }
        public Expression[] Expressions { get; set; }
        public List<T> RootList { get; set; }
        //public QueryableProvider<T> Queryable { get; set; }

        private List<Expression> _preExpressionList = new List<Expression>();
        private List<object> _preList = new List<object>();
        private List<Expression> _ListCallFunc;
        //private Expression[] _expressions;
        //private List<T> _list;
        //private EntityInfo _entityInfo;
        public void Execute()
        {
            var i = 1;
            foreach (var item in Expressions)
            {
                ExecuteByLay(i, item);
                i++;
            }
        }

        private void ExecuteByLay(int i, Expression item)
        {
            _ListCallFunc = GetWhereExpression(ref item);
            if (i == 1)
            {
                ExecuteByLay(item, RootList.Select(it => it as object).ToList(), SelectR1);
            }
            else if (i == 2)
            {
                var currentList = RootList;
                if (RootList == null || currentList.Count == 0) return;
                var memberExpression = ((_preExpressionList.Last() as LambdaExpression).Body as MemberExpression);
                var navObjectName = memberExpression.Member.Name;
                var navType = currentList[0].GetType().GetProperty(navObjectName).PropertyType.Name;    
                var isList = navType.StartsWith("List`");
                List<object> list = new List<object>();
                if (isList)
                {
                     list = currentList.SelectMany(it => (it.GetType().GetProperty(navObjectName).GetValue(it) as IList)?.Cast<object>()??new List<object> { }).ToList();
                
                }
                else 
                {
                    list = currentList.Select(it => (it.GetType().GetProperty(navObjectName).GetValue(it))).ToList();
                }
                ExecuteByLay(item, list, SelectR2);
                _preList = list;
            }
            else if (i == 3)
            {
                var currentList = _preList.Where(it => it != null).ToList();
                if (RootList == null || currentList.Count == 0) return;
                List<object> list = ExecuteByLay(currentList);
                ExecuteByLay(item, list, SelectR3);
                _preList = list.ToList();
            }
            else if (i == 4)
            {
                var currentList = _preList.Where(it => it != null).ToList();
                if (RootList == null || currentList.Count == 0) return;
                List<object> list = ExecuteByLay(currentList);
                ExecuteByLay(item, list, SelectR4);
                _preList = list.ToList();
            }
            else if (i == 5)
            {
                var currentList = _preList.Where(it => it != null).ToList();
                if (RootList == null || currentList.Count == 0) return;
                List<object> list = ExecuteByLay(currentList);
                ExecuteByLay(item, list, SelectR5);
                _preList = list.ToList();
            }
            else if (i == 6)
            {
                var currentList = _preList.Where(it => it != null).ToList();
                if (RootList == null || currentList.Count == 0) return;
                List<object> list = ExecuteByLay(currentList);
                ExecuteByLay(item, list, SelectR6);
                _preList = list.ToList();
            }
            else if (i == 7)
            {
                var currentList = _preList.Where(it => it != null).ToList();
                if (RootList == null || currentList.Count == 0) return;
                List<object> list = ExecuteByLay(currentList);
                ExecuteByLay(item, list, SelectR7);
                _preList = list.ToList();
            }
            _preExpressionList.Add(item);
            _ListCallFunc = new List<Expression>();
        }

        private List<object> ExecuteByLay(List<object> currentList)
        {
            var memberExpression = ((_preExpressionList.Last() as LambdaExpression).Body as MemberExpression);
            var navObjectName = memberExpression.Member.Name;
            var navType = currentList[0].GetType().GetProperty(navObjectName).PropertyType.Name;
            var isList = navType.StartsWith("List`");
            List<object> list = new List<object>();
            if (isList)
            {
                list = currentList.Where(it=> it.GetType().GetProperty(navObjectName).GetValue(it)!=null).SelectMany(it => (it.GetType().GetProperty(navObjectName).GetValue(it) as IList).Cast<object>()).ToList();

            }
            else
            {
                list = currentList.Where(it=>it.GetType().GetProperty(navObjectName).GetValue(it)!=null).Select(it => (it.GetType().GetProperty(navObjectName).GetValue(it))).ToList();
            }

            return list;
        }

        private void ExecuteByLay(Expression expression, List<object> list, Func<ISugarQueryable<object>, List<object>> selector)
        {
            if (list == null || list.Count == 0) return;
            list = list.Where(it => it != null).ToList();
            var memberExpression = ((expression as LambdaExpression).Body as MemberExpression);

            var listItemType = list.Where(it=>it!=null).FirstOrDefault()?.GetType();
            if (listItemType == null) 
            {
                return;
            }
            if (listItemType.Name.StartsWith("List`")) 
            {
                listItemType = listItemType.GetGenericArguments()[0];
            }
            //if (listItemType == null) return;

            var listItemEntity = this.Context.EntityMaintenance.GetEntityInfo(listItemType);
            var listPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            var navObjectName = memberExpression.Member.Name;
            var navObjectNamePropety = listItemType.GetProperty(navObjectName);
            var navObjectNameColumnInfo = listItemEntity.Columns.First(it => it.PropertyName == navObjectName);
            Check.ExceptionEasy(navObjectNameColumnInfo.Navigat == null, $"{navObjectName} not [Navigat(..)] ", $"{navObjectName} 没有导航特性 [Navigat(..)] ");



            if (navObjectNameColumnInfo.Navigat.NavigatType == NavigateType.OneToOne)
            {
                OneToOne(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else if (navObjectNameColumnInfo.Navigat.NavigatType == NavigateType.OneToMany)
            {
                OneToMany(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else if (navObjectNameColumnInfo.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                OneToOne(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else if (navObjectNameColumnInfo.Navigat.NavigatType == NavigateType.Dynamic)
            {
                Dynamic(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo,expression);
            }
            else
            {
                ManyToMany(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
        }

        private List<Expression> GetWhereExpression(ref Expression expression)
        {
            List<Expression> expressions = new List<Expression>();
            var isCall = (expression as LambdaExpression).Body is MethodCallExpression;
            if (isCall) 
            {
               var newexp = (expression as LambdaExpression).Body;
                while (newexp is MethodCallExpression) 
                {
                    expressions.Add(newexp);
                    newexp= (newexp as MethodCallExpression).Arguments[0];  
                }
                expression =LambdaExpression.Lambda(newexp);
            }
            return expressions;
        }

        private void ManyToMany(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var bEntity = navObjectNameColumnInfo.PropertyInfo.PropertyType.GetGenericArguments()[0];
            var bEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(bEntity);
            var bPkColumn = bEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            Check.ExceptionEasy(bPkColumn==null, $"{bEntityInfo.EntityName} need primary key", $"{bEntityInfo.EntityName} 实体需要配置主键");
            var bDb = this.Context;
            bDb = GetCrossDatabase(bDb,bEntity);
            bDb.InitMappingInfo(bEntity);
            var listItemPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            Check.ExceptionEasy(listItemPkColumn == null, $"{listItemEntity.EntityName} need primary key", $"{listItemEntity.EntityName} 实体需要配置主键");
            var ids = list.Select(it => it.GetType().GetProperty(listItemPkColumn.PropertyName).GetValue(it)).Select(it => it == null ? "null" : it).Distinct().ToList();
            var mappingEntity = this.Context.EntityMaintenance.GetEntityInfo(navObjectNameColumnInfo.Navigat.MappingType);
            var aColumn = mappingEntity.Columns.First(it => it.PropertyName == navObjectNameColumnInfo.Navigat.MappingAId);
            var bColumn = mappingEntity.Columns.First(it => it.PropertyName == navObjectNameColumnInfo.Navigat.MappingBId);
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            conditionalModels.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = aColumn.DbColumnName,
                FieldValue = String.Join(",", ids),
                CSharpTypeName = aColumn.PropertyInfo.PropertyType.Name
            }));
            var abDb = this.Context;
            abDb = GetCrossDatabase(abDb, mappingEntity.Type);
            var queryable = abDb.Queryable<object>();
            var abids = queryable.AS(mappingEntity.DbTableName).Filter(mappingEntity.Type).Where(conditionalModels).Select<SugarAbMapping>($"{queryable.SqlBuilder.GetTranslationColumnName(aColumn.DbColumnName)} as aid,{queryable.SqlBuilder.GetTranslationColumnName(bColumn.DbColumnName)} as bid").ToList();

            List<IConditionalModel> conditionalModels2 = new List<IConditionalModel>();
            conditionalModels2.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = bPkColumn.DbColumnName,
                FieldValue = String.Join(",", abids.Select(it => it.Bid).ToArray()),
                CSharpTypeName = bColumn.PropertyInfo.PropertyType.Name
            }));
            var sql = GetWhereSql();
            var bList = selector(bDb.Queryable<object>().AS(bEntityInfo.DbTableName).Filter(bEntityInfo.Type).AddParameters(sql.Parameters).Where(conditionalModels2).WhereIF(sql.WhereString.HasValue(),sql.WhereString).Select(sql.SelectString).OrderByIF(sql.OrderByString.HasValue(),sql.OrderByString));  
            if (bList.HasValue())
            {
                foreach (var listItem in list)
                {
                    if (navObjectNamePropety.GetValue(listItem) == null)
                    {
                        var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                        var ilist = instance as IList;
                        foreach (var bInfo in bList)
                        {
                            var pk = listItemPkColumn.PropertyInfo.GetValue(listItem).ObjToString();
                            var bid = bPkColumn.PropertyInfo.GetValue(bInfo).ObjToString();
                            if (abids.Any(x => x.Aid == pk && x.Bid == bid))
                            {
                                ilist.Add(bInfo);
                            }
                        }
                        if (sql.MappingExpressions.HasValue())
                        {
                            MappingFieldsHelper<T> helper = new MappingFieldsHelper<T>();
                            helper.Context = this.Context;
                            helper.NavEntity = bEntityInfo;
                            helper.RootEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                            helper.SetChildList(navObjectNameColumnInfo, listItem, ilist.Cast<object>().ToList(), sql.MappingExpressions);
                        }
                        else
                        {
                            if (sql.Skip != null || sql.Take != null)
                            {
                                var instanceCast = (instance as IList);
                                var newinstance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true) as IList;
                                SkipTakeIList(sql, instanceCast, newinstance);
                                navObjectNamePropety.SetValue(listItem, newinstance);
                            }
                            else
                            {
                                navObjectNamePropety.SetValue(listItem, instance);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var listItem in list)
                {
                    if (navObjectNamePropety.GetValue(listItem) == null)
                    {
                        var newinstance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true) as IList;
                        navObjectNamePropety.SetValue(listItem,newinstance);
                    }
                }
            }
        }

        private static void SkipTakeIList(SqlInfo sql, IList instanceCast, IList newinstance)
        {
            var intArray = Enumerable.Range(0, instanceCast.Count);
            if (sql.Skip != null)
            {
                intArray = intArray
                    .Skip(sql.Skip.Value);
            }
            if (sql.Take != null)
            {
                intArray = intArray
                    .Take(sql.Take.Value);
            }
            foreach (var i in intArray)
            {
                newinstance.Add(instanceCast[i]);
            }
        }

        private void OneToOne(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var navColumn = listItemEntity.Columns.FirstOrDefault(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name);
            Check.ExceptionEasy(navColumn == null, "OneToOne navigation configuration error", $"OneToOne导航配置错误： 实体{ listItemEntity.EntityName } 不存在{navObjectNameColumnInfo.Navigat.Name}");
            var navType = navObjectNamePropety.PropertyType;
            var db = this.Context;
            db = GetCrossDatabase(db,navType);
            var navEntityInfo = db.EntityMaintenance.GetEntityInfo(navType);
            db.InitMappingInfo(navEntityInfo.Type);
            var navPkColumn = navEntityInfo.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            Check.ExceptionEasy(navPkColumn==null&& navObjectNameColumnInfo.Navigat.Name2==null, navEntityInfo.EntityName+ "need primarykey", navEntityInfo.EntityName + " 需要主键");
            if (navObjectNameColumnInfo.Navigat.Name2.HasValue()) 
            {
                navPkColumn = navEntityInfo.Columns.Where(it => it.PropertyName== navObjectNameColumnInfo.Navigat.Name2).FirstOrDefault();
            }
            if (navPkColumn == null && navType.FullName.IsCollectionsList()) 
            {
                Check.ExceptionEasy($"{navObjectNamePropety.Name} type error ", $"一对一不能是List对象 {navObjectNamePropety.Name} ");
            }
            var ids = list.Select(it => it.GetType().GetProperty(navObjectNameColumnInfo.Navigat.Name).GetValue(it)).Select(it => it == null ? "null" : it).Distinct().ToList();
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            conditionalModels.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = navPkColumn.DbColumnName,
                FieldValue = String.Join(",", ids),
                CSharpTypeName = navObjectNameColumnInfo.PropertyInfo.PropertyType.Name
            }));
            if (list.Any()&&navObjectNamePropety.GetValue(list.First()) == null)
            {
                var sqlObj = GetWhereSql(navObjectNameColumnInfo.Navigat.Name);
                var navList = selector(db.Queryable<object>().Filter(navEntityInfo.Type).AS(navEntityInfo.DbTableName)
                    .WhereIF(navObjectNameColumnInfo.Navigat.WhereSql.HasValue(), navObjectNameColumnInfo.Navigat.WhereSql)
                    .WhereIF(sqlObj.WhereString.HasValue(),sqlObj.WhereString)
                    .AddParameters(sqlObj.Parameters).Where(conditionalModels)
                    .Select(sqlObj.SelectString));
                var groupQuery = (from l in list
                                 join n in navList
                                      on navColumn.PropertyInfo.GetValue(l).ObjToString() 
                                      equals navPkColumn.PropertyInfo.GetValue(n).ObjToString()  
                                 select new
                                 {
                                     l,
                                     n
                                 }).ToList();
                foreach (var item in groupQuery)
                {

                    // var setValue = navList.FirstOrDefault(x => navPkColumn.PropertyInfo.GetValue(x).ObjToString() == navColumn.PropertyInfo.GetValue(item).ObjToString());

                    if (navObjectNamePropety.GetValue(item.l) == null)
                    {
                        navObjectNamePropety.SetValue(item.l, item.n);
                    }
                    else
                    {
                        //The reserved
                    }

                }
            }
        }

        private void OneToMany(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var navEntity = navObjectNameColumnInfo.PropertyInfo.PropertyType.GetGenericArguments()[0];
            var navEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(navEntity);
            var childDb = this.Context;
            childDb = GetCrossDatabase(childDb, navEntityInfo.Type);
            childDb.InitMappingInfo(navEntityInfo.Type);
            var navColumn = navEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name);
            Check.ExceptionEasy(navColumn == null, $"{navEntityInfo.EntityName} not found {navObjectNameColumnInfo.Navigat.Name} ", $"实体 {navEntityInfo.EntityName} 未找到导航配置列 {navObjectNameColumnInfo.Navigat.Name} ");
            //var navType = navObjectNamePropety.PropertyType;
            var listItemPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            Check.ExceptionEasy(listItemPkColumn == null&& navObjectNameColumnInfo.Navigat.Name2==null, listItemEntity.EntityName + " not primary key", listItemEntity.EntityName + "没有主键");
            if (navObjectNameColumnInfo.Navigat.Name2.HasValue()) 
            {
                listItemPkColumn = listItemEntity.Columns.Where(it => it.PropertyName== navObjectNameColumnInfo.Navigat.Name2).FirstOrDefault();
                Check.ExceptionEasy(listItemPkColumn == null, $"{navObjectNameColumnInfo.PropertyName} Navigate is error ", $"{navObjectNameColumnInfo.PropertyName}导航配置错误,可能顺序反了。");
            }
            var ids = list.Select(it => it.GetType().GetProperty(listItemPkColumn.PropertyName).GetValue(it)).Select(it => it == null ? "null" : it).Distinct().ToList();
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            conditionalModels.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = navColumn.DbColumnName,
                FieldValue = String.Join(",", ids),
                CSharpTypeName = listItemPkColumn.PropertyInfo.PropertyType.Name
            }));
            var sqlObj = GetWhereSql(navObjectNameColumnInfo.Navigat.Name);
      
            if (list.Any() && navObjectNamePropety.GetValue(list.First()) == null)
            {
                var navList = selector(childDb.Queryable<object>(sqlObj.TableShortName).AS(navEntityInfo.DbTableName).Filter(navEntityInfo.Type).AddParameters(sqlObj.Parameters).Where(conditionalModels).WhereIF(sqlObj.WhereString.HasValue(), sqlObj.WhereString).Select(sqlObj.SelectString).OrderByIF(sqlObj.OrderByString.HasValue(), sqlObj.OrderByString));
                if (navList.HasValue())
                {
                    //var setValue = navList
                    //  .Where(x => navColumn.PropertyInfo.GetValue(x).ObjToString() == listItemPkColumn.PropertyInfo.GetValue(item).ObjToString()).ToList();
                    var groupQuery = (from l in list.Distinct()
                                      join n in navList
                                           on listItemPkColumn.PropertyInfo.GetValue(l).ObjToString()
                                           equals navColumn.PropertyInfo.GetValue(n).ObjToString()
                                      select new
                                      {
                                          l,
                                          n 
                                      }).GroupBy(it => it.l).ToList();
                    foreach (var item in groupQuery)
                    {
                        var itemSelectList=item.Select(it => it.n);
                        if (sqlObj.Skip != null)
                        {
                            itemSelectList = itemSelectList
                                .Skip(sqlObj.Skip.Value);
                        }
                        if (sqlObj.Take != null) 
                        {
                            itemSelectList = itemSelectList
                                .Take(sqlObj.Take.Value);
                        }
                        if (sqlObj.MappingExpressions.HasValue())
                        {
                            MappingFieldsHelper<T> helper = new MappingFieldsHelper<T>();
                            helper.NavEntity = navEntityInfo;
                            helper.Context = this.Context;
                            helper.RootEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                            helper.SetChildList(navObjectNameColumnInfo, item.Key, itemSelectList.ToList(), sqlObj.MappingExpressions);
                        }
                        else
                        {

                            var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                            var ilist = instance as IList;
                            foreach (var value in itemSelectList.ToList())
                            {
                                ilist.Add(value);
                            }
                            navObjectNamePropety.SetValue(item.Key, instance);
                        }
                    }
                    foreach (var item in list)
                    {
                        if (navObjectNamePropety.GetValue(item) == null)
                        {
                            var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                            navObjectNamePropety.SetValue(item, instance);
                        }
                    }
                }
                else 
                {
                    //No navigation data  set new List()
                    foreach (var item in list)
                    {
                         var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                         navObjectNamePropety.SetValue(item, instance);
                    }
                }
            }
        }

        private void Dynamic(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo,Expression expression)
        {
            var args = navObjectNameColumnInfo.PropertyInfo.PropertyType.GetGenericArguments();
            if (args.Length == 0) 
            {
                DynamicOneToOne(list,selector,listItemEntity, navObjectNamePropety, navObjectNameColumnInfo,expression);
                return;
            }
            var navEntity = args[0];
            this.Context.InitMappingInfo(navEntity);
            var navEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(navEntity);
            var sqlObj = GetWhereSql(navObjectNameColumnInfo.Navigat.Name);
            Check.ExceptionEasy(sqlObj.MappingExpressions.IsNullOrEmpty(), $"{expression} error,dynamic need MappingField ,Demo: Includes(it => it.Books.MappingField(z=>z.studenId,()=>it.StudentId).ToList())", $"{expression} 解析出错,自定义映射需要 MappingField ,例子: Includes(it => it.Books.MappingField(z=>z.studenId,()=>it.StudentId).ToList())");
            if (list.Any() && navObjectNamePropety.GetValue(list.First()) == null)
            {
                MappingFieldsHelper<T> helper = new MappingFieldsHelper<T>();
                helper.Context = this.Context;
                helper.NavEntity = navEntityInfo;
                helper.RootEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                var whereSql = helper.GetMppingSql(list, sqlObj.MappingExpressions);
                var navList = selector(this.Context.Queryable<object>().AS(navEntityInfo.DbTableName).AddParameters(sqlObj.Parameters).Where(whereSql,true).WhereIF(sqlObj.WhereString.HasValue(), sqlObj.WhereString).Select(sqlObj.SelectString).OrderByIF(sqlObj.OrderByString.HasValue(), sqlObj.OrderByString));
                if (navList.HasValue())
                {
                    foreach (var item in list)
                    {
                        helper.SetChildList(navObjectNameColumnInfo, item, navList, sqlObj.MappingExpressions);
                    }
                }
            }

        }

        private void DynamicOneToOne(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo,Expression expression)
        {
            var navEntity = navObjectNameColumnInfo.PropertyInfo.PropertyType;
            var navEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(navEntity);
            this.Context.InitMappingInfo(navEntity);
            var sqlObj = GetWhereSql(navObjectNameColumnInfo.Navigat.Name);
            Check.ExceptionEasy(sqlObj.MappingExpressions.IsNullOrEmpty(), $"{expression} error，dynamic need MappingField ,Demo: Includes(it => it.Books.MappingField(z=>z.studenId,()=>it.StudentId).ToList())", $"{expression}解析出错， 自定义映射需要 MappingField ,例子: Includes(it => it.Books.MappingField(z=>z.studenId,()=>it.StudentId).ToList())");
            if (list.Any() && navObjectNamePropety.GetValue(list.First()) == null)
            {
                MappingFieldsHelper<T> helper = new MappingFieldsHelper<T>();
                helper.Context = this.Context;
                helper.NavEntity = navEntityInfo;
                helper.RootEntity = this.Context.EntityMaintenance.GetEntityInfo<T>();
                var whereSql = helper.GetMppingSql(list, sqlObj.MappingExpressions);
                var navList = selector(this.Context.Queryable<object>().AS(navEntityInfo.DbTableName).AddParameters(sqlObj.Parameters).Where(whereSql, true).WhereIF(sqlObj.WhereString.HasValue(), sqlObj.WhereString).Select(sqlObj.SelectString).OrderByIF(sqlObj.OrderByString.HasValue(), sqlObj.OrderByString));
                if (navList.HasValue())
                {
                    foreach (var item in list)
                    {
                        helper.SetChildItem(navObjectNameColumnInfo, item, navList, sqlObj.MappingExpressions);
                    }
                }
            }
        }

        private SqlInfo GetWhereSql(string properyName=null)
        {
            if (_ListCallFunc == null|| _ListCallFunc.Count==0) return new SqlInfo();
            List<string> where = new List<string>();
            List<string> oredrBy = new List<string>();
            _ListCallFunc.Reverse();
            SqlInfo result = new SqlInfo();
            result.Parameters = new List<SugarParameter>();
            var isList = false;
            int parameterIndex = 0;
            foreach (var item in _ListCallFunc)
            {
                var method = item as MethodCallExpression;
                var queryable = this.Context.Queryable<object>();
                if (method.Method.Name == "Where")
                {
                    if (method.Arguments[1].Type == typeof(List<IConditionalModel>))
                    {
                        //var x=method.Arguments[1];
                        var conditionals = ExpressionTool.GetExpressionValue(method.Arguments[1])  as List<IConditionalModel>;
                        if (conditionals.Count > 0)
                        {
                            var whereObj = queryable.QueryBuilder.Builder.ConditionalModelToSql(conditionals);
                            where.Add(whereObj.Key);
                            if(whereObj.Value!=null)
                                result.Parameters.AddRange(whereObj.Value);
                        }
                    }
                    else
                    {
                        queryable.QueryBuilder.LambdaExpressions.ParameterIndex = parameterIndex;
                        CheckHasRootShortName(method.Arguments[0], method.Arguments[1]);
                        var exp = method.Arguments[1];
                        where.Add(" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
                        SetTableShortName(result, queryable);
                        parameterIndex=queryable.QueryBuilder.LambdaExpressions.ParameterIndex ;
                    }
                }
                else if (method.Method.Name == "WhereIF")
                {
                    var isOk = LambdaExpression.Lambda(method.Arguments[1]).Compile().DynamicInvoke();
                    if (isOk.ObjToBool())
                    {
                        queryable.QueryBuilder.LambdaExpressions.ParameterIndex = parameterIndex;
                        var exp = method.Arguments[2];
                        CheckHasRootShortName(method.Arguments[1], method.Arguments[2]);
                        where.Add(" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
                        SetTableShortName(result, queryable);
                        parameterIndex = queryable.QueryBuilder.LambdaExpressions.ParameterIndex;
                    }
                }
                else if (method.Method.Name == "OrderBy")
                {
                    var exp = method.Arguments[1];
                    oredrBy.Add(" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
                    SetTableShortName(result, queryable);
                }
                else if (method.Method.Name == "MappingField")
                {
                    if (result.MappingExpressions == null)
                        result.MappingExpressions = new List<MappingFieldsExpression>();
                    result.MappingExpressions.Add(new MappingFieldsExpression()
                    {
                        LeftColumnExpression = method.Arguments[1],
                        RightColumnExpression = method.Arguments[2]
                    });
                }
                else if (method.Method.Name == "Select")
                {
                    Select(properyName, result, method, queryable);
                }
                else if (method.Method.Name == "OrderByDescending")
                {
                    var exp = method.Arguments[1];
                    oredrBy.Add(" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString() + " DESC");
                }
                else if (method.Method.Name == "Skip")
                {
                    var exp = method.Arguments[1];
                    if (exp is BinaryExpression)
                    {
                        result.Skip = (int)ExpressionTool.DynamicInvoke(exp);
                    }
                    else
                    {
                        result.Skip = (int)ExpressionTool.GetExpressionValue(exp);
                    }
                }
                else if (method.Method.Name == "Take")
                {
                    var exp = method.Arguments[1];
                    if (exp is BinaryExpression)
                    {
                        result.Take = (int)ExpressionTool.DynamicInvoke(exp);
                    }
                    else
                    {
                        result.Take = (int)ExpressionTool.GetExpressionValue(exp);
                    }
                }
                else if (method.Method.Name == "ToList")
                {
                    if (method.Arguments.Count > 1) 
                    {
                        Select(properyName, result, method, queryable);
                    }
                    isList = true;
                }
                else
                {
                    Check.ExceptionEasy($"no support {item}", $"不支持表达式{item} 不支持方法{method.Method.Name}");
                }
                if(queryable.QueryBuilder.Parameters!=null)
                 result.Parameters.AddRange(queryable.QueryBuilder.Parameters);
            }
            if (where.Any()) 
            {
                Check.ExceptionEasy(isList == false, $"{_ListCallFunc.First()} need is ToList()", $"{_ListCallFunc.First()} 需要ToList");
                result.WhereString=  String.Join(" AND ", where);
            }
            if (oredrBy.Any())
            {
                Check.ExceptionEasy(isList == false, $"{_ListCallFunc.First()} need is ToList()", $"{_ListCallFunc.First()} 需要ToList");
                result.OrderByString = String.Join(" , ", oredrBy);
            }
            if (result.SelectString.HasValue())
            {
                Check.ExceptionEasy(isList == false, $"{_ListCallFunc.First()} need is ToList()", $"{_ListCallFunc.First()} 需要ToList");
                result.OrderByString = String.Join(" , ", oredrBy);
            }
            return result;
        }

        private void Select(string properyName, SqlInfo result, MethodCallExpression method, ISugarQueryable<object> queryable)
        {
            var exp = method.Arguments[1];
            var newExp = (exp as LambdaExpression).Body;
            var types = exp.Type.GetGenericArguments();
            if (types != null && types.Length > 0)
            {
                var type = types[0];
                var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type);
                this.Context.InitMappingInfo(type);
                Check.ExceptionEasy(newExp.Type != entityInfo.Type, $" new {newExp.Type.Name}is error ,use Select(it=>new {entityInfo.Type.Name})", $"new {newExp.Type.Name}是错误的，请使用Select(it=>new {entityInfo.Type.Name})");
                if (entityInfo.Columns.Count(x => x.Navigat != null) == 0)
                {
                    result.SelectString = (" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.SelectSingle).GetString());
                }
                else
                {
                    var pkInfo = entityInfo.Columns.FirstOrDefault(x => x.IsPrimarykey);
                    result.SelectString = (" " + queryable.QueryBuilder.GetExpressionValue(exp, ResolveExpressType.SelectSingle).GetString());
                    if (pkInfo != null)
                    {
                        var pkName = pkInfo.DbColumnName;
                        AppColumns(result, queryable, pkName);
                    }
                    foreach (var nav in entityInfo.Columns.Where(x => x.Navigat != null && x.Navigat.NavigatType == NavigateType.OneToOne))
                    {
                        var navColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
                        if (navColumn != null)
                        {
                            AppColumns(result, queryable, navColumn.DbColumnName);
                        }
                    }
                    foreach (var nav in entityInfo.Columns.Where(x => x.Navigat != null && x.Navigat.NavigatType == NavigateType.OneToMany && x.Navigat.Name2 != null))
                    {
                        var navColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
                        if (navColumn != null)
                        {
                            AppColumns(result, queryable, navColumn.DbColumnName);
                        }
                    }
                    result.SelectString = result.SelectString.TrimStart(',');
                    if (result.SelectString == "")
                    {
                        result.SelectString = null;
                    }
                }
                if (properyName != null)
                {
                    var fkColumnsInfo = entityInfo.Columns.FirstOrDefault(x => x.PropertyName == properyName);
                    if (fkColumnsInfo != null)
                    {
                        var fkName = fkColumnsInfo.DbColumnName;
                        AppColumns(result, queryable, fkName);
                    }
                }
            }
        }

        private static void SetTableShortName(SqlInfo result, ISugarQueryable<object> queryable)
        {
            if (queryable.QueryBuilder.TableShortName.HasValue()&& result.TableShortName.IsNullOrEmpty())
            {
                result.TableShortName = queryable.QueryBuilder.TableShortName;
            }
        }


        private SqlSugarProvider GetCrossDatabase(SqlSugarProvider db, Type type)
        {
            if (IsCrossQueryWithAttr == false && this.CrossQueryItems == null)
            {
                return db;
            }
            else if (IsCrossQueryWithAttr)
            {
                var tenant = type.GetCustomAttribute<TenantAttribute>();
                if (tenant != null)
                {
                    return db.Root.GetConnection(tenant.configId);
                }
                else
                {
                    return db;
                }
            } 
            else if (this.CrossQueryItems!=null&& this.CrossQueryItems.Count>0&&this.CrossQueryItems.ContainsKey(type.FullName)) 
            {
                var result= db.Root.GetConnection(this.CrossQueryItems[type.FullName]);
                return result;
            }
            else
            {
                return db;
            }
        }

        private static void AppColumns(SqlInfo result, ISugarQueryable<object> queryable, string columnName)
        {
            var selectPkName = queryable.SqlBuilder.GetTranslationColumnName(columnName);
            if (result.SelectString!=null && !result.SelectString.ToLower().Contains(selectPkName.ToLower()))
            {
                result.SelectString = result.SelectString + "," + (selectPkName +" AS "+ selectPkName);
            }
        }
        public void CheckHasRootShortName(Expression rootExpression, Expression childExpression)
        {
            var rootShortName = GetShortName(rootExpression);
            if (rootShortName.HasValue()&& childExpression.ToString().Contains($" {rootShortName}."))
            {
                Check.ExceptionEasy($".Where({childExpression}) no support {rootShortName}.Field, Use .MappingField",$".Where({childExpression})禁止出{rootShortName}.字段 , 你可以使用.MappingField(z=>z.字段,()=>{rootShortName}.字段) 与主表字段进行过滤");
            }
            else if (rootShortName.HasValue() && childExpression.ToString().Contains($"({rootShortName}."))
            {
                Check.ExceptionEasy($".Where({childExpression}) no support {rootShortName}.Field, Use .MappingField", $".Where({childExpression})禁止出{rootShortName}.字段 , 你可以使用.MappingField(z=>z.字段,()=>{rootShortName}.字段) 与主表字段进行过滤");
            }
        }

        private static string GetShortName(Expression expression1)
        {
            string shortName = null;
            if (expression1 is MemberExpression)
            {
                var shortNameExpression = (expression1 as MemberExpression).Expression;
                if (shortNameExpression != null && shortNameExpression.Type == typeof(T))
                {
                    if (shortNameExpression is ParameterExpression)
                    {
                        shortName = (shortNameExpression as ParameterExpression).Name;
                    }
                }
            }
            return shortName;
        }

    }
}
