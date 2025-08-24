using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar.MongoDb 
{
    public partial class MethodCallExpressionTractor
    {
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

    }
}
