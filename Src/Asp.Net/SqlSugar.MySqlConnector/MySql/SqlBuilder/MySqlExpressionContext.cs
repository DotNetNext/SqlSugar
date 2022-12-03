using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar.MySqlConnector
{
    public class MySqlExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public MySqlExpressionContext()
        {
            base.DbMehtods = new MySqlMethod();
        }
        public override string SqlTranslationLeft { get { return "`"; } }
        public override string SqlTranslationRight { get { return "`"; } }
    }
    public class MySqlMethod : DefaultDbMethod, IDbMethods
    {
        public override string GetStringJoinSelector(string result, string separator)
        {
            return $"group_concat({result}  separator '{separator}') ";
        }
        public override string DateDiff(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" TIMESTAMPDIFF({0},{1},{2}) ", parameter.MemberValue?.ToString().ToSqlFilter(), parameter2.MemberName, parameter3.MemberName);
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            if (parameter.MemberName != null && parameter.MemberName is DateTime)
            {
                return string.Format(" {0}('{1}') ", parameter2.MemberValue, parameter.MemberName);
            }
            else
            {
                return string.Format(" {0}({1}) ", parameter2.MemberValue, parameter.MemberName);
            }
        }

        public override string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat('%',{1},'%')) ", parameter.MemberName, parameter2.MemberName  );
        }

        public override string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat({1},'%')) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like concat('%',{1}))", parameter.MemberName,parameter2.MemberName);
        }

        public override string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (TIMESTAMPDIFF(day,date({0}),date({1}))=0) ", parameter.MemberName, parameter2.MemberName); ;
        }
        
        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            if (parameter3.MemberValue.ObjToString() == DateType.Weekday.ObjToString()) 
            {
                parameter3.MemberValue = "Week";
            }
            return string.Format(" (TIMESTAMPDIFF({2},{0},{1})=0) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        }

        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            if (parameter3.MemberValue.ObjToString() == "Millisecond") 
            {
                parameter3.MemberValue = "Second";
                return string.Format(" (DATE_ADD({1} , INTERVAL {2}/1000 {0})) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
            }
            return string.Format(" (DATE_ADD({1} , INTERVAL {2} {0})) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATE_ADD({0}, INTERVAL {1} day)) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS CHAR)", parameter.MemberName);
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS CHAR)", parameter.MemberName);
        }

        public override string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DECIMAL(18,4))", parameter.MemberName);
        }

        public override string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS SIGNED)", parameter.MemberName);
        }

        public override string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DECIMAL(18,4))", parameter.MemberName);
        }

        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" LENGTH({0})", parameter.MemberName);
        }
        public override string MergeString(params string[] strings)
        {
            return " concat("+string.Join(",", strings) + ") ";
        }
        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            if (parameter1.MemberValue is bool)
            {
                return string.Format("IFNULL(CAST({0} as SIGNED),{1})", parameter.MemberName, parameter1.MemberName);
            }
            else
            {
                return string.Format("IFNULL({0},{1})", parameter.MemberName, parameter1.MemberName);
            }
        }
        public override string GetDate()
        {
            return "NOW()";
        }

        public override string GetRandom()
        {
            return "rand()";
        }

        public override string Collate(MethodCallExpressionModel model)
        {
            var name = model.Args[0].MemberName;
            return $" binary {name}  ";
        }

        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("instr ({0},{1})", model.Args[0].MemberName, model.Args[1].MemberName);
        }

        public override string JsonField(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            //var parameter2 = model.Args[2];
            //var parameter3= model.Args[3];
            var result = GetJson(parameter.MemberName, parameter1.MemberValue, model.Args.Count() == 2);
            if (model.Args.Count > 2)
            {
                result = GetJson(result, model.Args[2].MemberValue, model.Args.Count() == 3);
            }
            if (model.Args.Count > 3)
            {
                result = GetJson(result, model.Args[3].MemberValue, model.Args.Count() == 4);
            }
            if (model.Args.Count > 4)
            {
                result = GetJson(result, model.Args[4].MemberValue, model.Args.Count() == 5);
            }
            if (model.Args.Count > 5)
            {
                result = GetJson(result, model.Args[5].MemberValue, model.Args.Count() == 6);
            }
            return result;
        }

        private string GetJson(object memberName1, object memberName2, bool isLast)
        {
            return $"{memberName1}->\"$.{memberName2}\"";
        }

        public override string JsonArrayAny(MethodCallExpressionModel model)
        {
            if (SqlSugar.UtilMethods.IsNumber(model.Args[1].MemberValue.GetType().Name))
            {
                return $"JSON_CONTAINS({model.Args[0].MemberName}, '{model.Args[1].MemberValue}')";
            }
            else
            {
                return $"JSON_CONTAINS({model.Args[0].MemberName}, '\"{model.Args[1].MemberValue.ObjToStringNoTrim().ToSqlFilter()}\"')";
            }
        }
        public override string JsonListObjectAny(MethodCallExpressionModel model)
        {
            if (SqlSugar.UtilMethods.IsNumber(model.Args[2].MemberValue.GetType().Name))
            {
                return $"JSON_CONTAINS({model.Args[0].MemberName},'{{\"{model.Args[1].MemberValue}\":{model.Args[2].MemberValue}}}')";
            }
            else
            {
                return $"JSON_CONTAINS({model.Args[0].MemberName},'{{\"{model.Args[1].MemberValue}\":\"{model.Args[2].MemberValue.ObjToStringNoTrim().ToSqlFilter()}\"}}')";
            }
        }
    }
}
