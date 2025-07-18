using MongoDB.Bson.IO;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
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
            var shellString = sql.ToJson(UtilMethods.GetJsonWriterSettings()); 
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
            var toIntDoc = new BsonDocument("$toInt", GetMemberName(memberName));
            return toIntDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToInt64(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toLongDoc = new BsonDocument("$toLong", GetMemberName(memberName));
            return toLongDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToGuid(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            // MongoDB 没有直接的 Guid 类型，通常以字符串存储
            var toStringDoc = new BsonDocument("$toString", GetMemberName(memberName));
            return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToDouble(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toDoubleDoc = new BsonDocument("$toDouble", GetMemberName(memberName));
            return toDoubleDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToBool(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toBoolDoc = new BsonDocument("$toBool", GetMemberName(memberName));
            return toBoolDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToDate(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toDateDoc = new BsonDocument("$toDate", GetMemberName(memberName));
            return toDateDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ToTime(MethodCallExpressionModel model)
        {
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            // MongoDB 没有单独的 Time 类型，通常用字符串或日期处理
            var toStringDoc = new BsonDocument("$toString", GetMemberName(memberName));
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
            var match=new BsonDocument
                {
                    { left.ToString(), regexDoc }
                };
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
            var match =new BsonDocument
                {
                    { left.ToString(), regexDoc }
                };
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
            var match = new BsonDocument
                {
                    { left.ToString(), regexDoc }
                };
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
                var toStringDoc = new BsonDocument("$toString", GetMemberName(memberName));
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
                        { "date", GetMemberName(memberName) }
                    });
                    return dateToStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
                }
                else if (UtilConstants.NumericalTypes.Contains(type))
                {
                    // 数字格式化，MongoDB不支持C#的数字格式，需要先转字符串再在C#端格式化
                    // 这里只能简单转字符串
                    var toStringDoc = new BsonDocument("$toString", GetMemberName(memberName));
                    return toStringDoc.ToJson(UtilMethods.GetJsonWriterSettings());
                }
                else
                {
                    // 其他类型直接转字符串
                    var toStringDoc = new BsonDocument("$toString", GetMemberName(memberName));
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
            var toUpperDoc = new BsonDocument("$toUpper", GetMemberName(memberName));
            return toUpperDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string ToLower(MethodCallExpressionModel model)
        {
            var item = model.DataObject as Expression;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var toLowerDoc = new BsonDocument("$toLower", GetMemberName(memberName));
            return toLowerDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
        public override string Abs(MethodCallExpressionModel model)
        {
            // 取绝对值，MongoDB $abs 操作符
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var absDoc = new BsonDocument("$abs", GetMemberName(memberName));
            return absDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string Round(MethodCallExpressionModel model)
        {
            // 四舍五入，MongoDB $round 操作符
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var roundDoc = new BsonDocument("$round", new BsonArray { GetMemberName(memberName) });
            return roundDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

      public override string Floor(MethodCallExpressionModel model)
        {
            // 向下取整，MongoDB $floor 操作符
            var item = model.Args[0].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var floorDoc = new BsonDocument("$floor", GetMemberName(memberName));
            return floorDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string Substring(MethodCallExpressionModel model)
        {
            // 截取字符串，MongoDB $substr 操作符（推荐 $substrBytes）
            var item = model.DataObject as Expression;
            var start = model.Args[0].MemberValue;
            var length = model.Args[1].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item);
            BsonValue startValue = new ExpressionVisitor(context).Visit(start as Expression);
            BsonValue lengthValue = new ExpressionVisitor(context).Visit(length as Expression);
            var substrDoc = new BsonDocument("$substrBytes", new BsonArray { GetMemberName(memberName), startValue, lengthValue });
            return substrDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string Replace(MethodCallExpressionModel model)
        {
            // 替换字符串，MongoDB $replaceAll 操作符
            var item = model.DataObject as Expression;
            var find = model.Args[0].MemberValue;
            var replacement = model.Args[1].MemberValue;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item);
            BsonValue findValue = new ExpressionVisitor(context).Visit(find as Expression);
            BsonValue replacementValue = new ExpressionVisitor(context).Visit(replacement as Expression);
            var replaceDoc = new BsonDocument("$replaceAll", new BsonDocument
            {
                { "input", GetMemberName(memberName) },
                { "find", findValue },
                { "replacement", replacementValue }
            });
            return replaceDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string Length(MethodCallExpressionModel model)
        {
            // 字符串长度，MongoDB $strLenBytes 操作符
            var item = model.Args[0].MemberValue ?? model.DataObject;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var lengthDoc = new BsonDocument("$strLenBytes", GetMemberName(memberName));
            return lengthDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }
         

        public override string IsNullOrEmpty(MethodCallExpressionModel model)
        {
            // 获取字段名表达式
            var item = model.Args[0].MemberValue ?? model.DataObject;
            var fieldExpr = new ExpressionVisitor(context).Visit(item as Expression);

            string fieldName = fieldExpr.ToString();

            // 生成：{ "$match": { "$or": [ { "Name": null }, { "Name": "" } ] } }
            var or=new BsonDocument(
                "$or", new BsonArray
                {
            new BsonDocument(fieldName, BsonNull.Value),
            new BsonDocument(fieldName, "")
                });

            return or.ToJson(UtilMethods.GetJsonWriterSettings());
        }
         
        public override string Trim(MethodCallExpressionModel model)
        {
            // 去除首尾空格，MongoDB $trim 操作符
            var item = model.Args[0].MemberValue ?? model.DataObject;
            BsonValue memberName = new ExpressionVisitor(context).Visit(item as Expression);
            var trimDoc = new BsonDocument("$trim", new BsonDocument { { "input", GetMemberName(memberName) } });
            return trimDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ContainsArray(MethodCallExpressionModel model)
        {
            // 解析数组表达式和待判断的元素表达式
            var arrayExp = model.DataObject as Expression; 
            var itemExp = model.Args[0].MemberValue as Expression;
            if (arrayExp == null)
            {
                arrayExp = model.Args[0].MemberValue as Expression;
                itemExp = model.Args[1].MemberValue as Expression;
            }

            // 获取字段名
            BsonValue fieldName = new ExpressionVisitor(context).Visit(itemExp);

            // 获取数组值
            var arrayObj = ExpressionTool.DynamicInvoke(arrayExp) as IEnumerable;
            if (arrayObj == null)
                return null;
           var name=fieldName.ToString();
            // 构建BsonArray
            var bsonArray = new BsonArray();
            foreach (var val in arrayObj)
            {
                if (val == null)
                    bsonArray.Add(BsonNull.Value);
                else if (name == "_id") 
                {
                    var value = val?.ToString();
                    if (UtilMethods.IsValidObjectId(value))
                    {
                        bsonArray.Add(BsonValue.Create(ObjectId.Parse(value)));
                    }
                    else 
                    {
                        bsonArray.Add(BsonValue.Create(val));
                    }
                }
                else
                    bsonArray.Add(BsonValue.Create(val));
            }

            // 构建MongoDB的 $in 查询表达式
            var inDoc = new BsonDocument(name, new BsonDocument("$in", bsonArray));
            return inDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string ContainsArrayUseSqlParameters(MethodCallExpressionModel model)
        {
            return this.ContainsArray(model);
        }

        public override string JsonArrayAny(MethodCallExpressionModel model)
        {
            // 伪代码步骤：
            // 1. 获取数组字段表达式 arrayField
            // 2. 获取要判断的元素表达式 pars
            // 3. 解析字段名和元素值
            // 4. 构造 MongoDB $in 查询表达式：{ arrayField: { $in: [element] } }
            // 5. 返回 JSON 字符串

            var arrayFieldExpr = model.DataObject as Expression;
            var elementExpr = model.Args[0].MemberValue as Expression;

            // 获取字段名
            BsonValue fieldName = new ExpressionVisitor(context).Visit(arrayFieldExpr);
            // 获取元素值
            var elementValue = ExpressionTool.DynamicInvoke(elementExpr);
            if (elementValue is string s&&UtilMethods.IsValidObjectId(s)) 
            {
                elementValue = ObjectId.Parse(s);
            }
            // 构造 $in 查询表达式
            var inDoc = new BsonDocument(fieldName.ToString(), new BsonDocument("$in", new BsonArray { BsonValue.Create(elementValue) }));
            return inDoc.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        public override string IIF(MethodCallExpressionModel model)
        { 

            var test = model.Args[0].MemberValue as Expression;
            var ifTrue = model.Args[1].MemberValue as Expression;
            var ifFalse = model.Args[2].MemberValue as Expression;

            // 构造 ConditionalExpression
            var conditionalExpr = Expression.Condition(test, ifTrue, ifFalse);

            BsonValue testValue = new ConditionalExpressionTractor(context, null).Extract(conditionalExpr);
            return testValue.ToJson(UtilMethods.GetJsonWriterSettings());
        }

        #region  Helper 
        private static BsonValue GetMemberName(BsonValue memberName)
        {
            return UtilMethods.GetMemberName(memberName);
        }

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
