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
    public partial class OracleExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarClient Context { get; set; }
        public OracleExpressionContext()
        {
            base.DbMehtods = new OracleMethod();
        }
        public override string SqlParameterKeyWord
        {
            get
            {
                return ":";
            }
        }
        public override string SqlTranslationLeft { get { return "\""; } }
        public override string SqlTranslationRight { get { return "\""; } }
        public override string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            return base.GetTranslationTableName(entityName, isMapping).ToUpper();
        }
        public override string GetTranslationColumnName(string columnName)
        {
            return base.GetTranslationColumnName(columnName).ToUpper();
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            return base.GetDbColumnName(entityName,propertyName).ToUpper();
        }
    }
    public partial class OracleMethod : DefaultDbMethod, IDbMethods
    {
        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS VARCHAR2(4000))", parameter.MemberName);
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATE)", parameter.MemberName);
        }
        public override string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'||{1}||'%') ", parameter.MemberName, parameter2.MemberName);
        }
        public override string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like {1}||'%') ", parameter.MemberName, parameter2.MemberName);
        }
        public override string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format("  ({0} like '%'||{1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public override string Trim(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" trim({0}) ", parameter.MemberName);
        }
    }
}
