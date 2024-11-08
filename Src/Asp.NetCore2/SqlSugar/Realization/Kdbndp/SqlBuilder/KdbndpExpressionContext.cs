﻿using System;
using System.Linq;
namespace SqlSugar
{
    public class KdbndpExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public KdbndpExpressionContext()
        {
            base.DbMehtods = new KdbndpMethod();
        }
        public override string SqlTranslationLeft
        {
            get
            {
                return "\"";
            }
        }
        public override string SqlTranslationRight
        {
            get
            {
                return "\"";
            }
        }
        public override string GetTranslationText(string name)
        {
            return SqlTranslationLeft + name.ToUpper(IsUpper) + SqlTranslationRight;
        }

        public override string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (IsTranslationText(entityName)) return entityName;
            isMapping = isMapping && this.MappingTables.HasValue();
            var isComplex = entityName.Contains(UtilConstants.Dot);
            if (isMapping && isComplex)
            {
                var columnInfo = entityName.Split(UtilConstants.DotChar);
                var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo != null)
                {
                    columnInfo[columnInfo.Length - 1] = mappingInfo.EntityName;
                }
                return string.Join(UtilConstants.Dot, columnInfo.Select(it => GetTranslationText(it)));
            }
            else if (isMapping)
            {
                var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));

                var tableName = mappingInfo?.DbTableName + "";
                if (tableName.Contains("."))
                {
                    tableName = string.Join(UtilConstants.Dot, tableName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
                    return tableName;
                }

                return SqlTranslationLeft + (mappingInfo == null ? entityName : mappingInfo.DbTableName).ToUpper(IsUpper) + SqlTranslationRight;
            }
            else if (isComplex)
            {
                return string.Join(UtilConstants.Dot, entityName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
            }
            else
            {
                return GetTranslationText(entityName);
            }
        }
        public override string GetTranslationColumnName(string columnName)
        {
            Check.ArgumentNullException(columnName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            if (columnName.Substring(0, 1) == this.SqlParameterKeyWord)
            {
                return columnName;
            }
            if (IsTranslationText(columnName)) return columnName;
            if (columnName.Contains(UtilConstants.Dot))
            {
                return string.Join(UtilConstants.Dot, columnName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
            }
            else
            {
                return GetTranslationText(columnName);
            }
        }
        public override string GetDbColumnName(string entityName, string propertyName)
        {
            if (this.MappingColumns.HasValue())
            {
                var mappingInfo = this.MappingColumns.SingleOrDefault(it => it.EntityName == entityName && it.PropertyName == propertyName);
                return (mappingInfo == null ? propertyName : mappingInfo.DbColumnName).ToUpper(IsUpper);
            }
            else
            {
                return propertyName.ToUpper(IsUpper);
            }
        }
        public bool IsUpper
        {
            get
            {
                if (this.SugarContext?.Context?.Context?.CurrentConnectionConfig?.MoreSettings == null)
                {
                    return true;
                }
                else
                {
                    return this.SugarContext?.Context?.Context?.CurrentConnectionConfig?.MoreSettings.IsAutoToUpper == true;
                }
            }
        }
    }
    public class KdbndpMethod : DefaultDbMethod, IDbMethods
    {
        public override string CharIndexNew(MethodCallExpressionModel model)
        {
            return string.Format(" (strpos ({0},{1}))", model.Args[0].MemberName, model.Args[1].MemberName);
        }
        public override string CharIndex(MethodCallExpressionModel model)
        {
            return string.Format(" (strpos ({1},{0})-1)", model.Args[0].MemberName, model.Args[1].MemberName);
        }
        public override string TrueValue()
        {
            return "true";
        }
        public override string FalseValue()
        {
            return "false";
        }
        public override string DateDiff(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model))
            {
                return base.DateDiff(model);
            }

            var parameter = (DateType)(Enum.Parse(typeof(DateType), model.Args[0].MemberValue.ObjToString()));
            var begin = model.Args[1].MemberName;
            var end = model.Args[2].MemberName;
            switch (parameter)
            {
                case DateType.Year:
                    return $" ( DATE_PART('Year',  {end}   ) - DATE_PART('Year',  {begin}) )";
                case DateType.Month:
                    return $" (  ( DATE_PART('Year',  {end}   ) - DATE_PART('Year',  {begin}) ) * 12 + (DATE_PART('month', {end}) - DATE_PART('month', {begin})) )";
                case DateType.Day:
                    return $" ( DATE_PART('day', {end} - {begin}) )";
                case DateType.Hour:
                    return $" ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) )";
                case DateType.Minute:
                    return $" ( ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) ) * 60 + DATE_PART('minute', {end} - {begin} ) )";
                case DateType.Second:
                    return $" ( ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) ) * 60 + DATE_PART('minute', {end} - {begin} ) ) * 60 + DATE_PART('second', {end} - {begin} )";
                case DateType.Millisecond:
                    break;
                default:
                    break;
            }
            throw new Exception(parameter + " datediff no support");
        }
        public override string IIF(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            if (parameter.Type == UtilConstants.BoolType)
            {
                parameter.MemberName = parameter.MemberName.ToString().Replace("=1", "=true");
                parameter2.MemberName = false;
                parameter3.MemberName = true;
            }
            return string.Format("( CASE  WHEN {0} THEN {1}  ELSE {2} END )", parameter.MemberName, parameter2.MemberName, parameter3.MemberName);
        }
        public override string Substring(MethodCallExpressionModel model)
        {
            if (model?.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.MySql)
            {
               return base.Substring(model).Replace(" + ", " operator(pg_catalog.+) ");
            }
            return base.Substring(model);
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var format = "dd";
            if (parameter2.MemberValue.ObjToString() == DateType.Year.ToString())
            {
                format = "yyyy";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Month.ToString())
            {
                format = "MM";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Day.ToString())
            {
                format = "dd";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Hour.ToString())
            {
                format = "hh24";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Minute.ToString())
            {
                format = "mi";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Second.ToString())
            {
                format = "ss";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Millisecond.ToString())
            {
                format = "ms";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Quarter.ToString())
            {
                format = "q";
            }
            if (parameter2.MemberValue.ObjToString() == DateType.Weekday.ToString())
            {
                return $"  extract(DOW FROM cast({parameter.MemberName} as TIMESTAMP)) ";
            }

            return string.Format(" cast( to_char({1},'{0}')as integer ) ", format, parameter.MemberName);
        }

        public override string Contains(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model)) 
            {
                return base.Contains(model);
            }
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like pg_catalog.concat('%',{1},'%')) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string StartsWith(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model))
            {
                return base.StartsWith(model);
            }
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like pg_catalog.concat({1},'%')) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string EndsWith(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model))
            {
                return base.EndsWith(model);
            }
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} like pg_catalog.concat('%',{1}))", parameter.MemberName, parameter2.MemberName);
        }

        public override string DateIsSameDay(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ( to_char({0},'yyyy-MM-dd')=to_char({1},'yyyy-MM-dd') ) ", parameter.MemberName, parameter2.MemberName); ;
        }

        public override string HasValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("( {0} IS NOT NULL )", parameter.MemberName);
        }

        public override string DateIsSameByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            DateType dateType = (DateType)parameter3.MemberValue;
            var format = "yyyy-MM-dd";
            if (dateType == DateType.Quarter)
            {
                return string.Format(" (date_trunc('quarter',{0})=date_trunc('quarter',{1}) ) ", parameter.MemberName, parameter2.MemberName, format);
            }
            switch (dateType)
            {
                case DateType.Year:
                    format = "yyyy";
                    break;
                case DateType.Month:
                    format = "yyyy-MM";
                    break;
                case DateType.Day:
                    break;
                case DateType.Hour:
                    format = "yyyy-MM-dd HH";
                    break;
                case DateType.Second:
                    format = "yyyy-MM-dd HH:mm:ss";
                    break;
                case DateType.Minute:
                    format = "yyyy-MM-dd HH:mm";
                    break;
                case DateType.Millisecond:
                    format = "yyyy-MM-dd HH:mm.ms";
                    break;
                default:
                    break;
            }
            return string.Format(" ( to_char({0},'{2}')=to_char({1},'{2}') ) ", parameter.MemberName, parameter2.MemberName, format);
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            if (IsSqlServerModel(model))
            {
                return string.Format(" CAST({0} AS dateTime)", parameter.MemberName);
            }
            return string.Format(" CAST({0} AS timestamp)", parameter.MemberName);
        }
        public override string DateAddByType(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model))
            {
                return base.DateAddByType(model);
            }
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            if (model?.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.MySql) 
            {
                return string.Format(" ({1} +  ({2} operator(pg_catalog.||) '{0}')::INTERVAL) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
            }
            return string.Format(" ({1} +  ({2}||'{0}')::INTERVAL) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
            if (IsSqlServerModel(model)) 
            {
                return base.DateAddDay(model);
            }
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0} + ({1}||'day')::INTERVAL) ", parameter.MemberName, parameter2.MemberName);
        }

        public override string ToInt32(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INT4)", parameter.MemberName);
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS INT8)", parameter.MemberName);
        }

        public override string ToString(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS VARCHAR)", parameter.MemberName);
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS UUID)", parameter.MemberName);
        }

        public override string ToDouble(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS DECIMAL(18,4))", parameter.MemberName);
        }

        public override string ToBool(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format(" CAST({0} AS boolean)", parameter.MemberName);
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
            var key = Guid.NewGuid() + "";
            return " pg_catalog.concat(" + string.Join(",", strings.Select(it => it?.Replace("+", key))).Replace("+", "").Replace(key, "+") + ") ";
        }
        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("(CASE WHEN  {0} IS NULL THEN  {1} ELSE {0} END)", parameter.MemberName, parameter1.MemberName);
        }
        public override string GetDate()
        {
            return "NOW()";
        }
        public override string GetRandom()
        {
            return "RANDOM()";
        }

        public override string EqualTrue(string fieldName)
        {
            return "( " + fieldName + "=true )";
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

        public override string JsonContainsFieldName(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return $"({parameter.MemberName}::jsonb ?{parameter1.MemberName})";
        }

        private string GetJson(object memberName1, object memberName2, bool isLast)
        {
            if (isLast)
            {
                return $"({memberName1}::json->>{memberName2})";
            }
            else
            {
                return $"({memberName1}->{memberName2})";
            }
        }

        public override string JsonArrayLength(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            //var parameter1 = model.Args[1];
            return $" json_array_length({parameter.MemberName}::json) ";
        }

        public override string JsonParse(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            //var parameter1 = model.Args[1];
            return $" ({parameter.MemberName}::json) ";
        }

        public override string JsonArrayAny(MethodCallExpressionModel model)
        {
            if (UtilMethods.IsNumber(model.Args[1].MemberValue.GetType().Name))
            {
                return $"{model.Args[0].MemberName}::jsonb @> '[{model.Args[1].MemberValue.ObjToStringNoTrim().ToSqlFilter()}]'::jsonb ";
            }
            else
            {
                return $"{model.Args[0].MemberName}::jsonb @> '[\"{model.Args[1].MemberValue}\"]'::jsonb ";
            }
        }
        public override string Format(MethodCallExpressionModel model)
        {
            return base.Format(model).Replace("concat(", "pg_catalog.concat(");
        }
        public override string JsonListObjectAny(MethodCallExpressionModel model)
        {
            if (UtilMethods.IsNumber(model.Args[2].MemberValue.GetType().Name))
            {
                return $"{model.Args[0].MemberName}::jsonb @> '[{{\"{model.Args[1].MemberValue}\":{model.Args[2].MemberValue}}}]'::jsonb ";
            }
            else
            {
                return $"{model.Args[0].MemberName}::jsonb @> '[{{\"{model.Args[1].MemberValue}\":\"{model.Args[2].MemberValue.ObjToStringNoTrim().ToSqlFilter()}\"}}]'::jsonb ";
            }
        }
        public override string GetDateString(string dateValue, string formatString)
        {
            if (formatString.HasValue() && formatString.Contains("hh:mm"))
            {
                formatString = formatString.Replace("hh:mm", "hh:mi");
            }
            else if (formatString.HasValue() && formatString.Contains("hhmm"))
            {
                formatString = formatString.Replace("hhmm", "hhmi");
            }
            else if (formatString.HasValue() && formatString.Contains("HH:mm"))
            {
                formatString = formatString.Replace("HH:mm", "HH:mi");
            }
            else if (formatString.HasValue() && formatString.Contains("HHmm"))
            {
                formatString = formatString.Replace("HHmm", "HHmi");
            }
            if (formatString.Contains("HH")) 
            {
                formatString = formatString.Replace("HH", "hh24");
            }
            return $" to_char({dateValue},'{formatString}') ";
        }

        private static bool IsSqlServerModel(MethodCallExpressionModel model)
        {
            return model?.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.SqlServer;
        }

    }
}