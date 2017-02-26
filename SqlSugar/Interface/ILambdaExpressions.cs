using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial interface ILambdaExpressions
    {
        SqlSugarClient Context { get; set; }
        IDbMethods DbMehtods { get; set; }
        Expression Expression { get; set; }
        int Index { get; set; }
        int ParameterIndex { get; set; }
        List<SugarParameter> Parameters { get; set; }
        ExpressionResult Result { get; set; }
        string SqlParameterKeyWord { get; }
        string GetaMppingColumnsName(string name);
        string GetAsString(string fieldName, string fieldValue);
        void Resolve(Expression expression, ResolveExpressType resolveType);
    }
}
