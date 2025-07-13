using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar.MongoDb
{
    public class MongoDbBuilder : SqlBuilderProvider
    {
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
        public override string SqlDateNow
        {
            get
            {
                return "current_timestamp";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select current_timestamp";
            }
        }

        public bool isAutoToLower
        {
            get
            {
                return false;
            }
        }
        public override string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains(".")&& !propertyName.Contains(SqlTranslationLeft)) 
            {
                return string.Join(".", propertyName.Split('.').Select(it => $"{SqlTranslationLeft}{it.ToLower(isAutoToLower)}{SqlTranslationRight}"));
            }

            if (propertyName.Contains(SqlTranslationLeft)) return propertyName;
            else
                return SqlTranslationLeft + propertyName.ToLower(isAutoToLower) + SqlTranslationRight;
        }

        //public override string GetNoTranslationColumnName(string name)
        //{
        //    return name.TrimEnd(Convert.ToChar(SqlTranslationRight)).TrimStart(Convert.ToChar(SqlTranslationLeft)).ToLower();
        //}
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName.ToLower(isAutoToLower) + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName.ToLower(isAutoToLower) + SqlTranslationRight);
        }

        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            var context = this.Context;

            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (mappingInfo == null && name.Contains(".") && name.Contains("\"")) 
            {
                return name;
            }
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.Contains(".")&& !name.Contains("(")&&!name.Contains("\".\""))
            {
                return string.Join(".", name.ToLower(isAutoToLower).Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else if (name.Contains("("))
            {
                return name;
            }
            else if (name.Contains(SqlTranslationLeft) && name.Contains(SqlTranslationRight))
            {
                return name;
            }
            else
            {
                return name;
            }
        }
        public override string GetUnionFomatSql(string sql)
        {
            return " ( " + sql + " )  ";
        }

        public override Type GetNullType(string tableName, string columnName) 
        {
            if (tableName != null)
                tableName = tableName.Trim();
            var columnInfo=this.Context.DbMaintenance.GetColumnInfosByTableName(tableName).FirstOrDefault(z => z.DbColumnName?.ToLower()==columnName?.ToLower());
            if (columnInfo != null) 
            {
                var cTypeName=this.Context.Ado.DbBind.GetCsharpTypeNameByDbTypeName(columnInfo.DataType);
                var value=SqlSugar.UtilMethods.GetTypeByTypeName(cTypeName);
                if (value != null) 
                {
                    var key = "GetNullType_" + tableName + columnName;
                    return new ReflectionInoCacheService().GetOrCreate(key, () => value);
                }
            }
            return null;
        }

        public override KeyValuePair<string, SugarParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0)
        {
            // MongoDB的查询条件通常以JSON格式表达，不需要参数化
            if (models == null || models.Count == 0)
                return new KeyValuePair<string, SugarParameter[]>("{}", Array.Empty<SugarParameter>());

            if (models.Count == 1 && models.First() is ConditionalCollections collections)
            {
                return ConditionalModelToSqlByConditionalCollections(collections);
            }

            var filterParts = new List<string>();
            foreach (var model in models)
            {
                if (model is ConditionalModel cond)
                {
                    string field = cond.FieldName;
                    string op = cond.ConditionalType + "";
                    string fieldValue = cond.FieldValue;
                    object value= SqlSugar.UtilMethods.ConvertDataByTypeName(cond.CSharpTypeName, fieldValue);
                    switch (op)
                    {
                        case "Equal":
                            filterParts.Add($"\"{field}\": {ToMongoValue(value)}");
                            break;
                        case "NotEqual":
                            filterParts.Add($"\"{field}\": {{ \"$ne\": {ToMongoValue(value)} }}");
                            break;
                        case "GreaterThan":
                            filterParts.Add($"\"{field}\": {{ \"$gt\": {ToMongoValue(value)} }}");
                            break;
                        case "GreaterThanOrEqual":
                            filterParts.Add($"\"{field}\": {{ \"$gte\": {ToMongoValue(value)} }}");
                            break;
                        case "LessThan":
                            filterParts.Add($"\"{field}\": {{ \"$lt\": {ToMongoValue(value)} }}");
                            break;
                        case "LessThanOrEqual":
                            filterParts.Add($"\"{field}\": {{ \"$lte\": {ToMongoValue(value)} }}");
                            break;
                        case "In":
                            filterParts.Add($"\"{field}\": {{ \"$in\": {ToMongoArray(value)} }}");
                            break;
                        case "Like":
                            // MongoDB的模糊查询用正则表达式
                            filterParts.Add($"\"{field}\": {{ \"$regex\": {ToMongoRegex(value)}, \"$options\": \"i\" }}");
                            break;
                        case "IsNull":
                            filterParts.Add($"\"{field}\": null");
                            break;
                        case "IsNotNull":
                            filterParts.Add($"\"{field}\": {{ \"$ne\": null }}");
                            break;
                        default:
                            // 其他操作符可扩展
                            break;
                    }
                }
                // 其他类型的IConditionalModel可扩展
            }

            string filter = "{ " + string.Join(", ", filterParts) + " }";
            return new KeyValuePair<string, SugarParameter[]>(filter, Array.Empty<SugarParameter>());

            // 辅助方法：将C#对象转换为MongoDB JSON值
            static string ToMongoValue(object value)
            {
                if (value == null) return "null";
                if (value is string s) return $"\"{EscapeString(s)}\"";
                if (value is bool b) return b ? "true" : "false";
                if (value is DateTime dt) return $"{{ \"$date\": \"{dt:O}\" }}";
                if (value is Enum) return Convert.ToInt32(value).ToString();
                if (value is IEnumerable<object> arr) return ToMongoArray(arr);
                if (value is IEnumerable<string> arrStr) return ToMongoArray(arrStr);
                return value.ToString();
            }

            static string ToMongoArray(object value)
            {
                if (value is System.Collections.IEnumerable enumerable && !(value is string))
                {
                    var items = new List<string>();
                    foreach (var item in enumerable)
                    {
                        items.Add(ToMongoValue(item));
                    }
                    return "[ " + string.Join(", ", items) + " ]";
                }
                return "[ " + ToMongoValue(value) + " ]";
            }

            static string ToMongoRegex(object value)
            {
                if (value == null) return "\"\"";
                return $"\"{EscapeString(value.ToString())}\"";
            }

            static string EscapeString(string s)
            {
                return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
            }
        }

        private KeyValuePair<string, SugarParameter[]> ConditionalModelToSqlByConditionalCollections(ConditionalCollections collections)
        {
            // collections.ConditionalList: List<KeyValuePair<string, List<IConditionalModel>>>，每个子List是一个分组，Key为分组的WhereType
            var groupFilters = new List<string>();
            var allParameters = new List<SugarParameter>();
            int paramIndex = 0;

            foreach (var group in collections.ConditionalList)
            {
                var result = this.ConditionalModelToSql(new List<IConditionalModel>() { group.Value }, paramIndex);
                string filter = result.Key;
                SugarParameter[] parameters = result.Value;

                if (!string.IsNullOrWhiteSpace(filter) && filter != "{}")
                {
                    string innerFilter = filter.Trim();
                    if (innerFilter.StartsWith("{") && innerFilter.EndsWith("}"))
                    {
                        innerFilter = innerFilter.Substring(1, innerFilter.Length - 2).Trim();
                    }
                    if (!string.IsNullOrEmpty(innerFilter))
                    {
                        string groupOperator = group.Key == WhereType.Or ? "$or" : "$and";
                        groupFilters.Add($"\"{groupOperator}\": [{{ {innerFilter} }} ]");
                        allParameters.AddRange(parameters);
                        paramIndex += parameters.Length;
                    }
                }
            }
            string finalFilter;
            if (groupFilters.Count == 1)
            {
                finalFilter = "{ " + groupFilters[0] + " }";
            }
            else
            {
                finalFilter = "[ " + string.Join(", ", groupFilters.Select(f => "{ " + f + " }")) + " ] ";
            }
            return new KeyValuePair<string, SugarParameter[]>(finalFilter, allParameters.ToArray());
        }
    }
}
