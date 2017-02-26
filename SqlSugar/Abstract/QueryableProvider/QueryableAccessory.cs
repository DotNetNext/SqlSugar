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
        protected void AddPars(object whereObj, SqlSugarClient context)
        {
            var sqlParsArray = context.Database.GetParameters(whereObj);
            if (_Pars == null)
                _Pars = new List<SugarParameter>();
            if (sqlParsArray != null)
                _Pars.AddRange(sqlParsArray);
        }
        protected void AddPars(List<SugarParameter> pars, SqlSugarClient context)
        {
            if (_Pars == null)
                _Pars = new List<SugarParameter>();
            if (pars != null)
                _Pars.AddRange(pars);
        }
        protected void Where<T>(Expression<Func<T, bool>> expression, ResolveExpressType type, SqlSugarClient context) where T : class, new()
        {
            ILambdaExpressions resolveExpress =InstanceFactory.GetLambdaExpressions(context.CurrentConnectionConfig);
            resolveExpress.Resolve(expression,type);
            _Pars.AddRange(resolveExpress.Parameters);
            context.SqlBuilder.LambadaQueryBuilder.WhereInfos.Add(resolveExpress.Result.ToString());
        }

        protected void Where<T>(string whereString, object whereObj, SqlSugarClient context) where T : class, new()
        {
            var SqlBuilder = context.SqlBuilder;
            var whereValue = SqlBuilder.LambadaQueryBuilder.WhereInfos;
            whereValue.Add(SqlBuilder.AppendWhereOrAnd(whereValue.Count == 0, whereString));
            this.AddPars(whereObj, context);
        }
    }
}
