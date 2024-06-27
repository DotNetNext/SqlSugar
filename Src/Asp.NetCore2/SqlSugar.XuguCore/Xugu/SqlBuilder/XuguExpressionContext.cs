using System;

namespace SqlSugar.Xugu
{
    public partial class XuguExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public override string SqlParameterKeyWord => ":";
        public SqlSugarProvider Context { get; set; }
        public XuguExpressionContext() => base.DbMehtods = new XuguMethod();
        public override string SqlTranslationLeft { get; } = "\"";
        public override string SqlTranslationRight { get; } = "\"";
        public override bool IsTranslationText(string name)=> name.IsContainsIn(UtilConstants.Space, "(", ")");
        public override string GetLimit() => "";
    }
    public partial class XuguMethod : DefaultDbMethod, IDbMethods
    {
        public override string ParameterKeyWord { get; set; } = ":";
        public override string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" LENGTH({0}) ", parameter.MemberName);
        }

        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("NVL({0},{1})", parameter.MemberName, parameter1.MemberName);
        }

        public override string MergeString(params string[] strings)=> string.Join("||", strings);

        public override string GetRandom()=> " SYS_GUID() ";
        public override string GetForXmlPath() => null;// "  FOR XML PATH('')),1,len(N','),'')  ";
        public override string GetStringJoinSelector(string result, string separator)=> $"REPLACE(WM_CONCAT({result}),',','{separator}')";
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var type = (DateType)Enum.Parse(typeof(DateType), parameter2.MemberValue.ObjToString(), false);
            switch (type)
            {
                case DateType.Year: return $"GETYEAR({parameter.MemberName})";
                case DateType.Month: return $"GETMONTH({parameter.MemberName})";
                case DateType.Day: return $"GETDAY({parameter.MemberName})";
                case DateType.Hour: return $"GETHOUR({parameter.MemberName})";
                case DateType.Minute: return $"GETMINUTE({parameter.MemberName})";
                case DateType.Second: return $"GETSECOND({parameter.MemberName})";
                case DateType.Weekday: return $"DAYOFWEEK({parameter.MemberName})";
                case DateType.Quarter: return $"CEIL(GETMONTH({parameter.MemberName})/3)";
                case DateType.Millisecond:
                default: return $"DAYOFYEAR({parameter.MemberName})";
            }
        }
        public override string GetDate() => " SYSDATE  ";
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
        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("INSTR ({0},{1},1,1) ", model.Args[0].MemberName, model.Args[1].MemberName);
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
        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS NVARCHAR)", parameter.MemberName);
        }
    }
}
