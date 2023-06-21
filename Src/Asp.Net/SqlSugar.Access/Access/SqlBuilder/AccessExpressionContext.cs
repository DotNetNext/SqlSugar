using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public partial class AccessExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public AccessExpressionContext()
        {
            base.DbMehtods = new AccessMethod();
        }

    }
    public partial class AccessMethod : DefaultDbMethod, IDbMethods
    {
        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            if (parameter.MemberName?.ToString()?.Contains(":")==true
                && parameter.MemberName?.ToString()?.Contains("-") == true
                 &&parameter.MemberName?.ToString()?.StartsWith("'") == true) 
            {
                parameter.MemberName ="'"+ parameter.MemberName.ToString()
                    .ToString().TrimEnd('\'').TrimStart('\'').ObjToDate().ToString("yyyy-MM-dd HH:mm:ss")+"'";
            }
            return string.Format(" CDate({0}) ", parameter.MemberName);
        }
        public override string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CDate({0}) ", parameter.MemberName);
        }
        public override string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CInt({0}) ", parameter.MemberName);
        }
        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CVar({0}) ", parameter.MemberName);
        }
        public override string ToDateShort(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" Format$({0},\"Long Date\") ", parameter.MemberName);
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var type = "";
            if (model.Args[1].MemberValue.ObjToString() == "Day") 
            {
                type = "d";
            }
            if (model.Args[1].MemberValue.ObjToString() == "Month")
            {
                type = "m";
            }
            if (model.Args[1].MemberValue.ObjToString() == "Year")
            {
                type = "yyyy";
            }
            if (model.Args[1].MemberValue.ObjToString() == DateType.Minute.ToString())
            {
                type = "M";
            }
            if (model.Args[1].MemberValue.ObjToString() == DateType.Second.ToString())
            {
                type = "s";
            }
            return "DATEPART(\""+ type + "\", date())";
        }
        public override string GetRandom()
        {
            return " rnd() ";
        }
        public override string GetDate()
        {
            return " NOW()";
        }
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
    }
}
