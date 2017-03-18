using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class QueryableAccessory
    {
        protected List<SugarParameter> _Pars;
        protected List<SugarParameter> BasePars
        {
            get
            {
                if (_Pars == null)
                    _Pars = new List<SugarParameter>();
                return _Pars;
            }

        }
        protected void AddPars(object whereObj, SqlSugarClient context)
        {
            var sqlParsArray = context.Database.GetParameters(whereObj);
            if (sqlParsArray != null)
                this.BasePars.AddRange(sqlParsArray);
        }
        protected void AddPars(List<SugarParameter> pars, SqlSugarClient context)
        {
            if (_Pars == null)
                _Pars = new List<SugarParameter>();
            _Pars.AddRange(pars);
        }
        protected void Where(Expression expression,SqlSugarClient context,ISqlBuilder builder)
        {
            ResolveExpressType type = ResolveExpressType.WhereSingle;
            if (builder.LambadaQueryBuilder.JoinQueryInfos.IsValuable())
            {
                type = ResolveExpressType.WhereMultiple;
            }
            ILambdaExpressions resolveExpress = context.LambdaExpressions;
            resolveExpress.Resolve(expression, type);
            BasePars.AddRange(resolveExpress.Parameters);
            builder.LambadaQueryBuilder.WhereInfos.Add(builder.AppendWhereOrAnd(builder.LambadaQueryBuilder.WhereInfos.IsNullOrEmpty(),resolveExpress.Result.GetResultString()));
            resolveExpress.Clear();
        }

        protected void Where<T>(string whereString, object whereObj, SqlSugarClient context, ISqlBuilder builder) 
        {
            var SqlBuilder = builder;
            var whereValue = SqlBuilder.LambadaQueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString));
            this.AddPars(whereObj, context);
        }

        protected void SetSelectType(SqlSugarClient context,ISqlBuilder builder)
        {
            var type = ResolveExpressType.SelectSingle;
            if (builder.LambadaQueryBuilder.JoinQueryInfos.IsValuable())
            {
                type = ResolveExpressType.SelectMultiple;
            }
            builder.LambadaQueryBuilder.ResolveType = type;
        }

    }
}
