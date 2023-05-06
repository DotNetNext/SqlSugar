using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SqlSugar 
{
    public class UpdateableProvider<T, T2, T3, T4> : IUpdateable<T, T2, T3, T4> where T : class, new()
    {
        public IUpdateable<T> updateableObj { get; set; }
        public int ExecuteCommand()
        {
            return this.updateableObj.ExecuteCommand();
        }

        public Task<int> ExecuteCommandAsync()
        {
            return this.updateableObj.ExecuteCommandAsync();
        }

        public IUpdateable<T, T2, T3, T4> SetColumns(Expression<Func<T, T2, T3, T4, T>> columns)
        {
            updateableObj.UpdateBuilder.Context.InitMappingInfo<T4>();
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

        public IUpdateable<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> whereExpression)
        {

            var value = updateableObj.UpdateBuilder.GetExpressionValue(whereExpression, ResolveExpressType.WhereMultiple).GetString();
            updateableObj.UpdateBuilder.WhereValues.Add(value);
            return this;
        }
    }
}
