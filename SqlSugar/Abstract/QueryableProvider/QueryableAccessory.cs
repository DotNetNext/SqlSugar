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
        protected ILambdaExpressions _LambdaExpressions;
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
    }
}
