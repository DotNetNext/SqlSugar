﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IDbMethods
    {
        string IIF(MethodCallExpressionModel model);
        string HasNumber(MethodCallExpressionModel model);
        string HasValue(MethodCallExpressionModel model);
        string IsNullOrEmpty(MethodCallExpressionModel model);
        string ToLower(MethodCallExpressionModel model);
        string ToUpper(MethodCallExpressionModel model);
        string Trim(MethodCallExpressionModel model);
        string Contains(MethodCallExpressionModel model);
        string ContainsArray(MethodCallExpressionModel model);
        string ContainsArrayUseSqlParameters(MethodCallExpressionModel model);
        string Equals(MethodCallExpressionModel model);
        string EqualsNull(MethodCallExpressionModel model);
        string DateIsSameDay(MethodCallExpressionModel model);
        string DateIsSameByType(MethodCallExpressionModel model);
        string DateAddByType(MethodCallExpressionModel model);

        string DateValue(MethodCallExpressionModel model);
        string DateAddDay(MethodCallExpressionModel model);
        string Between(MethodCallExpressionModel model);
        string StartsWith(MethodCallExpressionModel model);
        string EndsWith(MethodCallExpressionModel model);
        string ToInt32(MethodCallExpressionModel model);
        string GetStringJoinSelector(string result,string separator);
        string ToInt64(MethodCallExpressionModel model);
        string ToString(MethodCallExpressionModel model);
        string ToVarchar(MethodCallExpressionModel model); 
        string ToGuid(MethodCallExpressionModel model);
        string ToDouble(MethodCallExpressionModel model);
        string ToBool(MethodCallExpressionModel model);
        string CaseWhen(List<KeyValuePair<string,string>> sqls);
        string Substring(MethodCallExpressionModel model);
        string ToDate(MethodCallExpressionModel model);
        string ToDateShort(MethodCallExpressionModel model);
        string ToTime(MethodCallExpressionModel model);
        string ToDecimal(MethodCallExpressionModel model);
        string Length(MethodCallExpressionModel model);
        string Replace(MethodCallExpressionModel model);
        string AggregateSum(MethodCallExpressionModel model);
        string AggregateAvg(MethodCallExpressionModel model);
        string AggregateMin(MethodCallExpressionModel model);
        string AggregateMax(MethodCallExpressionModel model);
        string AggregateCount(MethodCallExpressionModel model);
        string AggregateDistinctCount(MethodCallExpressionModel model);
        string MappingColumn(MethodCallExpressionModel model);
        string IsNull(MethodCallExpressionModel model);
        string GetSelfAndAutoFill(string shortName,bool isSingle);
        string True();
        string False();
        string TrueValue();
        string FalseValue();
        string GuidNew();
        string MergeString(params string[] strings);
        string EqualTrue(string value);
        string Pack(string sql);
        string Null();
        string GetDate();
        string GetRandom();
        string CharIndex(MethodCallExpressionModel model);
        string BitwiseAnd(MethodCallExpressionModel model);
        string BitwiseInclusiveOR(MethodCallExpressionModel model);

        string Oracle_ToDate(MethodCallExpressionModel model);
        string Oracle_ToChar(MethodCallExpressionModel model);
        string SqlServer_DateDiff(MethodCallExpressionModel model);
        string Format(MethodCallExpressionModel model);
        string FormatRowNumber(MethodCallExpressionModel model);
        string Abs(MethodCallExpressionModel model);
        string Round(MethodCallExpressionModel model);

        string DateDiff(MethodCallExpressionModel model);
        string GreaterThan(MethodCallExpressionModel model);
        string GreaterThanOrEqual(MethodCallExpressionModel model);
        string  LessThan(MethodCallExpressionModel model);
        string LessThanOrEqual(MethodCallExpressionModel model);
        string Asc(MethodCallExpressionModel model);
        string Desc(MethodCallExpressionModel model);
        string Stuff(MethodCallExpressionModel model);
        string RowNumber(MethodCallExpressionModel model);
        string RowCount(MethodCallExpressionModel model);
        string Exists(MethodCallExpressionModel model);
        string GetDateString(string dateValue,string format);
        string GetForXmlPath();
        string JsonField(MethodCallExpressionModel model);
        string JsonContainsFieldName(MethodCallExpressionModel model);
        string JsonArrayLength(MethodCallExpressionModel model);
        string JsonParse(MethodCallExpressionModel model);
        string JsonLike(MethodCallExpressionModel model);
        string Collate(MethodCallExpressionModel model);
        string AggregateSumNoNull(MethodCallExpressionModel model);
        string JsonListObjectAny(MethodCallExpressionModel model);
        string JsonArrayAny(MethodCallExpressionModel model);
        string CompareTo(MethodCallExpressionModel model);
        string SplitIn(MethodCallExpressionModel model);
        string GetTableWithDataBase(string databaseName,string tableName);
    }
}
