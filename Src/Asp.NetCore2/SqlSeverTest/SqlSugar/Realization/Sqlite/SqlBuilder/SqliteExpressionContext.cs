using System;
using System.Linq;
namespace SqlSugar
{
    public class SqliteExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public SqliteExpressionContext()
        {
            base.DbMehtods = new SqliteMethod();
        }
        public override string SqlTranslationLeft { get { return "`"; } }
        public override string SqlTranslationRight { get { return "`"; } }
    }
    public class SqliteMethod : DefaultDbMethod, IDbMethods
    {
        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("LENGTH({0})", parameter.MemberName);
        }

        public override string Substring(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("SUBSTR({0},1 + {1},{2})", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
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

        public override string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INTEGER)", parameter.MemberName);
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INTEGER)", parameter.MemberName);
        }

        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS TEXT)", parameter.MemberName);
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS TEXT)", parameter.MemberName);
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

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" DATETIME({0})", parameter.MemberName);
        }

        public override string ToDateShort(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" strftime('%Y-%m-%d', {0})", parameter.MemberName);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            if (parameter2.MemberValue.ObjToInt() < 0)
            {
                return string.Format(" DATE(DATETIME({0}), '-{1} days')", parameter.MemberName, Math.Abs(parameter2.MemberValue.ObjToInt()));
            }
            else
            {
                return string.Format(" DATE(DATETIME({0}), '+{1} days')", parameter.MemberName, parameter2.MemberValue);
            }
        }

        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0].MemberName;
            var parameter2 = model.Args[1].MemberValue;
            var parameter3 = model.Args[2].MemberValue;
            if (parameter2.ObjToInt() < 0)
            {
                return string.Format(" DATETIME(DATETIME({0}), '-{1} {2}s')", parameter, Math.Abs(parameter2.ObjToInt()), parameter3);
            }
            else
            {
                return string.Format(" DATETIME(DATETIME({0}), '+{1} {2}s')", parameter, parameter2, parameter3);
            }
        }

        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var typeName = model.Args[1].MemberValue.ToString();
            var parameter2 = typeName;
            var type = (DateType)Enum.Parse(typeof(DateType), parameter2, false);
            switch (type)
            {
                case DateType.Year:
                    parameter2 = "%Y";
                    break;
                case DateType.Month:
                    parameter2 = "%m";
                    break;
                case DateType.Day:
                    parameter2 = "%d";
                    break;
                case DateType.Hour:
                    parameter2 = "%H";
                    break;
                case DateType.Second:
                    parameter2 = "%S";
                    break;
                case DateType.Minute:
                    parameter2 = "%M";
                    break;
                case DateType.Millisecond:
                default:
                    Check.ThrowNotSupportedException(typeName);
                    break;
            }
            return string.Format(" CAST(STRFTIME('{1}', DATETIME(DATETIME({0}), 'LOCALTIME')) AS INTEGER)", parameter.MemberName, parameter2);
        }

        public override string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0].MemberName;
            var parameter2 = model.Args[1].MemberName;
            int time = 1;
            return string.Format("  date({0}, 'localtime',  'start of day')= date({1}, 'localtime',  'start of day') ", parameter, parameter2, time);
        }
        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0].MemberName;
            var parameter2 = model.Args[1].MemberName;
            var parameter3 = model.Args[2].MemberValue;
            var type = (DateType)Enum.Parse(typeof(DateType), parameter3.ObjToString(), false);
            int time = 1;
            switch (type)
            {
                case DateType.Year:
                    time = time * 1 / 365;
                    break;
                case DateType.Month:
                    time = time * 1 / 30;
                    break;
                case DateType.Day:
                    break;
                case DateType.Hour:
                    time = time * 24;
                    break;
                case DateType.Second:
                    time = time * 24 * 60 * 60;
                    break;
                case DateType.Minute:
                    time = time * 24 * 60;
                    break;
                case DateType.Millisecond:
                    time = time * 24 * 60 * 60 * 1000;
                    break;
            }
            return string.Format(" Cast((JulianDay({0}) - JulianDay({1}))  *{2} As INTEGER)", parameter, parameter2, time);
        }

        public override string MergeString(params string[] strings)
        {
            return  string.Join("||", strings).Replace("+","");
        }

        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("IFNULL({0},{1})", parameter.MemberName, parameter1.MemberName);
        }

        public override string GetDate()
        {
            return "DATETIME('now')";
        }

        public override string GetRandom()
        {
            return "RANDOM()";
        }

        public override string CharIndex(MethodCallExpressionModel model)
        {
            throw new NotSupportedException("Slqite Not Supported CharIndex");
        }
    }
}
