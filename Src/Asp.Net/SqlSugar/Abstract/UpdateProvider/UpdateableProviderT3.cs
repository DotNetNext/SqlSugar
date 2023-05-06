using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SqlSugar 
{
    public class UpdateableProvider<T, T2, T3> : IUpdateable<T, T2, T3> where T : class, new()
    {
        internal IUpdateable<T> updateableObj;

        public int ExecuteCommand()
        {
            return this.updateableObj.ExecuteCommand();
        }

        public Task<int> ExecuteCommandAsync()
        {
            return this.updateableObj.ExecuteCommandAsync();
        }

        public IUpdateable<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> joinExpress)
        {
            updateableObj.UpdateBuilder.Context.InitMappingInfo<T4>();
            UpdateableProvider<T, T2, T3,T4> result = new UpdateableProvider<T, T2, T3,T4>();
            result.updateableObj = updateableObj;
            var joinIno = ((QueryableProvider<T>)updateableObj.UpdateBuilder.Context.Queryable<T>()).GetJoinInfo(joinExpress, JoinType.Inner);
            result.updateableObj.UpdateBuilder.JoinInfos.Add(joinIno);
            result.updateableObj.UpdateBuilder.ShortName = joinExpress.Parameters.FirstOrDefault()?.Name;
            return result;
        }

        public IUpdateable<T, T2, T3> SetColumns(Expression<Func<T, T2, T3, T>> columns)
        {
            var exp = ((columns as LambdaExpression).Body as MemberInitExpression).Bindings;
            var items = ExpressionTool.GetMemberBindingItemList(exp);
            var UpdateBuilder = updateableObj.UpdateBuilder;
            var SqlBuilder = UpdateBuilder.Builder;
            foreach (var item in items)
            {
                var dbColumnName = updateableObj.UpdateBuilder.Context.EntityMaintenance.GetDbColumnName<T>(item.Key);
                var value = updateableObj.UpdateBuilder.GetExpressionValue(ExpressionTool.RemoveConvert(item.Value), ResolveExpressType.WhereMultiple).GetString();
                this.updateableObj.UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(dbColumnName, value));
            }
            UpdateBuilder.DbColumnInfoList = UpdateBuilder.DbColumnInfoList
                .Where(it => it.UpdateServerTime == true || it.UpdateSql.HasValue() || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            return this;
        }

        public IUpdateable<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> whereExpression)
        {

            var value = updateableObj.UpdateBuilder.GetExpressionValue(whereExpression, ResolveExpressType.WhereMultiple).GetString();
            updateableObj.UpdateBuilder.WhereValues.Add(value);
            return this;
        }
    }
}
