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
        #region Properties
        public SqlSugarProvider Context { get; set; }
        public CommandType CommandType { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public SqlQueryBuilder SqlQueryBuilder
        {
            get
            {
                base._SqlQueryBuilder = UtilMethods.IsNullReturnNew(base._SqlQueryBuilder);
                return base._SqlQueryBuilder;
            }
            set { base._SqlQueryBuilder = value; }
        }
        #endregion

        #region abstract Methods
        public virtual string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (name.IsContainsIn("(", ")", SqlTranslationLeft)&&name!= "Dictionary`2")
            {
                return name;
            }
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.IsContainsIn("(", ")", SqlTranslationLeft))
            {
                return name;
            }
            if (name.Contains("."))
            {
                return string.Join(".", name.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else
            {
                return SqlTranslationLeft + name + SqlTranslationRight;
            }
        }
        public virtual string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName + SqlTranslationRight);
        }

        public virtual string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains(SqlTranslationLeft)) return propertyName;
            else
                return SqlTranslationLeft + propertyName + SqlTranslationRight;
        }

        public virtual string GetNoTranslationColumnName(string name)
        {
            if (name.Contains("="))
            {
               name=name.Split('=').First();
            }
            if (!name.Contains(SqlTranslationLeft)) return name;
            return name == null ? string.Empty : Regex.Match(name, @".*" + "\\" + SqlTranslationLeft + "(.*?)" + "\\" + SqlTranslationRight + "").Groups[1].Value;
        }
        public virtual string GetPackTable(string sql, string shortName)
        {
            return UtilMethods.GetPackTable(sql, shortName);
        }
        public virtual string GetDefaultShortName()
        {
            return "t";
        }


        public string GetWhere(string fieldName,string conditionalType,int? parameterIndex=null)
        {
            return string.Format(" {0} {1} {2}{3} ",fieldName,conditionalType,this.SqlParameterKeyWord,fieldName+ parameterIndex);
        }
        public virtual string GetUnionAllSql(List<string> sqlList)
        {
            return string.Join(" UNION ALL \r\n", sqlList);
        }
        public virtual string GetUnionSql(List<string> sqlList)
        {
            return string.Join(" UNION \r\n", sqlList);
        }
        public virtual void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex)
        {
            UtilMethods.RepairReplicationParameters(ref appendSql, parameters, addIndex);
        }
        public KeyValuePair<string, SugarParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0)
        {
            if (models.IsNullOrEmpty()) return new KeyValuePair<string, SugarParameter[]>();
            StringBuilder builder = new StringBuilder();
            List<SugarParameter> parameters = new List<SugarParameter>();
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig);
            foreach (var model in models)
            {
                if (model is ConditionalModel)
                {
                    var item = model as ConditionalModel;
                    var index = models.IndexOf(item) + beginIndex;
                    var type = index == 0 ? "" : "AND";
                    if (beginIndex > 0)
                    {
                        type = null;
                    }
                    string temp = " {0} {1} {2} {3}  ";
                    string parameterName = string.Format("{0}Conditional{1}{2}", sqlBuilder.SqlParameterKeyWord, item.FieldName, index);
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
                    switch (item.ConditionalType)
                    {
                        case ConditionalType.Equal:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
                            parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
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
                            if (item.FieldValue == null) item.FieldValue = string.Empty;
                            var inValue1 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "IN", inValue1);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.NotIn:
                            if (item.FieldValue == null) item.FieldValue = string.Empty;
                            var inValue2 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "NOT IN", inValue2);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.LikeLeft:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue + "%"));
                            break;
                        case ConditionalType.NoLike:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " NOT LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue + "%"));
                            break;
                        case ConditionalType.LikeRight:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
                            parameters.Add(new SugarParameter(parameterName, "%" + item.FieldValue));
                            break;
                        case ConditionalType.NoEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.IsNullOrEmpty:
                            builder.AppendFormat("{0} ({1}) OR ({2}) ", type, item.FieldName.ToSqlFilter() + " IS NULL ", item.FieldName.ToSqlFilter() + " = '' ");
                            parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            break;
                        case ConditionalType.IsNot:
                            if (item.FieldValue == null)
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " IS NOT ", "NULL");
                            }
                            else
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
                                parameters.Add(new SugarParameter(parameterName, item.FieldValue));
                            }
                            break;
                        case ConditionalType.EqualNull:
                            if (GetFieldValue(item) == null)
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "  IS ", " NULL ");
                            }
                            else
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
                                parameters.Add(new SugarParameter(parameterName, GetFieldValue(item)));
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
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
                            if (!isFirst)
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
            }
            return new KeyValuePair<string, SugarParameter[]>(builder.ToString(), parameters.ToArray());
        }

        private static object GetFieldValue(ConditionalModel item)
        {
            if (item.FieldValueConvertFunc != null)
                return item.FieldValueConvertFunc(item.FieldValue);
            else
                return item.FieldValue;
        }
        #endregion

        #region Common SqlTemplate
        public string AppendWhereOrAnd(bool isWhere, string sqlString)
        {
            return isWhere ? (" WHERE " + sqlString) : (" AND " + sqlString);
        }
        public string AppendHaving(string sqlString)
        {
            return " HAVING " + sqlString;
        }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public abstract string SqlTranslationLeft { get; }
        public abstract string SqlTranslationRight { get; }
        public virtual string SqlFalse { get { return "1=2 "; } }
        public virtual string SqlDateNow { get { return "GETDATE()"; } }
        public virtual string FullSqlDateNow { get { return "SELECT GETDATE()"; } }
        public virtual string SqlSelectAll { get { return "*"; } }
        #endregion
    }
}
