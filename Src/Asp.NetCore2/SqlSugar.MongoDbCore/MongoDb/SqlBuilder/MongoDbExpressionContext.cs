using MongoDB.Bson.IO;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
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
        public override string ToInt32(MethodCallExpressionModel model)
        {
            var item =model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toIntDoc = new BsonDocument("$toInt", $"${memberName}");
            return toIntDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toLongDoc = new BsonDocument("$toLong", $"${memberName}");
            return toLongDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            // MongoDB 没有直接的 Guid 类型，通常以字符串存储
            var toStringDoc = new BsonDocument("$toString", $"${memberName}");
            return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToDouble(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toDoubleDoc = new BsonDocument("$toDouble", $"${memberName}");
            return toDoubleDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToBool(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toBoolDoc = new BsonDocument("$toBool", $"${memberName}");
            return toBoolDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toDateDoc = new BsonDocument("$toDate", $"${memberName}");
            return toDateDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToTime(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            // MongoDB 没有单独的 Time 类型，通常用字符串或日期处理
            var toStringDoc = new BsonDocument("$toString", $"${memberName}");
            return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
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
        public override string Contains(MethodCallExpressionModel model)
        {
            var item = model.Args.First().MemberValue;
            BsonValue right = new ExpressionVisitor(context).Visit(item as Expression);
            BsonValue left = new ExpressionVisitor(context).Visit(model.DataObject as Expression);
            // 构造 $regex 匹配
            var regexDoc = new BsonDocument
            {
                { "$regex", right },        // right 是普通字符串值，例如 "a"
                { "$options", "i" }         // 忽略大小写
            }; 
            var match = new BsonDocument("$match", new BsonDocument
                {
                    { left.ToString(), regexDoc }
                });
            return match.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string StartsWith(MethodCallExpressionModel model)
        {
            var item = model.Args.First().MemberValue;
            BsonValue right = new ExpressionVisitor(context).Visit(item as Expression);
            BsonValue left = new ExpressionVisitor(context).Visit(model.DataObject as Expression);
            // 构造 $regex 匹配，^ 表示以...开头
            var regexPattern = "^" + right.ToString();
            var regexDoc = new BsonDocument
            {
                { "$regex", regexPattern },
                { "$options", "i" }
            };
            var match = new BsonDocument("$match", new BsonDocument
                {
                    { left.ToString(), regexDoc }
                });
            return match.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string EndsWith(MethodCallExpressionModel model)
        {
            var item = model.Args.First().MemberValue;
            BsonValue right = new ExpressionVisitor(context).Visit(item as Expression);
            BsonValue left = new ExpressionVisitor(context).Visit(model.DataObject as Expression);
            // 构造 $regex 匹配，$ 表示以...结尾
            var regexPattern = right.ToString() + "$";
            var regexDoc = new BsonDocument
            {
                { "$regex", regexPattern },
                { "$options", "i" }
            };
            var match = new BsonDocument("$match", new BsonDocument
                {
                    { left.ToString(), regexDoc }
                });
            return match.ToJson(UtilMethods.GetJsonWriterSettings());
        } 
        public override string ToDateShort(MethodCallExpressionModel model)
        {
            var item = model.Args.First().MemberValue;
            BsonValue itemValue = new ExpressionVisitor(context).Visit(item as Expression);
            var dateTruncDoc = new BsonDocument("$dateTrunc", new BsonDocument
            {
                { "date", $"${itemValue}" },
                { "unit", "day" }
            });
            return dateTruncDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string DateAddByType(MethodCallExpressionModel model)
        {  
            var dateExpr = model.DataObject;
            var numberExpr = model.Args[0].MemberValue;
            var typeExpr = model.Args[1].MemberValue; 
            BsonValue dateValue = new ExpressionVisitor(context).Visit(dateExpr as Expression);
            BsonValue numberValue = new ExpressionVisitor(context).Visit(numberExpr as Expression);
            var dateType = (DateType)typeExpr;

            string unit = dateType switch
            {
                DateType.Year => "year",
                DateType.Month => "month",
                DateType.Day => "day",
                DateType.Hour => "hour",
                DateType.Minute => "minute",
                DateType.Second => "second",
                DateType.Millisecond => "millisecond",
                _ => throw new NotSupportedException($"不支持的DateType: {dateType}")
            };

            var dateAddDoc = new BsonDocument("$dateAdd", new BsonDocument
            {
                { "startDate", $"${dateValue}" },
                { "unit", unit },
                { "amount", numberValue }
            });

            return dateAddDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string DateValue(MethodCallExpressionModel model)
        {
            var item = model.Args.First().MemberValue;
            BsonValue itemValue = new ExpressionVisitor(context).Visit(item as Expression);
            var dateType = (DateType)model.Args.Last().MemberValue;
            switch (dateType)
            {
                case DateType.Year:
                    // MongoDB $year 操作符
                    return new BsonDocument("$year", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Month:
                    return new BsonDocument("$month", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Day:
                    return new BsonDocument("$dayOfMonth", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Hour:
                    return new BsonDocument("$hour", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Minute:
                    return new BsonDocument("$minute", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Second:
                    return new BsonDocument("$second", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Millisecond:
                    return new BsonDocument("$millisecond", $"${itemValue}").ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Weekday:
                    return new BsonDocument("$subtract", new BsonArray
                    {
                        new BsonDocument("$dayOfWeek", $"${itemValue}"),
                        1
                    }).ToJson(UtilMethods.GetJsonWriterSettings());
                case DateType.Quarter:
                    // MongoDB 没有直接的quarter操作符，需自定义表达式
                    var expr = new BsonDocument("$add", new BsonArray
                    {
                        new BsonDocument("$divide", new BsonArray
                        {
                            new BsonDocument("$subtract", new BsonArray
                            {
                                new BsonDocument("$month", $"${itemValue}"),
                                1
                            }),
                            3
                        }),
                        1
                    });
             return expr.ToJson(UtilMethods.GetJsonWriterSettings()); 
            }
            return null;
        } 
        public override string ToString(MethodCallExpressionModel model)
        {
            var item = model.DataObject as Expression;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);

            if (model.Args == null || model.Args.Count == 0)
            {
                // 只有 ToString()，直接转字符串
                var toStringDoc = new BsonDocument("$toString", $"${memberName}");
                return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
            }
            else if (model.Args.Count == 1)
            {
                var format = (model.Args.First().MemberValue).ObjToString().TrimStart('"').TrimEnd('"');
                // 先判断类型
                var type = (item as MemberExpression)?.Type ?? (item as UnaryExpression)?.Type;
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    // C#格式转MongoDB格式
                    string mongoFormat = ConvertCSharpDateFormatToMongo(format);
                    var dateToStringDoc = new BsonDocument("$dateToString", new BsonDocument
                    {
                        { "format", mongoFormat },
                        { "date", $"${memberName}" }
                    });
                    return dateToStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
                }
                else if (UtilConstants.NumericalTypes.Contains(type))
                {
                    // 数字格式化，MongoDB不支持C#的数字格式，需要先转字符串再在C#端格式化
                    // 这里只能简单转字符串
                    var toStringDoc = new BsonDocument("$toString", $"${memberName}");
                    return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
                }
                else
                {
                    // 其他类型直接转字符串
                    var toStringDoc = new BsonDocument("$toString", $"${memberName}");
                    return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
                }
            }
            else
            {
                throw new NotSupportedException("ToString 只支持0或1个参数");
            }
        }
        public override string ToUpper(MethodCallExpressionModel model)
        {
            var item = model.DataObject as Expression;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toUpperDoc = new BsonDocument("$toUpper", $"${memberName}");
            return toUpperDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string ToLower(MethodCallExpressionModel model)
        {
            var item = model.DataObject as Expression;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toLowerDoc = new BsonDocument("$toLower", $"${memberName}");
            return toLowerDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        #region

        // Existing methods...

        /// <summary>
        /// Converts a C# date format string to a MongoDB-compatible date format string.
        /// </summary>
        /// <param name="csharpFormat">The C# date format string.</param>
        /// <returns>The MongoDB-compatible date format string.</returns>
        public string ConvertCSharpDateFormatToMongo(string csharpFormat)
        {
            if (string.IsNullOrEmpty(csharpFormat))
            {
                throw new ArgumentNullException(nameof(csharpFormat), "Date format cannot be null or empty.");
            }

            // Replace C# date format specifiers with MongoDB equivalents
            return csharpFormat
                .Replace("yyyy", "%Y")
                .Replace("MM", "%m")
                .Replace("dd", "%d")
                .Replace("HH", "%H")
                .Replace("mm", "%M")
                .Replace("ss", "%S")
                .Replace("fff", "%L"); // Milliseconds
        }
        #endregion
    }
}
