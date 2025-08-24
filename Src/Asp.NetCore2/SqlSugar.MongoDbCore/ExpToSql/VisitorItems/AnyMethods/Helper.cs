using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class MethodCallExpressionTractor
    { 
        private static bool IsAnyMethodCallOrEmpty(MethodCallExpression methodCallExpression, string name)
        {
            return IsAnyMethodCall(methodCallExpression, name) || IsAnyMethodCallEmpty(methodCallExpression, name);
        } 
        private bool IsCallAnyExpression(AnyArgModel anyArgModel)
        {
            var isleftCall = (anyArgModel.Left is MethodCallExpression) && anyArgModel.LeftCount >= 1;
            var isRightCall = !(anyArgModel.Right is MethodCallExpression) && anyArgModel.RightCount >= 1;
            var allCount = anyArgModel.LeftCount + anyArgModel.RightCount;
            if ((isleftCall || isRightCall) && allCount == 1)
            {
                anyArgModel.Left = isleftCall ? anyArgModel.Left : anyArgModel.Right;
                anyArgModel.Right = isleftCall ? anyArgModel.Right : anyArgModel.Left;
                return true;
            }
            return false;
        }
        private static bool IsAnyMethodCall(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Any" && methodCallExpression.Arguments.Count == 2;
        }
        private static bool IsAnyMethodCallEmpty(MethodCallExpression methodCallExpression, string name)
        {
            return name == "Any" && methodCallExpression.Arguments.Count == 1;
        }

        private static bool IsComplexAnyExpression(MethodCallExpression methodCallExpression, out AnyArgModel anyArgModel)
        {

            anyArgModel = new AnyArgModel() { IsBinary = false };

            if (methodCallExpression.Arguments.Count != 2)
                return false;
            var isTwoMemeber = ExpressionTool.GetParameters(methodCallExpression.Arguments[1]).Select(s => s.Name).Distinct().Count() == 2;

            if (!isTwoMemeber)
            {
                if (methodCallExpression.Arguments[1] is LambdaExpression la && la.Body is BinaryExpression bin)
                {
                    var leftCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(bin.Left)).Count();
                    var rightCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(bin.Right)).Count();
                    anyArgModel.IsBinary = true;
                    anyArgModel.LamdaExpression = la;
                    anyArgModel.Left = MongoDbExpTools.RemoveConvert(bin.Left);
                    anyArgModel.LeftCount = leftCount;
                    anyArgModel.RightCount = rightCount;
                    anyArgModel.Right = MongoDbExpTools.RemoveConvert(bin.Right);
                    anyArgModel.NodeType = bin.NodeType;
                }
                return false;
            }
            if (methodCallExpression.Arguments[1] is LambdaExpression l && l.Body is BinaryExpression b)
            {
                var leftCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(b.Left)).Count();
                var rightCount = ExpressionTool.GetParameters(MongoDbExpTools.RemoveConvert(b.Right)).Count();
                anyArgModel.LamdaExpression = l;
                anyArgModel.Left = MongoDbExpTools.RemoveConvert(b.Left);
                anyArgModel.LeftCount = leftCount;
                anyArgModel.RightCount = rightCount;
                anyArgModel.Right = MongoDbExpTools.RemoveConvert(b.Right);
                anyArgModel.NodeType = b.NodeType;
                if (leftCount != 1)
                    return false;
                if (rightCount != 1)
                    return false;
                anyArgModel.IsBinary = true;
            }
            return true;
        }
        private static bool IsSimpleValueListAny(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.Count == 2)
            {
                var memberExpression = methodCallExpression.Arguments[0] as MemberExpression;
                var lambdaExpression = methodCallExpression.Arguments[1] as LambdaExpression;
                if (memberExpression != null && lambdaExpression != null)
                {
                    var memberType = memberExpression.Type;
                    if (typeof(IEnumerable<string>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<int>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<long>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<double>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<float>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<Guid>).IsAssignableFrom(memberType) ||
                        typeof(IEnumerable<decimal>).IsAssignableFrom(memberType))
                    {
                        // 判断 lambda 是否是 s => s == value 或 s => s != value
                        if (lambdaExpression.Body is BinaryExpression binaryExpr)
                        {
                            if ((binaryExpr.Left is ParameterExpression && binaryExpr.Right is ConstantExpression) ||
                                (binaryExpr.Left is ParameterExpression && binaryExpr.Right != null))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


    }
}
