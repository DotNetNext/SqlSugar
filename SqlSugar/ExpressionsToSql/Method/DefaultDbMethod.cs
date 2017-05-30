using System;
using System.Collections;
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
            return string.Format("( {0}='' OR {0} IS NULL )", parameter.MemberName);
        }

        public virtual string ToUpper(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (UPPER({0})) ", parameter.MemberName);
        }

        public virtual string ToLower(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (LOWER({0})) ", parameter.MemberName);
        }

        public virtual string Trim(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" (rtrim(ltrim({0}))) ", parameter.MemberName);
        }

        public virtual string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'+{1}+'%') ", parameter.MemberName, parameter2.MemberName);
        }
        public virtual string ContainsArray(MethodCallExpressionModel model)
        {
            var inValueIEnumerable = (IEnumerable)model.Args[0].MemberValue;
            List<object>  inValues = new List<object>();
            if (inValueIEnumerable != null)
            {
                foreach (var item in inValueIEnumerable)
                {
                    inValues.Add(item);
                }
            }
            var value = model.Args[1].MemberName;
            string inValueString = null;
            if (inValues != null && inValues.Count > 0)
            {
                inValueString = inValues.ToArray().ToJoinSqlInVals();
            }
            else {
               return " (1=2) ";
            }
            return string.Format(" ({0} IN ({1})) ", value,inValueString);
        }

        public string Equals(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} = {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEDIFF(day,{0},{1})=0) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEDIFF({2},{0},{1})=0) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberName); 
        }

        public string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEADD({2},{1},{0})) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }

        public string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEADD(day,{1},{0})) ", parameter.MemberName, parameter2.MemberName); 
        }

        public string Between(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            var parameter2 = model.Args[2];
            return string.Format(" ({0} BETWEEN {1} AND {2}) ", parameter.MemberName, parameter1.MemberName, parameter2.MemberName);
        }

        public string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like {1}+'%') ", parameter.MemberName, parameter2.MemberName);
        }

        public string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'+{1}) ", parameter.MemberName, parameter2.MemberName);
        }

        public string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0}({1})) ", parameter2.MemberName, parameter.MemberName);
        }

        public string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INT)", parameter.MemberName);
        }

        public string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIGINT)", parameter.MemberName);
        }

        public string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS NVARCHAR(MAX))", parameter.MemberName);
        }

        public string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS UNIQUEIDENTIFIER)", parameter.MemberName);
        }

        public string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS FLOAT)", parameter.MemberName);
        }

        public string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIT)", parameter.MemberName);
        }

        public string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATETIME)", parameter.MemberName);
        }

        public string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS MONEY)", parameter.MemberName);
        }
        public string Substring(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("SUBSTRING({0},1 + {1},{2})", parameter.MemberName,parameter2.MemberName, parameter3.MemberName);
        }

        public string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("LEN({0})", parameter.MemberName);
        }

        public string Replace(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("REPLACE({0},{1},{2})", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }

        public string AggregateSum(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("SUM({0})", parameter.MemberName);
        }

        public string AggregateAvg(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("AVG({0})", parameter.MemberName);
        }

        public string AggregateMin(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("MIN({0})", parameter.MemberName);
        }

        public string AggregateMax(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("MAX({0})", parameter.MemberName);
        }

        public string AggregateCount(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("COUNT({0})", parameter.MemberName);
        }
    }
}
