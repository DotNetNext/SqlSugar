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
        protected ILambdaExpressions _LambdaExpressions;
        protected bool _RestoreMapping = true;
        protected int _InQueryableIndex = 100;
    }
}
