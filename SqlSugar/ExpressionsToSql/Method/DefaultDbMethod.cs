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

        public string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INT)", parameter.Value);
        }

        public string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIGINT)", parameter.Value);
        }

        public string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS NVARCHAR(MAX))", parameter.Value);
        }

        public string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS UNIQUEIDENTIFIER)", parameter.Value);
        }

        public string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS FLOAT)", parameter.Value);
        }

        public string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIT)", parameter.Value);
        }

        public string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATETIME)", parameter.Value);
        }

        public string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS MONEY)", parameter.Value);
        }
        public string Substring(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("SUBSTRING({0},1 + {1},{2})", parameter.Value,parameter2.Value, parameter3.Value);
        }

        public string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("LEN({0})", parameter.Value);
        }

        public string Replace(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("REPLACE({0},{1},{2})", parameter.Value, parameter2.Value, parameter3.Value);
        }
    }
}
