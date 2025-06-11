using MongoDB.Bson.IO;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace SqlSugar.MongoDb
{
    public class MongoDbExpressionContext : ExpressionContext, ILambdaExpressions
    {
        public new void Resolve(Expression expression, ResolveExpressType resolveType) 
        {
            var context = new MongoNestedTranslatorContext();
            context.resolveType = resolveType;
            context.context = this.SugarContext.Context;
            context.queryBuilder = this.SugarContext.QueryBuilder;
            var sql=MongoNestedTranslator.Translate(expression, context);
            var shellString = sql.ToJson(new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Shell
            }); 
            this.Result.Append(shellString);
        }

        public SqlSugarProvider Context { get; set; }
        public MongoDbExpressionContext()
        {
            base.DbMehtods = new MongoDbMethod();
        }
        public override string GetLimit()
        {
            return " LIMIT 1 OFFSET 0 ";
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
                return base.PgSqlIsAutoToLower;
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
    public class MongoDbMethod : DefaultDbMethod, IDbMethods
    {
        public MongoNestedTranslatorContext  context { get; set; }
        public override string AggregateCount(MethodCallExpressionModel model)
        {
            var index = context.queryBuilder.LambdaExpressions.Index;
            var name = "count" + index;
            var item=model.Args.First().MemberValue;
            BsonValue arg = new ExpressionVisitor(context).Visit(item as Expression); 
            if (arg is BsonString str && !str.AsString.StartsWith("$"))
            {
                arg = "$" + str.AsString;
            }
            var countExpression = new BsonDocument(name, new BsonDocument("$sum",1));
            var result= countExpression.ToJson(SqlSugar.MongoDb.UtilMethods.GetJsonWriterSettings());
            context.queryBuilder.GroupByValue += $"({UtilConstants.ReplaceCommaKey}({result}){UtilConstants.ReplaceCommaKey})";
            context.queryBuilder.LambdaExpressions.Index++;
            return name;
        }

        public override string AggregateMax(MethodCallExpressionModel model)
        {
            var index = context.queryBuilder.LambdaExpressions.Index;
            var name = "max" + index;
            var item = model.Args.First().MemberValue;
            BsonValue arg = new ExpressionVisitor(context).Visit(item as Expression);
            if (arg is BsonString str && !str.AsString.StartsWith("$"))
            {
                arg = "$" + str.AsString;
            }
            var maxExpression = new BsonDocument(name, new BsonDocument("$max", arg));
            var result = maxExpression.ToJson(SqlSugar.MongoDb.UtilMethods.GetJsonWriterSettings());
            context.queryBuilder.GroupByValue += $"({UtilConstants.ReplaceCommaKey}({result}){UtilConstants.ReplaceCommaKey})";
            context.queryBuilder.LambdaExpressions.Index++;
            return name;
        }

        public override string AggregateMin(MethodCallExpressionModel model)
        {
            var index = context.queryBuilder.LambdaExpressions.Index;
            var name = "min" + index;
            var item = model.Args.First().MemberValue;
            BsonValue arg = new ExpressionVisitor(context).Visit(item as Expression);
            if (arg is BsonString str && !str.AsString.StartsWith("$"))
            {
                arg = "$" + str.AsString;
            }
            var minExpression = new BsonDocument(name, new BsonDocument("$min", arg));
            var result = minExpression.ToJson(SqlSugar.MongoDb.UtilMethods.GetJsonWriterSettings());
            context.queryBuilder.GroupByValue += $"({UtilConstants.ReplaceCommaKey}({result}){UtilConstants.ReplaceCommaKey})";
            context.queryBuilder.LambdaExpressions.Index++;
            return name;
        }

        public override string AggregateAvg(MethodCallExpressionModel model)
        {
            var index = context.queryBuilder.LambdaExpressions.Index;
            var name = "avg" + index;
            var item = model.Args.First().MemberValue;
            BsonValue arg = new ExpressionVisitor(context).Visit(item as Expression);
            if (arg is BsonString str && !str.AsString.StartsWith("$"))
            {
                arg = "$" + str.AsString;
            }
            var avgExpression = new BsonDocument(name, new BsonDocument("$avg", arg));
            var result = avgExpression.ToJson(SqlSugar.MongoDb.UtilMethods.GetJsonWriterSettings());
            context.queryBuilder.GroupByValue += $"({UtilConstants.ReplaceCommaKey}({result}){UtilConstants.ReplaceCommaKey})";
            context.queryBuilder.LambdaExpressions.Index++;
            return name;
        }

        public override string AggregateSum(MethodCallExpressionModel model)
        {
            var index = context.queryBuilder.LambdaExpressions.Index;
            var name = "sum" + index;
            var item = model.Args.First().MemberValue;
            BsonValue arg = new ExpressionVisitor(context).Visit(item as Expression);
            if (arg is BsonString str && !str.AsString.StartsWith("$"))
            {
                arg = "$" + str.AsString;
            }
            var sumExpression = new BsonDocument(name, new BsonDocument("$sum", arg));
            var result = sumExpression.ToJson(SqlSugar.MongoDb.UtilMethods.GetJsonWriterSettings());
            context.queryBuilder.GroupByValue += $"({UtilConstants.ReplaceCommaKey}({result}){UtilConstants.ReplaceCommaKey})";
            context.queryBuilder.LambdaExpressions.Index++;
            return name;
        }
    }
}
