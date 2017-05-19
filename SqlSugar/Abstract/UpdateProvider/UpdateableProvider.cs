using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableProvider<T> : IUpdateable<T>
    {
        public SqlSugarClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; internal set; }
        public IAdo Ado { get { return Context.Ado; } }
        public object[] UpdateObjs { get; internal set; }

        public int ExecuteCommand()
        {
            return this.Ado.ExecuteCommand(UpdateBuilder.ToSqlString(), UpdateBuilder.Parameters.ToArray());
        }

        public IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumMethod(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression)
        {
            var expResult = UpdateBuilder.GetExpressionValue(setValueExpression, ResolveExpressType.WhereSingle);
            var resultString=expResult.GetResultString();
            LambdaExpression lambda = setValueExpression as LambdaExpression;
            var expression = lambda.Body;
            Check.Exception(!(expression is BinaryExpression), "Expression  format error");
            var leftExpression = (expression as BinaryExpression).Left;
            Check.Exception(!(leftExpression is MemberExpression), "Expression  format error");
            var leftResultString=UpdateBuilder.GetExpressionValue(leftExpression, ResolveExpressType.WhereSingle).GetString();
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(leftResultString, resultString));
            return this;
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            return new KeyValuePair<string, List<SugarParameter>>();
        }

        public IUpdateable<T> Update(T InsertObj)
        {
            return this;
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => ignoreColumns.Contains(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> UpdateRange(List<T> InsertObjs)
        {
            return this;
        }

        public IUpdateable<T> Where(bool isUpdateNull)
        {
            return this;
        }
        public IUpdateable<T> Where(Expression<Func<T, bool>> expression)
        {
            return this;
        }
        public IUpdateable<T> With(string lockString)
        {
            this.UpdateBuilder.TableWithString = lockString;
            return this;
        }

        internal void Init()
        {

        }
    }
}
