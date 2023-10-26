using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        #region Core
        public KeyValuePair<string, SugarParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0)
        {
            if (models.IsNullOrEmpty()) return new KeyValuePair<string, SugarParameter[]>();
            StringBuilder builder = new StringBuilder();
            List<SugarParameter> parameters = new List<SugarParameter>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            var mainIndex = 0;
            var indexTree = 0;
            foreach (var model in models)
            {
                if (model is ConditionalModel)
                {
                    var item = model as ConditionalModel;
                    if (item.CustomConditionalFunc != null)
                    {
                        var colIndex = mainIndex + beginIndex;
                        var colType = colIndex == 0 ? "" : "AND";
                        var custom = item.CustomConditionalFunc.GetConditionalSql(item, colIndex);
                        parameters.AddRange(custom.Value);
                        builder.AppendFormat(" " + colType + " " + custom.Key);
                        mainIndex++;
                        continue;
                    }
                    else if (item.FieldName == UtilMethods.FiledNameSql())
                    {
                        builder.Append(item.FieldValue);
                        continue;
                    }
                    var index = mainIndex + beginIndex;
                    var type = index == 0 ? "" : "AND";
                    if (beginIndex > 0)
                    {
                        type = null;
                    }
                    string temp = " {0} {1} {2} {3}  ";
                    string parameterName = string.Format("{0}Condit{1}{2}", sqlBuilder.SqlParameterKeyWord, item.FieldName, index);
                    if (parameterName.Contains("."))
                    {
                        parameterName = parameterName.Replace(".", "_");
                    }
                    if (parameterName.Contains("["))
                    {
                        parameterName = parameterName.Replace("[", "_");
                    }
                    if (parameterName.Contains("]"))
                    {
                        parameterName = parameterName.Replace("]", "_");
                    }
                    if (!string.IsNullOrEmpty(this.SqlTranslationLeft) && parameterName.Contains(this.SqlTranslationLeft))
                    {
                        parameterName = parameterName.Replace(this.SqlTranslationLeft, "_");
                    }
                    string oldName = item.FieldName;
                    item.FieldName = GetTranslationColumnName(item.FieldName);
                    item.FieldName = item.FieldName.ToCheckField();
                    switch (item.ConditionalType)
                    {
                        case ConditionalType.Equal:
                            Equal(builder, parameters, item, type, temp, parameterName);
                            break;
                        case ConditionalType.Like:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, "%" + item.FieldValue + "%"));
                            break;
                        case ConditionalType.GreaterThan:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            break;
                        case ConditionalType.GreaterThanOrEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">=", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            break;
                        case ConditionalType.LessThan:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            break;
                        case ConditionalType.LessThanOrEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<=", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            break;
                        case ConditionalType.In:
                            In(builder, item, type, temp);
                            //parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.NotIn:
                            NotIn(builder, parameters, item, type, temp, parameterName);
                            break;
                        case ConditionalType.LikeLeft:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue + "%"));
                            break;
                        case ConditionalType.NoLike:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " NOT LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, "%" + item.FieldValue + "%"));
                            break;
                        case ConditionalType.LikeRight:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, "%" + item.FieldValue));
                            break;
                        case ConditionalType.NoEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            break;
                        case ConditionalType.IsNullOrEmpty:
                            builder.AppendFormat(" {0} (({1}) OR ({2})) ", type, item.FieldName.ToSqlFilter() + " IS NULL ", item.FieldName.ToSqlFilter() + " = '' ");
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.IsNot:
                            IsNot(builder, parameters, item, type, temp, parameterName);
                            break;
                        case ConditionalType.EqualNull:
                            EqualNull(builder, parameters, item, type, temp, parameterName);
                            break;
                        case ConditionalType.InLike:
                            InLike(builder, parameters, item, index, type, parameterName);
                            break;
                        default:
                            break;
                    }
                    item.FieldName = oldName;
                }
                else if (model is ConditionalCollections)
                {
                    var item = model as ConditionalCollections;
                    if (item != null && item.ConditionalList.HasValue())
                    {
                        foreach (var con in item.ConditionalList)
                        {
                            var index = item.ConditionalList.IndexOf(con);
                            var isFirst = index == 0;
                            var isLast = index == (item.ConditionalList.Count - 1);
                            if (models.IndexOf(item) == 0 && index == 0 && beginIndex == 0)
                            {
                                builder.AppendFormat(" ( ");

                            }
                            else if (isFirst)
                            {
                                builder.AppendFormat(" {0} ( ", con.Key.ToString().ToUpper());
                            }
                            List<IConditionalModel> conModels = new List<IConditionalModel>();
                            conModels.Add(con.Value);
                            var childSqlInfo = ConditionalModelToSql(conModels, 1000 * (1 + index) + models.IndexOf(item));
                            if (!isFirst && con.Value.FieldName != $"[value=sql{UtilConstants.ReplaceKey}]")
                            {

                                builder.AppendFormat(" {0} ", con.Key.ToString().ToUpper());
                            }
                            builder.Append(childSqlInfo.Key);
                            parameters.AddRange(childSqlInfo.Value);
                            if (isLast)
                            {
                                builder.Append(" ) ");
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {
                    var item = model as ConditionalTree;
                    BuilderTree(builder, item, ref indexTree, parameters, ref mainIndex);
                }
                mainIndex++;
            }
            return new KeyValuePair<string, SugarParameter[]>(builder.ToString(), parameters.ToArray());
        }

        #endregion

        #region  Case Method
        private static void InLike(StringBuilder builder, List<SugarParameter> parameters, ConditionalModel item, int index, string type, string parameterName)
        {
            var array = (item.FieldValue + "").Split(',').ToList();
            List<string> sqls = new List<string>();
            int i = 0;
            foreach (var val in array)
            {
                var itemParameterName = $"{parameterName}{index}{i}";
                sqls.Add(item.FieldName.ToSqlFilter() + " LIKE " + itemParameterName);
                parameters.Add(new SugarParameter(itemParameterName, "%" + val + "%"));
                i++;
            }
            builder.Append($" {type} ({string.Join(" OR ", sqls)}) ");
        }

        private static void EqualNull(StringBuilder builder, List<SugarParameter> parameters, ConditionalModel item, string type, string temp, string parameterName)
        {
            if (GetFieldValue(item) == null)
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "  IS ", " NULL ");
            }
            else
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
                parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
            }
        }

        private static void IsNot(StringBuilder builder, List<SugarParameter> parameters, ConditionalModel item, string type, string temp, string parameterName)
        {
            if (item.FieldValue == null)
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " IS NOT ", "NULL");
            }
            else
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
                parameters.Add(new SugarParameter(parameterName, item.FieldValue));
            }
        }

        private static void NotIn(StringBuilder builder, List<SugarParameter> parameters, ConditionalModel item, string type, string temp, string parameterName)
        {
            if (item.FieldValue == null) item.FieldValue = string.Empty;
            var inValue2 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
            if (item.CSharpTypeName.HasValue() && UtilMethods.IsNumber(item.CSharpTypeName))
            {
                inValue2 = inValue2.Replace("'", "");
            }
            else if (inValue2.Contains("'null'"))
            {
                inValue2 = inValue2.Replace("'null'", "null");
            }
            else if (inValue2.Contains("[null]"))
            {
                inValue2 = inValue2.Replace("[null]", "null");
            }
            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "NOT IN", inValue2);
            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
        }

        private  void In(StringBuilder builder, ConditionalModel item, string type, string temp)
        {
            if (item.FieldValue == null) item.FieldValue = string.Empty;
            var inValue1 = string.Empty;
            var inArray=item.FieldValue.Split(',');
            var pageSize = 1000;
            if (inArray.Length > pageSize&&this.Context.CurrentConnectionConfig.DbType==DbType.Oracle) 
            {
                InBig(builder,item,type,temp,inArray, pageSize);
                return;
            }
            inValue1 = In_GetInValue(item, inArray);
            if (item.CSharpTypeName.HasValue() && UtilMethods.IsNumber(item.CSharpTypeName))
            {
                inValue1 = inValue1.Replace("'", "");
            }
            else if (inValue1.Contains("'null'"))
            {
                inValue1 = inValue1.Replace("'null'", "null");
            }
            else if (inValue1.Contains("[comma]")) 
            {
                inValue1 = inValue1.Replace("[comma]", ",");
            }
            else if (inValue1.Contains("[null]"))
            {
                inValue1 = inValue1.Replace("[null]", "null");
            }
            if (item.CSharpTypeName.EqualCase("guid") && inValue1 == "('')")
            {
                inValue1 = $"('{Guid.Empty.ToString()}')";
            }
            else if (inValue1 == "()")
            {
                inValue1 = $"(NULL)";
            }
            if (inArray.Length == 1)
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", inValue1.TrimStart('(').TrimEnd(')'));
            }
            else
            {
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "IN", inValue1);
            }
        }

        private  void InBig(StringBuilder builder, ConditionalModel item, string type, string temp, string[] inArray, int pageSize)
        {
            var sqlList = new List<string>();
            this.Context.Utilities.PageEach(inArray, pageSize, items =>
            {
                if (item.CSharpTypeName.EqualCase("string") || item.CSharpTypeName == null)
                {
                    sqlList.Add("(" + item.FieldName.ToSqlFilter() + " IN (" + items.Distinct().ToArray().ToJoinSqlInVals() + "))");
                }
                else
                {
                    sqlList.Add("(" + item.FieldName.ToSqlFilter() + " IN (" + items.Select(it => it == "" ? "null" : it).Distinct().ToArray().ToJoinSqlInVals() + "))");
                }
            });
            var inValue1 = $" {string.Join(" OR ",sqlList)} ";
            if (item.CSharpTypeName.HasValue() && UtilMethods.IsNumber(item.CSharpTypeName))
            {
                inValue1 = inValue1.Replace("'", "");
            }
            builder.AppendFormat(temp, type, "", " ", inValue1);
        }

        private static string In_GetInValue(ConditionalModel item,string[] inArray)
        {
            string inValue1;
            if (item.CSharpTypeName.EqualCase("string") || item.CSharpTypeName == null)
            {
                inValue1 = ("(" + inArray.Distinct().ToArray().ToJoinSqlInVals() + ")");
            }
            else
            {
                inValue1 = ("(" + inArray.Select(it => it == "" ? "null" : it).Distinct().ToArray().ToJoinSqlInVals() + ")");
            }

            return inValue1;
        }

        private static void Equal(StringBuilder builder, List<SugarParameter> parameters, ConditionalModel item, string type, string temp, string parameterName)
        {
            if (item.FieldValue != null && item.FieldValue == "null" && item.FieldValue != "[null]")
            {
                builder.AppendFormat($" {item.FieldName.ToSqlFilter()} is null ");
            }
            else
            {
                if (item.FieldValue == "[null]")
                {
                    item.FieldValue = "null";
                }
                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
                parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
            }
        } 
        #endregion

        #region ConditionalCollections
        private void BuilderTree(StringBuilder builder, ConditionalTree item, ref int indexTree, List<SugarParameter> parameters, ref int mainIndex)
        {
            var conditionals = ToConditionalCollections(item, ref indexTree, parameters);
            var sqlobj = ConditionalModelToSql(new List<IConditionalModel> { conditionals }, mainIndex);
            var sql = sqlobj.Key;
            RepairReplicationParameters(ref sql, sqlobj.Value, indexTree);
            parameters.AddRange(sqlobj.Value);
            var buiderSql = sql;
            builder.Append(buiderSql);
            indexTree++;
        }
        private ConditionalCollections ToConditionalCollections(ConditionalTree item, ref int indexTree, List<SugarParameter> parameters)
        {
            List<KeyValuePair<WhereType, ConditionalModel>> list = new List<KeyValuePair<WhereType, ConditionalModel>>();
            var index = 0;
            foreach (var it in item.ConditionalList)
            {
                ConditionalModel model = new ConditionalModel();
                if (it.Value is ConditionalModel)
                {
                    model = (ConditionalModel)it.Value;
                }
                else
                {
                    var tree = it.Value as ConditionalTree;
                    var con = ToConditionalCollections(tree, ref indexTree, parameters);
                    var sqlobj = ConditionalModelToSql(new List<IConditionalModel> { con }, index);
                    var sql = sqlobj.Key;
                    if (sql.StartsWith(" NULL "))
                    {
                        sql = Regex.Replace(sql, "^ NULL ", it.Key.ToString().ToUpper());
                    }
                    RepairReplicationParameters(ref sql, sqlobj.Value, indexTree);
                    model = new ConditionalModel()
                    {
                        FieldName = $"[value=sql{UtilConstants.ReplaceKey}]",
                        FieldValue = sql
                    };
                    parameters.AddRange(sqlobj.Value);
                    indexTree++;
                }
                list.Add(new KeyValuePair<WhereType, ConditionalModel>(it.Key, model));
                index++;
            }
            var result = new ConditionalCollections()
            {
                ConditionalList = list
            };
            return result;
        } 
        #endregion

    }
}
