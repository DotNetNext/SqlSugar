using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public abstract partial class DefaultDbMethod : IDbMethods
    {
        public virtual string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0}='' OR {0} IS NULL )", parameter.Value);
        }

        public virtual string ToUpper(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (UPPER({0})) ", parameter.Value);
        }

        public virtual string ToLower(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (LOWER({0})) ", parameter.Value);
        }

        public virtual string Trim(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (rtrim(ltrim({0}))) ", parameter.Value);
        }

        public virtual string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'+{1}+'%') ", parameter.Value, parameter2.Value);
        }

        public string Equals(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} = {1}) ", parameter.Value, parameter2.Value); ;
        }

        public string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEDIFF(day,{0},{1})=0) ", parameter.Value, parameter2.Value); ;
        }

        public string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEDIFF({2},{0},{1})=0) ", parameter.Value, parameter2.Value, parameter3.Value); 
        }

        public string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEADD({2},{1},{0})) ", parameter.Value, parameter2.Value, parameter3.Value);
        }

        public string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEADD(day,{1},{0})) ", parameter.Value, parameter2.Value); 
        }

        public string Between(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            var parameter2 = model.Args[2];
            return string.Format(" ({0} BETWEEN {1} AND {2}) ", parameter.Value, parameter1.Value, parameter2.Value);
        }

        public string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like {1}+'%') ", parameter.Value, parameter2.Value);
        }

        public string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'+{1}) ", parameter.Value, parameter2.Value);
        }

        public string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0}({1})) ", parameter2.Value, parameter.Value);
        }
    }
}
