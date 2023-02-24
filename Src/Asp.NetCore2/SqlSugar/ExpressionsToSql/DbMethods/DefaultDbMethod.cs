﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class DefaultDbMethod : IDbMethods
    {
        public virtual string RowNumber(MethodCallExpressionModel model) 
        {
            if (model.Args.Count == 1)
            {
                return $"row_number() over(order by {model.Args[0].MemberName.ObjToString().TrimEnd('\'').TrimStart('\'')})";
            }
            else
            {
                return $"row_number() over( partition by {model.Args[1].MemberName.ObjToString().TrimEnd('\'').TrimStart('\'')} order by {model.Args[0].MemberName.ObjToString().TrimEnd('\'').TrimStart('\'')})";
            }
        }
        public virtual string RowCount(MethodCallExpressionModel model) 
        {
            return "count(1) over()";
        }
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
            return string.Format("( {0} IS NULL   OR {0}='')", parameter.MemberName);
        }

        public virtual string HasValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("({0} IS NOT NULL )", parameter.MemberName);
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
                    else if (item != null && item.GetType()==UtilConstants.ByteArrayType)
                    {
                        var inStr= BitConverter.ToString((byte[])item).Replace("-", "");
                        inValues.Add(inStr);
                    }
                    else
                    {
                        inValues.Add(item);
                    }
                }
            }
            var value = model.Args[1].MemberName;
            string inValueString = null;
            var isNvarchar = model.Args.Count == 3;
            if (inValues != null && inValues.Count > 0)
            {
                if (isNvarchar&& model.Args[2].MemberValue.Equals(true))
                {
                    inValueString = inValues.ToArray().ToJoinSqlInValsN();
                }
                else
                {
                    inValueString = inValues.ToArray().ToJoinSqlInVals();
                }
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
            if (parameter.MemberName != null && parameter.MemberName is DateTime)
            {
                return string.Format(" DateName({0},'{1}') ", parameter2.MemberValue, parameter.MemberName);
            }
            else
            {
                return string.Format(" DateName({0},{1}) ", parameter2.MemberValue, parameter.MemberName);
            }
        }

        public virtual string GetStringJoinSelector(string result, string separator) 
        {
            return $"string_agg(({result})::text,'{separator}') ";
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
            return string.Format("COUNT(DISTINCT {0} )", parameter.MemberName);
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
        public virtual string TrueValue()
        {
            return "1 ";
        }

        public virtual string FalseValue()
        {
            return "0";
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

        public virtual string ToVarchar(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS VARCHAR(MAX))", parameter.MemberName);
        }
        public virtual string BitwiseAnd(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} & {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }
        public virtual string BitwiseInclusiveOR(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} | {1}) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public string Oracle_ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" to_date({0},{1}) ", parameter.MemberName, parameter2.MemberName); ;
        }
        public string Oracle_ToChar(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format("to_char({0},{1}) ", parameter.MemberName, parameter2.MemberName); ;
        }
        public string SqlServer_DateDiff(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" DATEDIFF({0},{1},{2}) ", parameter.MemberValue?.ToString().ToSqlFilter(), parameter2.MemberName, parameter3.MemberName); ;
        }

        public virtual string FormatRowNumber(MethodCallExpressionModel model)
        {
            var str = model.Args[0].MemberValue.ObjToString();
            var array = model.Args.Skip(1).Select(it => it.IsMember ? it.MemberName : it.MemberValue).ToArray();
            return string.Format("'" + str + "'", array);
        }
        public virtual string Format(MethodCallExpressionModel model)
        {
           
            var str ="'"+ model.Args[0].MemberValue.ObjToString()+"'";
            var revalue = MergeString("'", "$1", "'");
            if (revalue.Contains("concat("))
            {
                return FormatConcat(model);
            }
            str =Regex.Replace(str, @"(\{\d+?\})", revalue);
            var array = model.Args.Skip(1).Select(it => it.IsMember?it.MemberName:(it.MemberValue==null?"''":it.MemberValue.ToSqlValue()))
                .Select(it=>ToString(new MethodCallExpressionModel() { Args=new List<MethodCallExpressionArgs>() {
                 new MethodCallExpressionArgs(){ IsMember=true, MemberName=it }
                } })).ToArray();
             return string.Format(""+str+ "", array);
        }
        private  string FormatConcat(MethodCallExpressionModel model)
        {

            var str = "concat('" + model.Args[0].MemberValue.ObjToString() + "')";
            str = Regex.Replace(str, @"(\{\d+?\})", "',$1,'");
            var array = model.Args.Skip(1).Select(it => it.IsMember ? it.MemberName : it.MemberValue)
                .Select(it => ToString(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                 new MethodCallExpressionArgs(){ IsMember=true, MemberName=it }
                }
                })).ToArray();
            return string.Format("" + str + "", array);
        }

        public virtual string Abs(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" ABS({0}) ", parameter.MemberName);
        }

        public virtual string Round(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2= model.Args[1];
            return string.Format("  ROUND({0},{1}) ", parameter.MemberName, parameter2.MemberName);
        }

        public virtual string DateDiff(MethodCallExpressionModel model) 
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" DATEDIFF({0},{1},{2}) ", parameter.MemberValue?.ToString().ToSqlFilter(), parameter2.MemberName, parameter3.MemberName);
        }
        public virtual string GreaterThan(MethodCallExpressionModel model) 
        {
            //>
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} > {1}) ", parameter.MemberName, parameter2.MemberName); 
        }
        public virtual string GreaterThanOrEqual(MethodCallExpressionModel model) 
        {
            //>=
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} >= {1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public virtual string LessThan(MethodCallExpressionModel model) 
        {
            //<
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} < {1}) ", parameter.MemberName, parameter2.MemberName);
        }
        public virtual string LessThanOrEqual(MethodCallExpressionModel model) 
        {
            //<=
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} <= {1}) ", parameter.MemberName, parameter2.MemberName);
        }

        public virtual string Asc(MethodCallExpressionModel model) 
        {
            return model.Args[0].MemberName + " ASC ";
        }
        public virtual string Desc(MethodCallExpressionModel model)
        {
            return model.Args[0].MemberName + " DESC ";
        }
        public virtual string Stuff(MethodCallExpressionModel model) 
        {
            var parameter1 = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            var parameter4 = model.Args[3];
            return $" STUFF ({parameter1.MemberName}, {parameter2.MemberName}, {parameter3.MemberName},  {parameter4.MemberName}) ";
        }
        public virtual string Exists(MethodCallExpressionModel model) 
        {
            var parameter1 = model.Args[0];
            if (model.Args.Count > 1)
            {
                var parameter2 = model.Args[1];
                if (UtilMethods.IsParentheses(parameter1.MemberName))
                {
                    parameter1.MemberName = $" {parameter1.MemberName.ObjToString().Trim().TrimEnd(')')} AND {parameter2.MemberName}) ";
                }
                else
                {
                    parameter1.MemberName = $" {parameter1.MemberName} AND {parameter2.MemberName} ";
                }
            }
            if (UtilMethods.IsParentheses(parameter1.MemberName))
            {
                return $" Exists{parameter1.MemberName} ";
            }
            else
            {
                return $" Exists({parameter1.MemberName}) ";
            }
        }

        public virtual string GetDateString(string dateValue, string format)
        {
            return null;
        }
        public virtual string GetForXmlPath() 
        {
            return null;
        }

        public virtual string JsonField(MethodCallExpressionModel model)
        {
            throw new NotImplementedException("Current database no support");
        }

        public virtual string JsonContainsFieldName(MethodCallExpressionModel model)
        {
            throw new NotImplementedException("Current database no support");
        }

        public virtual string JsonArrayLength(MethodCallExpressionModel model)
        {
            throw new NotImplementedException("Current database no support");
        }

        public virtual string JsonParse(MethodCallExpressionModel model)
        {
            throw new NotImplementedException("Current database no support");
        }
        public virtual string JsonLike(MethodCallExpressionModel model) 
        {
            model.Args[0].MemberName = ToString(model);
            return Contains(model);
        }
        public virtual string Collate(MethodCallExpressionModel model) 
        {
            var name=model.Args[0].MemberName;
            return $" {name}  collate Chinese_PRC_CS_AS  ";
        }
        public virtual string AggregateSumNoNull(MethodCallExpressionModel model) 
        {
            model.Args.Add(new MethodCallExpressionArgs() { MemberValue = 0, MemberName = 0 });
            var name= IsNull(model);
            model.Args[0].MemberName = name;
            return AggregateSum(model);
        }
        public virtual string JsonListObjectAny(MethodCallExpressionModel model) 
        {
            throw new NotImplementedException("Current database no support");
        }
        public virtual string JsonArrayAny(MethodCallExpressionModel model) 
        {
            throw new NotImplementedException("Current database no support");
        }
        public virtual string CompareTo(MethodCallExpressionModel model) 
        {
            var parameterNameA=model.Args[0].MemberName;
            var parameterNameB = model.Args[1].MemberName;
            return $"(case when   {parameterNameA}>{parameterNameB}  then 1   when {parameterNameA}={parameterNameB} then 0 else -1 end)";
        }
        public virtual string SplitIn(MethodCallExpressionModel model)
        {
            var fullString = model.Args[0].MemberName+"";
            var value = model.Args[1].MemberName+"";
            var value1 = MergeString(value, "','");
            var value2 = MergeString("','", value);
            var value3 = MergeString("','", value, "','");
            var likeString1 = 
                StartsWith(new MethodCallExpressionModel() { Args = new List<MethodCallExpressionArgs>() { 
                 new MethodCallExpressionArgs(){ IsMember=true, MemberName=fullString },
                 new MethodCallExpressionArgs(){ IsMember=true, MemberName=value1 }
                } });
            var likeString2 =
                EndsWith(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                             new MethodCallExpressionArgs(){ IsMember=true, MemberName=fullString },
                             new MethodCallExpressionArgs(){ IsMember=true, MemberName=value2 }
                }
                });
            var likeString3 =
                Contains(new MethodCallExpressionModel()
                {
                    Args = new List<MethodCallExpressionArgs>() {
                                            new MethodCallExpressionArgs(){ IsMember=true, MemberName=fullString },
                                            new MethodCallExpressionArgs(){ IsMember=true, MemberName=value3 }
                }
                });
            return $" ({likeString1} or {likeString2}  or {likeString3} or {fullString}={value} ) ";
        }
        public virtual string GetTableWithDataBase(string dataBaseName,string tableName) 
        {
            return $"{dataBaseName}.{tableName}";
        }
    }
}
