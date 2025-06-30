using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class BinaryExpressionTranslator
    { 
        private BsonDocument GetCalculationOperationBySqlFunc(SqlFuncBinaryExpressionInfo expressionInfo, BsonValue field, ExpressionType nodeType, BsonValue value, bool leftIsMember, bool rightIsMember)
        {
            var mongoDbType = BinaryExpressionTranslator.GetComparisonType(expressionInfo.NodeType);
            var left = new ExpressionVisitor(_context).Visit(expressionInfo.LeftExp);
            var right = new ExpressionVisitor(_context).Visit(expressionInfo.RightExp);
            if (leftIsMember && !UtilMethods.IsMongoVariable(left))
                left = UtilMethods.GetMemberName(left);
            if (rightIsMember && !UtilMethods.IsMongoVariable(right))
                right = UtilMethods.GetMemberName(right);
            var filter = new BsonDocument("$expr", new BsonDocument(mongoDbType, new BsonArray
            {
                left,
                right
            }));
            return filter;
        }
        private SqlFuncBinaryExpressionInfo GetSqlFuncBinaryExpressionInfo(bool leftIsMember, bool rightIsMember, BinaryExpression expr)
        {
            SqlFuncBinaryExpressionInfo sqlFuncBinaryExpressionInfo = new SqlFuncBinaryExpressionInfo();
            var left = MongoDbExpTools.RemoveConvert(expr.Left);
            var right = MongoDbExpTools.RemoveConvert(expr.Right);
            sqlFuncBinaryExpressionInfo.LeftMethodName= GetSystemDateMemberName(left);
            sqlFuncBinaryExpressionInfo.LeftIsFunc =!leftIsMember&& left is MethodCallExpression &&ExpressionTool.GetParameters(left).Count>0;
            sqlFuncBinaryExpressionInfo.RightMethodName = GetSystemDateMemberName(right);
            sqlFuncBinaryExpressionInfo.RightIsFunc = !rightIsMember && right is MethodCallExpression && ExpressionTool.GetParameters(right).Count > 0; 
            sqlFuncBinaryExpressionInfo.LeftExp = MongoDbExpTools.RemoveConvert(left);
            sqlFuncBinaryExpressionInfo.RightExp = MongoDbExpTools.RemoveConvert(right);
            sqlFuncBinaryExpressionInfo.NodeType= expr.NodeType;
            return sqlFuncBinaryExpressionInfo;
        }
        public static string GetSystemDateMemberName(Expression expr)
        {
            // 只处理 MemberExpression
            // 确保是 MemberExpression（访问属性或字段）
            if (expr is MemberExpression memberExpr&&ExpressionTool.GetParameters(expr).Count>0)
            {
                var memberName = memberExpr.Member.Name;

                // 检查名字是否是我们关心的字段
                if (!AllowedDateParts.Contains(memberName))
                    return null;

                // 获取访问者对象的类型（例如 x.CreateTime.Year 中的 x.CreateTime）
                var expressionType = memberExpr.Expression.Type;

                // 确保它是 System.DateTime（可拓展为 Nullable<DateTime>）
                if (expressionType == typeof(DateTime) || expressionType == typeof(DateTime?))
                {
                    return memberName;
                }
            }

            return null;
        }
        public static readonly List<string> AllowedDateParts = new List<string>()
        {
            "Date","DayOfWeek", "Year", "Month", "Day", "Hour", "Minute", "Second", "Millisecond"
        };
    }
}
