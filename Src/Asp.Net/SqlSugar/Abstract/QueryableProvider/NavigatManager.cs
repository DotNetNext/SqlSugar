using System;
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
        public Action<ISugarQueryable<object>> SelectR1 { get;  set; }
        public Action<ISugarQueryable<object>> SelectR2 { get; set; }
        public Action<ISugarQueryable<object>> SelectR3 { get; set; }
        public Action<ISugarQueryable<object>> SelectR4 { get; set; }
        public Action<ISugarQueryable<object>> SelectR5 { get; set; }
        public Action<ISugarQueryable<object>> SelectR6 { get; set; }
        public Action<ISugarQueryable<object>> SelectR7 { get; set; }
        public Action<ISugarQueryable<object>> SelectR8 { get; set; }
        public Expression[] Expressions { get; set; }
        public List<T> RootList { get; set; }
        //public QueryableProvider<T> Queryable { get; set; }

        //private List<Expression> _preExpressionList = new List<Expression>();
        //private Expression[] _expressions;
        //private List<T> _list;
        //private EntityInfo _entityInfo;
        public void Execute()
        {
            foreach (var item in Expressions) 
            {

            }
        }

        //private void Lay1(List<Expression> preExpressionList, Expression expression, List<object> list)
        //{
        //    if (list == null || list.Count == 0) return;
        //    var memberExpression = ((expression as LambdaExpression).Body as MemberExpression);
        //    var navName = memberExpression.Member.Name;
        //    var type = list[0].GetType();
        //    var propety = type.GetProperty(navName);
        //    var entity=this.Context.EntityMaintenance.GetEntityInfo(type);
        //    var columInfo = entity.Columns.First(it => it.PropertyName == navName);
        //    Check.ExceptionEasy(columInfo.Navigat == null, $"{navName} not [Navigat(..)] ", $"{navName} 没有导航特性 [Navigat(..)] ");
        //    var navColumn = entity.Columns.FirstOrDefault(it => it.PropertyName == columInfo.Navigat.Name);
        //    if (columInfo.Navigat.MappingType != null)
        //    {

        //    }
        //    else if (columInfo.Navigat.Name.Contains("."))
        //    {

        //    }
        //    else 
        //    {
        //        var tableType =this.Context.EntityMaintenance.GetEntityInfo(list.First().GetType());
        //        var pkColumn = tableType.Columns.Where(it=>it.IsPrimarykey).FirstOrDefault();
        //        var ids = list.Select(it => it.GetType().GetProperty(navColumn.PropertyName).GetValue(it)).Select(it=>it==null?"null":it).Distinct().ToList();
        //        List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
        //        conditionalModels.Add((new ConditionalModel() { 
        //          ConditionalType  = ConditionalType.In,
        //          FieldName= pkColumn.DbColumnName,
        //          FieldValue=String.Join(",", ids),
        //          CSharpTypeName=columInfo.PropertyInfo.PropertyType.Name
        //        }));
        //        var list2 = this.Context.Queryable<object>().AS(tableType.DbTableName).Where(conditionalModels).ToList();
        //        foreach (var item in list) 
        //        {
        //            item.
        //        }
        //    }
        //}
        //private void Lay2(List<Expression> preExpressionList, List<T> list)
        //{

        //}
        //private void Lay3(List<Expression> preExpressionList, List<T> list)
        //{

        //}
    }
}
