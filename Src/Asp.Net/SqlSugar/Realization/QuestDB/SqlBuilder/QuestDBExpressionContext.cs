﻿using System;
using System.Linq;
namespace SqlSugar
{
    public class QuestDBExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public SqlSugarProvider Context { get; set; }
        public QuestDBExpressionContext()
        {
            base.DbMehtods = new QuestDBMethod();
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
            return SqlTranslationLeft + name.ToLower(isAutoToLower) + SqlTranslationRight;
        }
        public bool isAutoToLower
        {
            get
            {
                return false;
            }
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

                var tableName = mappingInfo?.DbTableName+"";
                if (tableName.Contains("."))
                {
                    tableName = string.Join(UtilConstants.Dot, tableName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
                    return tableName;
                }

                return SqlTranslationLeft + (mappingInfo == null ? entityName : mappingInfo.DbTableName).ToLower(isAutoToLower) + SqlTranslationRight;
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
                return (mappingInfo == null ? propertyName : mappingInfo.DbColumnName).ToLower(isAutoToLower);
            }
            else
            {
                return propertyName.ToLower(isAutoToLower);
            }
        }

        public  string GetValue(object entityValue)
        {
            if (entityValue == null)
                return null;
            var type = UtilMethods.GetUnderType(entityValue.GetType());
            if (UtilConstants.NumericalTypes.Contains(type))
            {
                return entityValue.ToString();
            }
            else if (type == UtilConstants.DateType)
            {
                return this.DbMehtods.ToDate(new MethodCallExpressionModel()
                {
                    Args = new System.Collections.Generic.List<MethodCallExpressionArgs>() {
                 new MethodCallExpressionArgs(){ MemberName=$"'{entityValue}'" }
                }
                });
            }
            else 
            {
                return this.DbMehtods.ToString(new MethodCallExpressionModel()
                {
                    Args = new System.Collections.Generic.List<MethodCallExpressionArgs>() {
                 new MethodCallExpressionArgs(){ MemberName=$"'{entityValue}'" }
                }
                });
            }
        }
    }
    public class QuestDBMethod : DefaultDbMethod, IDbMethods
    {
        //public override string DateIsSameByType(MethodCallExpressionModel model)
        //{
        //    var parameter = model.Args[0];
        //    var parameter2 = model.Args[1];
        //    var parameter3 = model.Args[2];

        //    return string.Format(" (DATEDIFF('{2}',{0},{1})=0) ", parameter.MemberName, parameter2.MemberName, parameter3.MemberValue);
        //}
        public override string Contains(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            return string.Format(" ({0}~{1} ) ", parameter.MemberName, parameter2.MemberName);
        }
        public override string DateDiff(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" DATEDIFF('{0}',{1},{2}) ", parameter.MemberValue.ObjToString().First(), parameter2.MemberName, parameter3.MemberName);
        }
        public override string TrueValue()
        {
            return "1";
        }
        public override string FalseValue()
        {
            return "0";
        }
        //public override string DateDiff(MethodCallExpressionModel model)
        //{
        //    var parameter = (DateType)(Enum.Parse(typeof(DateType), model.Args[0].MemberValue.ObjToString()));
        //    var begin = model.Args[1].MemberName;
        //    var end = model.Args[2].MemberName;
        //    switch (parameter)
        //    {
        //        case DateType.Year:
        //            return $" ( DATE_PART('Year',  {end}   ) - DATE_PART('Year',  {begin}) )";
        //        case DateType.Month:
        //            return $" (  ( DATE_PART('Year',  {end}   ) - DATE_PART('Year',  {begin}) ) * 12 + (DATE_PART('month', {end}) - DATE_PART('month', {begin})) )";
        //        case DateType.Day:
        //            return $" ( DATE_PART('day', {end} - {begin}) )";
        //        case DateType.Hour:
        //            return $" ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) )";
        //        case DateType.Minute:
        //            return $" ( ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) ) * 60 + DATE_PART('minute', {end} - {begin} ) )";
        //        case DateType.Second:
        //            return $" ( ( ( DATE_PART('day', {end} - {begin}) ) * 24 + DATE_PART('hour', {end} - {begin} ) ) * 60 + DATE_PART('minute', {end} - {begin} ) ) * 60 + DATE_PART('minute', {end} - {begin} )";
        //        case DateType.Millisecond:
        //            break;
        //        default:
        //            break;
        //    }
        //    throw new Exception(parameter + " datediff no support");
        //}
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
        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var format = parameter2.MemberValue.ObjToString();
            return string.Format("  {1}({0})  ) ", format, parameter.MemberName);
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
            DateType dateType =(DateType)parameter3.MemberValue;
            var format = "yyyy-MM-dd";
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
        public override string ToDateShort(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("to_date (to_str({0}, 'yyyy-MM-dd'),'yyyy-MM-dd')", parameter.MemberName);
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            return string.Format("CAST({0} AS DATE)", parameter.MemberName);
        }
        public override string DateAddByType(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" ({1} +  ({2}||'{0}')::INTERVAL) ", parameter3.MemberValue, parameter.MemberName, parameter2.MemberName);
        }

        public override string DateAddDay(MethodCallExpressionModel model)
        {
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
            return string.Format(" CAST({0} AS VARCHAR)", parameter.MemberName);
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
            return " concat("+string.Join(",", strings).Replace("+", "") + ") ";
        }
        public override string IsNull(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter1 = model.Args[1];
            return string.Format("(CASE WHEN  {0} IS NULL THEN  {1} ELSE {0} END)", parameter.MemberName, parameter1.MemberName);
        }
        public override string GetDate()
        {
            return "now()";
        }
        public override string GetRandom()
        {
            return "now()";
        }

        public override string EqualTrue(string fieldName)
        {
            return "( " + fieldName + "=true )";
        }

        public override string GetDateString(string dateValue,string format)
        {
            return  $"  to_str({dateValue},'{format}') ";
        }
    }
}
