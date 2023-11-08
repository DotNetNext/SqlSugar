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
        public override string JsonArrayLength(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return $" (SELECT COUNT(*) FROM OPENJSON({parameter.MemberName})) ";
        }

        public override string JsonIndex(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return $"JSON_VALUE({parameter.MemberName}, '$[{parameter1.MemberValue}]')";
        }
        public override string CharIndexNew(MethodCallExpressionModel model)
        {
            return string.Format("CHARINDEX ({1},{0})", model.Args[0].MemberName, model.Args[1].MemberName);
        }
        public override string WeekOfYear(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            return $"DATEPART(WEEK, {parameterNameA})  ";
        }
        public override string GetTableWithDataBase(string dataBaseName, string tableName)
        {
            return $"{dataBaseName}.dbo.{tableName}";
        }
        public override string GetForXmlPath()
        {
            return "  FOR XML PATH('')),1,len(N','),'')  ";
        }
        public override string GetStringJoinSelector(string result, string separator)
        {
            return $"stuff((SELECT cast(N'{separator}' as nvarchar(max)) + cast({result} as nvarchar(max))";
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            if (parameter.MemberName != null && parameter.MemberName is DateTime)
            {
                return string.Format(" datepart({0},'{1}') ", parameter2.MemberValue, parameter.MemberName);
            }
            else
            {
                return string.Format(" datepart({0},{1}) ", parameter2.MemberValue, parameter.MemberName);
            }
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

        public override string JsonField(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            //var parameter2 = model.Args[2];
            //var parameter3= model.Args[3];
            var result = GetJson(parameter.MemberName, parameter1.MemberName, model.Args.Count() == 2);
            if (model.Args.Count > 2)
            {
                result = GetJson(result, model.Args[2].MemberName, model.Args.Count() == 3);
            }
            if (model.Args.Count > 3)
            {
                result = GetJson(result, model.Args[3].MemberName, model.Args.Count() == 4);
            }
            if (model.Args.Count > 4)
            {
                result = GetJson(result, model.Args[4].MemberName, model.Args.Count() == 5);
            }
            if (model.Args.Count > 5)
            {
                result = GetJson(result, model.Args[5].MemberName, model.Args.Count() == 6);
            }
            return result;
        }

        private string GetJson(object memberName1, object memberName2, bool isLast)
        {
            return $"JSON_VALUE({memberName1}, '$.'+"+memberName2+")";
        }

        public override string JsonListObjectAny(MethodCallExpressionModel model)
        {
            return $"(EXISTS (SELECT * from OPENJSON({model.Args[0].MemberName}) " +
                         $"WITH([value] NVARCHAR(MAX) '$.{model.Args[1].MemberValue.ToString().ToSqlFilter()}') " +
                         $"WHERE [value] = {model.Args[2].MemberName}))";
        }

        public override string JsonArrayAny(MethodCallExpressionModel model)
        {
           return string.Format("(EXISTS(SELECT * from OPENJSON({0}) WHERE [value] = {1}))"
                      , model.Args[0].MemberName
                      , model.Args[1].MemberName
                      );
        }
        public override string TrimEnd(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $"CASE WHEN RIGHT({parameterNameA}, 1) = {parameterNameB} THEN LEFT({parameterNameA}, LEN({parameterNameA}) - 1) ELSE {parameterNameA} END";
        }
        public override string TrimStart(MethodCallExpressionModel mode)
        {

            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            return $" CASE WHEN LEFT({parameterNameA}, 1) = {parameterNameB} THEN RIGHT({parameterNameA}, LEN({parameterNameA}) - 1) ELSE {parameterNameA} END ";
        }

        public override string PadLeft(MethodCallExpressionModel mode)
        {
            var parameterNameA = mode.Args[0].MemberName;
            var parameterNameB = mode.Args[1].MemberName;
            var parameterNameC = mode.Args[2].MemberName;
            return $" CONCAT(REPLICATE({parameterNameC}, {parameterNameB} - LEN({parameterNameA})), {parameterNameA})  ";
        }

        public override string NewUid(MethodCallExpressionModel mode)
        {
            return " NEWID() ";
        }

        public override string FullTextContains(MethodCallExpressionModel mode)
        {
            var columns = mode.Args[0].MemberName;
            if (mode.Args[0].MemberValue is List<string>) 
            {
                columns = "("+string.Join(",", mode.Args[0].MemberValue as List<string>)+")";
            }
            var searchWord = mode.Args[1].MemberName;
            return $" CONTAINS({columns},{searchWord}) ";
        }
    }


}