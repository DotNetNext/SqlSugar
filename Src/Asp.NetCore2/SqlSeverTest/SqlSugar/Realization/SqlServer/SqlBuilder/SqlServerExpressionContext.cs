using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqlServerExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public SqlServerExpressionContext()
        {
            base.DbMehtods = new SqlServerMethod();
        }

    }
    public partial class SqlServerMethod : DefaultDbMethod, IDbMethods
    {
        public override string HasValue(MethodCallExpressionModel model)
        {
            if (model.Args[0].Type == UtilConstants.GuidType)
            {
                var parameter = model.Args[0];
                return string.Format("( {0} IS NOT NULL )", parameter.MemberName);
            }
            else
            {
                var parameter = model.Args[0];
                return string.Format("( {0}<>'' AND {0} IS NOT NULL )", parameter.MemberName);
            }
        }
        public override string DateToString(MethodCallExpressionModel model)
        {
            string parm = "20";
            if (model.Args[1].MemberValue.ToString() == SugarDateTimeFormat.Date)
            {
                parm = "120";
            }
            else if (model.Args[1].MemberValue.ToString() == SugarDateTimeFormat.Time)
            {
                parm = "108";
            }

            return string.Format(" CONVERT(varchar(100),{0}, {1}) ", model.Args[0].MemberName, parm);
        }
    }
}
