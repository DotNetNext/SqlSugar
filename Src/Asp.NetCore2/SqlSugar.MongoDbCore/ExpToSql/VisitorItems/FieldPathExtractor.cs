using Dm;
using MongoDB.Bson;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SqlSugar.MongoDb
{
    public class FieldPathExtractor
    {
        MongoNestedTranslatorContext _context;
        ExpressionVisitorContext _visitorContext;
        Dictionary<string, DateType> dateTypeNames = new Dictionary<string, DateType>
          {
              { "Year", DateType.Year },
              { "Month", DateType.Month },
              { "Day", DateType.Day },
              { "Hour", DateType.Hour },
              { "Minute", DateType.Minute },
              { "Second", DateType.Second },
              { "Millisecond", DateType.Millisecond },
              { "DayOfWeek", DateType.Weekday }
          };
        public FieldPathExtractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            _context = context;
            _visitorContext = visitorContext;
        }

        public BsonValue Extract(Expression expr)
        {
            var oldExp = expr;
            var oldMember = expr as MemberExpression;
            if (ExpressionTool.GetParameters(expr).Count == 0)
            {
                var value = ExpressionTool.GetMemberValue(oldMember.Member, oldExp);
                return UtilMethods.MyCreate(value);
            }
            else if (IsLength(oldMember))
            {
                var memberExp = (expr as MemberExpression);
                var method = new MongoDbMethod() { context = _context };
                var model = new MethodCallExpressionModel() { Args = new List<MethodCallExpressionArgs>() };
                model.Args.Add(new MethodCallExpressionArgs() { MemberValue = memberExp.Expression });
                return BsonDocument.Parse(method.Length(model));
            }
            else if (IsDateProperty(expr))
            {
                var memberExp = (expr as MemberExpression);
                var method = new MongoDbMethod() { context = _context };
                var model = new MethodCallExpressionModel() { Args = new List<MethodCallExpressionArgs>() };
                if (memberExp.Member.Name == "Date")
                {
                    model.Args.Add(new MethodCallExpressionArgs() { MemberValue = memberExp.Expression });
                    return BsonDocument.Parse(method.ToDateShort(model));
                }
                else if (dateTypeNames.TryGetValue(memberExp.Member.Name, out var dateType))
                {
                    model.Args.Add(new MethodCallExpressionArgs() { MemberValue = memberExp.Expression });
                    model.Args.Add(new MethodCallExpressionArgs() { MemberValue = dateType });
                    return BsonDocument.Parse(method.DateValue(model));
                }
            }
            return ExtractFieldPath(expr);
        }

        private static bool IsDateProperty(Expression expr)
        {
            return !string.IsNullOrEmpty(BinaryExpressionTranslator.GetSystemDateMemberName(expr));
        }

        private static bool IsLength(MemberExpression oldMember)
        {
            if (oldMember.Member.Name != "Length")
                return false;

            var expressionType = oldMember.Expression?.Type;
            if (expressionType == null)
                return false;

            return expressionType.IsArray
                || expressionType == typeof(string)
                || expressionType.FullName == "System.Span`1"
                || expressionType.FullName == "System.ReadOnlySpan`1";
        }

        private BsonValue ExtractFieldPath(Expression expr)
        {
            var parts = new Stack<string>();

            while (expr is MemberExpression member)
            {
                parts.Push(member.Member.Name);
                expr = member.Expression!;
            }

            if (_visitorContext != null)
            {
                _visitorContext.ExpType = typeof(MemberExpression);
            }
            string resultString = null;
            if (parts.Count == 1 && expr is ParameterExpression parameter&& _context?.context != null)
            {
                var entityInfo = _context.context.EntityMaintenance.GetEntityInfo(parameter.Type);
                var columnInfo = entityInfo.Columns.FirstOrDefault(s => s.PropertyName == parts.First());
                if (columnInfo != null)
                {
                    resultString = columnInfo.DbColumnName;
                }
                else 
                {
                    resultString = string.Join(".", parts);
                }
            }
            else
            {
                resultString = string.Join(".", parts);
            }
            var isJoin = this._context.queryBuilder?.IsSingle()==false;
            var shortName = ((ParameterExpression)expr)?.Name;
            var joinInfo = this._context.queryBuilder.JoinQueryInfos.FirstOrDefault(it => it.ShortName.EqualCase(shortName));
            var isObj = false;
            if (joinInfo != null)
            { 
                shortName = $"{joinInfo.ShortName}.";
                if (this._context.resolveType.IsIn(ResolveExpressType.SelectSingle,ResolveExpressType.SelectMultiple))
                {
                    // 构造 $ifNull 表达式
                    var columnString = $"{{ \"$ifNull\": [\"${joinInfo.ShortName}.{resultString}\", null] }}";
                    resultString = columnString;
                    isObj = true;
                }
            }
            if (isObj)
                return BsonDocument.Parse(resultString);
            else
                return UtilMethods.MyCreate(resultString);
        }
    }

}
