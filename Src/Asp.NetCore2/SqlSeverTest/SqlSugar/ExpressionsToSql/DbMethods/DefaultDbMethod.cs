using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class DefaultDbMethod : IDbMethods
    {
        public virtual string IIF(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("( CASE  WHEN {0} THEN {1}  ELSE {2} END )", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }

        public virtual string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0}='' OR {0} IS NULL )", parameter.MemberName);
        }

        public virtual string HasValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0}<>'' AND {0} IS NOT NULL )", parameter.MemberName);
        }

        public virtual string HasNumber(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0}>0 AND {0} IS NOT NULL )", parameter.MemberName);
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
            List<object> inValues = new List<object>();
            if (inValueIEnumerable != null)
            {
                foreach (var item in inValueIEnumerable)
                {
                    if (item != null && item.GetType().IsEnum())
                    {
                        inValues.Add(Convert.ToInt64(item));
                    }
                    else
                    {
                        inValues.Add(item);
                    }
                }
            }
            var value = model.Args[1].MemberName;
            string inValueString = null;
            if (inValues != null && inValues.Count > 0)
            {
                inValueString = inValues.ToArray().ToJoinSqlInVals();
            }
            if (inValueString.IsNullOrEmpty())
            {
                return " (1=2) ";
            }
            else
            {
                return string.Format(" ({0} IN ({1})) ", value, inValueString);
            }
        }

        public virtual string ContainsArrayUseSqlParameters(MethodCallExpressionModel model)
        {
            var inValueIEnumerable = (IEnumerable)model.Args[0].MemberValue;
            List<object> inValues = new List<object>();
            if (inValueIEnumerable != null)
            {
                foreach (var item in inValueIEnumerable)
                {
                    if (item != null && item.GetType().IsEnum())
                    {
                        inValues.Add(Convert.ToInt64(item));
                    }
                    else
                    {
                        inValues.Add(item);
                    }
                }
            }
            var value = model.Args[1].MemberName;
            string inValueString = null;
            if (inValues != null && inValues.Count > 0)
            {
                for (int i = 0; i < inValues.Count; i++)
                {
                    inValueString += model.Data + "_" + i+",";
                }
            }
            if (inValueString.IsNullOrEmpty())
            {
                return " (1=2) ";
            }
            else
            {
                inValueString=inValueString.TrimEnd(',');
                return string.Format(" ({0} IN ({1})) ", value, inValueString);
            }
        }

        public virtual string Equals(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} = {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public virtual string EqualsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            if (parameter2.MemberValue == null)
            {
                return string.Format(" ({0} is null) ", parameter.MemberName, parameter2.MemberName) ;
            }
            else
            {
                return string.Format(" ({0} = {1}) ", parameter.MemberName, parameter2.MemberName);
            }
        }
        public virtual string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEDIFF(day,{0},{1})=0) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public virtual string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEDIFF({2},{0},{1})=0) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        }

        public virtual string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" (DATEADD({2},{1},{0})) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        }

        public virtual string DateAddDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" (DATEADD(day,{1},{0})) ", parameter.MemberName, parameter2.MemberName);
        }

        public virtual string Between(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            var parameter2 = model.Args[2];
            return string.Format(" ({0} BETWEEN {1} AND {2}) ", parameter.MemberName, parameter1.MemberName, parameter2.MemberName);
        }

        public virtual string StartsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like {1}+'%') ", parameter.MemberName, parameter2.MemberName);
        }

        public virtual string EndsWith(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like '%'+{1}) ", parameter.MemberName, parameter2.MemberName);
        }

        public virtual string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" DateName({0},{1}) ", parameter2.MemberValue, parameter.MemberName);
        }

        public virtual string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INT)", parameter.MemberName);
        }

        public virtual string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIGINT)", parameter.MemberName);
        }

        public virtual string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS NVARCHAR(MAX))", parameter.MemberName);
        }

        public virtual string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS UNIQUEIDENTIFIER)", parameter.MemberName);
        }

        public virtual string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS FLOAT)", parameter.MemberName);
        }

        public virtual string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS BIT)", parameter.MemberName);
        }

        public virtual string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATETIME)", parameter.MemberName);
        }

        public virtual string ToDateShort(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DATE)", parameter.MemberName);
        }

        public virtual string ToTime(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS TIME)", parameter.MemberName);
        }

        public virtual string ToDecimal(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS MONEY)", parameter.MemberName);
        }
        public virtual string Substring(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("SUBSTRING({0},1 + {1},{2})", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }

        public virtual string Length(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("LEN({0})", parameter.MemberName);
        }

        public virtual string Replace(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format("REPLACE({0},{1},{2})", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }

        public virtual string AggregateSum(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("SUM({0})", parameter.MemberName);
        }

        public virtual string AggregateAvg(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("AVG({0})", parameter.MemberName);
        }

        public virtual string AggregateMin(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("MIN({0})", parameter.MemberName);
        }

        public virtual string AggregateMax(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("MAX({0})", parameter.MemberName);
        }

        public virtual string AggregateCount(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("COUNT({0})", parameter.MemberName);
        }

        public virtual string AggregateDistinctCount(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("COUNT(DISTINCT{0})", parameter.MemberName);
        }

        public virtual string MappingColumn(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("{0}", parameter1.MemberValue);
        }

        public virtual string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("ISNULL({0},{1})", parameter.MemberName, parameter1.MemberName);
        }

        public virtual string True()
        {
            return "( 1 = 1 ) ";
        }

        public virtual string False()
        {
            return "( 1 = 2 ) ";
        }

        public string GuidNew()
        {
            return "'" + Guid.NewGuid() + "' ";
        }

        public string GetSelfAndAutoFill(string shortName, bool isSingle)
        {
            if (isSingle) return "*";
            else
                return string.Format("{0}.*", shortName);
        }

        public virtual string MergeString(params string[] strings)
        {
            return string.Join("+", strings);
        }

        public virtual string Pack(string sql)
        {
            return "(" + sql + ")";
        }

        public virtual string EqualTrue(string fieldName)
        {
            return "( " + fieldName + "=1 )";
        }

        public virtual string Null()
        {
            return "NULL";
        }

        public virtual string GetDate()
        {
            return "GETDATE()";
        }

        public virtual string GetRandom()
        {
            return "NEWID()";
        }

        public virtual string CaseWhen(List<KeyValuePair<string, string>> sqls)
        {
            StringBuilder reslut = new StringBuilder();
            foreach (var item in sqls)
            {
                if (item.Key == "IF")
                {
                    reslut.AppendFormat(" ( CASE  WHEN {0} ", item.Value);
                }
                else if (item.Key == "End")
                {
                    reslut.AppendFormat("ELSE {0} END )", item.Value);
                }
                else if (item.Key == "Return")
                {
                    reslut.AppendFormat(" THEN {0} ", item.Value);
                }
                else {
                    reslut.AppendFormat(" WHEN {0} ", item.Value);
                }
            }
            return reslut.ToString();
        }
        public virtual string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format("CHARINDEX ({0},{1})", model.Args[0].MemberName, model.Args[1].MemberName);
        }

        public string ToVarchar(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS VARCHAR(MAX))", parameter.MemberName);
        }
        public  string BitwiseAnd(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} & {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }
        public string BitwiseInclusiveOR(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} | {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }
    }
}
