using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NavigatManager<T>
    {
        public SqlSugarProvider Context { get; set; }
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
                     list = currentList.SelectMany(it => (it.GetType().GetProperty(navObjectName).GetValue(it) as IList).Cast<object>()).ToList();
                
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
                var memberExpression = ((_preExpressionList.Last() as LambdaExpression).Body as MemberExpression);
                var navObjectName = memberExpression.Member.Name;
                var navType = currentList[0].GetType().GetProperty(navObjectName).PropertyType.Name;
                var isList = navType.StartsWith("List`");
                List<object> list = new List<object>();
                if (isList)
                {
                    list = currentList.SelectMany(it => (it.GetType().GetProperty(navObjectName).GetValue(it) as IList).Cast<object>()).ToList();

                }
                else
                {
                    list = currentList.Select(it => (it.GetType().GetProperty(navObjectName).GetValue(it))).ToList();
                }
                ExecuteByLay(item, list, SelectR3);
                _preList = list.ToList();
            }
            _preExpressionList.Add(item);
        }

        private void ExecuteByLay(Expression expression, List<object> list, Func<ISugarQueryable<object>, List<object>> selector)
        {
            if (list == null || list.Count == 0) return;
            list = list.Where(it => it != null).ToList();
            var memberExpression = ((expression as LambdaExpression).Body as MemberExpression);

            var listItemType = list.Where(it=>it!=null).FirstOrDefault()?.GetType();
            if (listItemType.Name.StartsWith("List`")) 
            {
                listItemType = listItemType.GetGenericArguments()[0];
            }
            if (listItemType == null) return;

            var listItemEntity = this.Context.EntityMaintenance.GetEntityInfo(listItemType);
            var listPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            var navObjectName = memberExpression.Member.Name;
            var navObjectNamePropety = listItemType.GetProperty(navObjectName);
            var navObjectNameColumnInfo = listItemEntity.Columns.First(it => it.PropertyName == navObjectName);
            Check.ExceptionEasy(navObjectNameColumnInfo.Navigat == null, $"{navObjectName} not [Navigat(..)] ", $"{navObjectName} 没有导航特性 [Navigat(..)] ");



            if (navObjectNameColumnInfo.Navigat.NavigatType == NavigatType.OneToOne)
            {
                OneToOne(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else if (navObjectNameColumnInfo.Navigat.NavigatType == NavigatType.OneToMany)
            {
                OneToMany(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else if (navObjectNameColumnInfo.Navigat.NavigatType == NavigatType.ManyToOne)
            {
                OneToOne(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
            else
            {
                ManyToMany(list, selector, listItemEntity, navObjectNamePropety, navObjectNameColumnInfo);
            }
        }

        private void ManyToMany(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var bEntity = navObjectNameColumnInfo.PropertyInfo.PropertyType.GetGenericArguments()[0];
            var bEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(bEntity);
            var bPkColumn = bEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);

            var listItemPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
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
            var abids = this.Context.Queryable<object>().AS(mappingEntity.DbTableName).Where(conditionalModels).Select<SugarAbMapping>($"{aColumn.DbColumnName} as aid,{bColumn.DbColumnName} as bid").ToList();

            List<IConditionalModel> conditionalModels2 = new List<IConditionalModel>();
            conditionalModels2.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = bPkColumn.DbColumnName,
                FieldValue = String.Join(",", abids.Select(it => it.Bid).ToArray()),
                CSharpTypeName = bColumn.PropertyInfo.PropertyType.Name
            }));
            var bList = selector(this.Context.Queryable<object>().AS(bEntityInfo.DbTableName).Where(conditionalModels2));
            if (bList.HasValue())
            {
                foreach (var listItem in list)
                {

                    var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                    var ilist = instance as IList;
                    foreach (var bInfo in bList)
                    {
                        var pk = listItemPkColumn.PropertyInfo.GetValue(listItem).ObjToString();
                        var bid = bPkColumn.PropertyInfo.GetValue(bInfo).ObjToString();
                        if (abids.Any(x => x.Aid == pk&& x.Bid== bid))
                        {
                                ilist.Add(bInfo);
                        }
                    }
                    navObjectNamePropety.SetValue(listItem, instance);
                }
            }
        }

        private void OneToOne(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var navColumn = listItemEntity.Columns.FirstOrDefault(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name);
            var navType = navObjectNamePropety.PropertyType;
            var navEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(navType);
            var navPkColumn = navEntityInfo.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();

            var ids = list.Select(it => it.GetType().GetProperty(navObjectNameColumnInfo.Navigat.Name).GetValue(it)).Select(it => it == null ? "null" : it).Distinct().ToList();
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            conditionalModels.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = navPkColumn.DbColumnName,
                FieldValue = String.Join(",", ids),
                CSharpTypeName = navObjectNameColumnInfo.PropertyInfo.PropertyType.Name
            }));
            var navList = selector(this.Context.Queryable<object>().AS(navEntityInfo.DbTableName).Where(conditionalModels));
            foreach (var item in list)
            {
                var setValue = navList.FirstOrDefault(x => navPkColumn.PropertyInfo.GetValue(x).ObjToString() == navColumn.PropertyInfo.GetValue(item).ObjToString());
                navObjectNamePropety.SetValue(item, setValue);
            }
        }

        private void OneToMany(List<object> list, Func<ISugarQueryable<object>, List<object>> selector, EntityInfo listItemEntity, System.Reflection.PropertyInfo navObjectNamePropety, EntityColumnInfo navObjectNameColumnInfo)
        {
            var navEntity = navObjectNameColumnInfo.PropertyInfo.PropertyType.GetGenericArguments()[0];
            var navEntityInfo = this.Context.EntityMaintenance.GetEntityInfo(navEntity);
            var navColumn = navEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name);
            //var navType = navObjectNamePropety.PropertyType;
            var listItemPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();

            var ids = list.Select(it => it.GetType().GetProperty(listItemPkColumn.PropertyName).GetValue(it)).Select(it => it == null ? "null" : it).Distinct().ToList();
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            conditionalModels.Add((new ConditionalModel()
            {
                ConditionalType = ConditionalType.In,
                FieldName = navObjectNameColumnInfo.Navigat.Name,
                FieldValue = String.Join(",", ids),
                CSharpTypeName = listItemPkColumn.PropertyInfo.PropertyType.Name
            }));
            var navList = selector(this.Context.Queryable<object>().AS(navEntityInfo.DbTableName).Where(conditionalModels));
            if (navList.HasValue())
            {
                foreach (var item in list)
                {
                    var setValue = navList
                         .Where(x => navColumn.PropertyInfo.GetValue(x).ObjToString() == listItemPkColumn.PropertyInfo.GetValue(item).ObjToString()).ToList();
                    var instance = Activator.CreateInstance(navObjectNamePropety.PropertyType, true);
                    var ilist = instance as IList;
                    foreach (var value in setValue)
                    {
                        ilist.Add(value);
                    }
                    navObjectNamePropety.SetValue(item, instance);
                }
            }
        }
    }
}
