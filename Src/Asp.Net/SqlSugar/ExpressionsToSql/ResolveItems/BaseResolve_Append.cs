using System;
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    /// <summary>
    /// BaseResolve-Append
    /// </summary>
    public partial class BaseResolve
    {
        protected void AppendMember(ExpressionParameter parameter, bool? isLeft, object appendValue)
        {

            Context.ParameterIndex++;
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }
        }
        protected void AppendValue(ExpressionParameter parameter, bool? isLeft, object value)
        {
            if (parameter.BaseExpression is BinaryExpression || parameter.BaseExpression == null)
            {
                var oppoSiteExpression = isLeft == true ? parameter.BaseParameter.RightExpression : parameter.BaseParameter.LeftExpression;

                if (value is MapperSql)
                {
                    ApppendMapperSql(parameter, isLeft, value);
                }
                else if (parameter.CurrentExpression is MethodCallExpression || parameter.CurrentExpression is ConditionalExpression || parameter.CurrentExpression.NodeType == ExpressionType.Coalesce)
                {
                    AppendMethod(parameter, isLeft, value);
                }
                else if (oppoSiteExpression is MemberExpression)
                {
                    AppendMember(parameter, isLeft, value, oppoSiteExpression);
                }
                else if (ExpressionTool.RemoveConvert(oppoSiteExpression) is MemberExpression)
                {
                    AppendMember(parameter, isLeft, value, ExpressionTool.RemoveConvert(oppoSiteExpression));
                }
                else if ((oppoSiteExpression is UnaryExpression && (oppoSiteExpression as UnaryExpression).Operand is MemberExpression))
                {
                    value = AppendUnaryExp(parameter, isLeft, value, oppoSiteExpression);
                }
                else
                {
                    value = AppendOther(parameter, isLeft, value);
                }
            }
        }
        private object AppendOther(ExpressionParameter parameter, bool? isLeft, object value)
        {
            var appendValue = this.Context.SqlParameterKeyWord + ExpressionConst.Const + Context.ParameterIndex;
            Context.ParameterIndex++;
            if (value != null && value.GetType().IsEnum())
            {
                if (this.Context.TableEnumIsString == true)
                {
                    value = value.ToString();
                }
                else
                {
                    value = Convert.ToInt64(value);
                }
            }
            this.Context.Parameters.Add(new SugarParameter(appendValue, value));
            appendValue = string.Format(" {0} ", appendValue);
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }

            return value;
        }
        private object AppendUnaryExp(ExpressionParameter parameter, bool? isLeft, object value, Expression oppoSiteExpression)
        {
            string appendValue = Context.SqlParameterKeyWord
              + ((MemberExpression)(oppoSiteExpression as UnaryExpression).Operand).Member.Name
              + Context.ParameterIndex;
            if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
            {
                value = this.Context.TableEnumIsString == true ? value.ToString() : value;
                this.Context.Parameters.Add(new SugarParameter(appendValue, value));
            }
            else
            {
                appendValue = value.ObjToString();
            }
            Context.ParameterIndex++;
            appendValue = string.Format(" {0} ", appendValue);
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }

            return value;
        }
        private void AppendMember(ExpressionParameter parameter, bool? isLeft, object value, Expression oppoSiteExpression)
        {
            string appendValue = Context.SqlParameterKeyWord
                + ((MemberExpression)oppoSiteExpression).Member.Name
                + Context.ParameterIndex;
            if (IsNullValue(parameter, value))
            {
                appendValue = $" NULL ";
                parameter.BaseParameter.ValueIsNull = true;
            }
            else if (value.ObjToString() != "NULL" && !parameter.ValueIsNull)
            {
                EntityColumnInfo columnInfo = GetColumnInfo(oppoSiteExpression);
                if (columnInfo != null && columnInfo.SqlParameterDbType != null && columnInfo.SqlParameterDbType is System.Data.DbType)
                {
                    var p = new SugarParameter(appendValue, value, (System.Data.DbType)columnInfo.SqlParameterDbType);
                    if (columnInfo.SqlParameterSize != null)
                    {
                        p.Size = columnInfo.SqlParameterSize.ObjToInt();
                    }
                    this.Context.Parameters.Add(p);
                }
                else if (UtilMethods.IsParameterConverter(columnInfo))
                {
                    SugarParameter p = UtilMethods.GetParameterConverter(this.Context.ParameterIndex,this.Context.SugarContext.Context, value, oppoSiteExpression, columnInfo);
                    appendValue = p.ParameterName;
                    this.Context.Parameters.Add(p);
                }
                else if (parameter?.BaseParameter?.CommonTempData.ObjToString() == "IsJson=true")
                {
                    this.Context.Parameters.Add(new SugarParameter(appendValue, new SerializeService().SerializeObject(value)) { IsJson = true });
                }
                else if (parameter?.BaseParameter?.CommonTempData.ObjToString() == "IsArray=true")
                {
                    this.Context.Parameters.Add(new SugarParameter(appendValue, value) { IsArray = true });
                }
                else if (value!=null&&(value is Enum) &&this.Context?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString == true) 
                {
                    this.Context.Parameters.Add(new SugarParameter(appendValue,Convert.ToString(value)));
                }
                else
                {
                    this.Context.Parameters.Add(new SugarParameter(appendValue, value));
                }
            }
            else
            {
                appendValue = value.ObjToString();
            }
            Context.ParameterIndex++;
            appendValue = string.Format(" {0} ", appendValue);
            if (isLeft == true)
            {
                appendValue += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue);
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }
        }
        private void AppendMethod(ExpressionParameter parameter, bool? isLeft, object value)
        {
            var appendValue = value;
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, appendValue.ObjToString());
            }
            else
            {
                this.Context.Result.Append(appendValue);
            }
            this.AppendOpreator(parameter, isLeft);
        }
        private void ApppendMapperSql(ExpressionParameter parameter, bool? isLeft, object value)
        {
            var sql = ((MapperSql)value).Sql;
            if (isLeft == true)
            {
                sql += ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index;
            }
            if (this.Context.Result.Contains(ExpressionConst.FormatSymbol))
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, sql);
            }
            else
            {
                this.Context.Result.Append(sql);
            }
        }
        protected void AppendOpreator(ExpressionParameter parameter, bool? isLeft)
        {
            if (isLeft == true)
            {
                this.Context.Result.Append(" " + ExpressionConst.ExpressionReplace + parameter.BaseParameter.Index);
            }
        }
        protected string AppendParameter(object paramterValue)
        {
            string parameterName = this.Context.SqlParameterKeyWord + "constant" + this.Context.ParameterIndex;
            this.Context.ParameterIndex++; ;
            this.Context.Parameters.Add(new SugarParameter(parameterName, paramterValue));
            return parameterName;
        }
        protected string AppendParameter(SugarParameter p)
        {
             p.ParameterName= p.ParameterName + this.Context.ParameterIndex;
            this.Context.ParameterIndex++; ;
            this.Context.Parameters.Add(p);
            return p.ParameterName;
        }
        protected void AppendNot(object Value)
        {
            var isAppend = !this.Context.Result.Contains(ExpressionConst.FormatSymbol);
            var lastCharIsSpace = this.Context.Result.LastCharIsSpace;
            if (isAppend)
            {
                this.Context.Result.Append(lastCharIsSpace ? "NOT" : " NOT");
            }
            else
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, "NOT");
            }
        }
        protected void AppendNegate(object Value)
        {
            var isAppend = !this.Context.Result.Contains(ExpressionConst.FormatSymbol);
            var lastCharIsSpace = this.Context.Result.LastCharIsSpace;
            if (isAppend)
            {
                this.Context.Result.Append(lastCharIsSpace ? "-" : " -");
            }
            else
            {
                this.Context.Result.Replace(ExpressionConst.FormatSymbol, "-");
            }
        }
        private void AppendOnlyInSelectConvertToString(ExpressionParameter parameter, Expression item, string asName)
        {
            var name =GetNewExpressionValue((item as MethodCallExpression)?.Arguments[0]);
            var methodInfo = ExpressionTool.DynamicInvoke(((item as MethodCallExpression)?.Arguments[1]));
            if (this.Context.SugarContext.QueryBuilder.QueryableFormats == null)
                this.Context.SugarContext.QueryBuilder.QueryableFormats = new List<QueryableFormat>();
            this.Context.SugarContext.QueryBuilder.QueryableFormats.Add(new QueryableFormat()
            {
                Format = "",
                PropertyName = asName,  
                MethodName = "OnlyInSelectConvertToString",
                MethodInfo= (MethodInfo)methodInfo
            });
            parameter.Context.Result.Append(this.Context.GetAsString2(asName, name));
        }
    }
}
