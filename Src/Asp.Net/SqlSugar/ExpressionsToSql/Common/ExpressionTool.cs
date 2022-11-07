using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public class ExpressionTool
    {
        public static bool IsComparisonOperatorBool(BinaryExpression binaryExp)
        {
            return binaryExp.NodeType.IsIn(ExpressionType.Equal, 
                                           ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual,
                                           ExpressionType.LessThan, ExpressionType.LessThanOrEqual);
        }

        public static string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Coalesce:
                    throw new Exception("Expression no support ?? ,Use SqlFunc.IsNull");
                default:
                    Check.ThrowNotSupportedException(string.Format(ErrorMessage.OperatorError, expressiontype.ToString()));
                    return null;
            }
        }


        public static void GetOneToOneInfo<T>(SqlSugarProvider context,Expression<Func<T, object>> LeftObject, out MemberExpression memberExpression, out string navObjectName, out EntityColumnInfo navColumn, out EntityInfo navEntityInfo, out EntityColumnInfo navPkColumn)
        {
            memberExpression = ((LeftObject as LambdaExpression).Body as MemberExpression);
            var listItemType = typeof(T);
            var listItemEntity = context.EntityMaintenance.GetEntityInfo(listItemType);
            var listPkColumn = listItemEntity.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            navObjectName = memberExpression.Member.Name;
            var navObjectName2 = navObjectName;
            var navObjectNamePropety = listItemType.GetProperty(navObjectName);
            var navObjectNameColumnInfo = listItemEntity.Columns.First(it => it.PropertyName == navObjectName2);
            Check.ExceptionEasy(navObjectNameColumnInfo.Navigat == null, $"{navObjectName} not [Navigat(..)] ", $"{navObjectName} 没有导航特性 [Navigat(..)] ");
            Check.ExceptionEasy(navObjectNameColumnInfo.Navigat.NavigatType != NavigateType.OneToOne, $"IncludeLeftJoin can only be one-on-one ", $"IncludeLeftJoin 只能是一对一 ");
            navColumn = listItemEntity.Columns.FirstOrDefault(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name);
            Check.ExceptionEasy(navColumn == null, "OneToOne navigation configuration error", $"OneToOne导航配置错误： 实体{ listItemEntity.EntityName } 不存在{navObjectNameColumnInfo.Navigat.Name}");
            var navType = navObjectNamePropety.PropertyType;
            navEntityInfo = context.EntityMaintenance.GetEntityInfo(navType);
            context.InitMappingInfo(navEntityInfo.Type);
            navPkColumn = navEntityInfo.Columns.Where(it => it.IsPrimarykey).FirstOrDefault();
            Check.ExceptionEasy(navPkColumn == null && navObjectNameColumnInfo.Navigat.Name2 == null, navEntityInfo.EntityName + "need primarykey", navEntityInfo.EntityName + " 需要主键");
            if (navObjectNameColumnInfo.Navigat.Name2.HasValue())
            {
                navPkColumn = navEntityInfo.Columns.Where(it => it.PropertyName == navObjectNameColumnInfo.Navigat.Name2).FirstOrDefault();
            }
        }


        public static List<ParameterExpression> ExpressionParameters(Expression expression)
        {
             List<ParameterExpression> parameters = null;
             if (expression is LambdaExpression)
             {
                  if ((expression as LambdaExpression).Parameters != null) 
                  {
                      parameters = (expression as LambdaExpression).Parameters.ToList();
                  }
             }
             return parameters;
        }

        public static object GetValue(object value,ExpressionContext context)
        {
            if (value == null) return value;
            var type = value.GetType();
            if (type.IsEnum() && type != typeof(DateType) && type != typeof(JoinType) && type != typeof(OrderByType))
            {
                if (context.TableEnumIsString == true)
                {
                    return value.ToString();
                }
                else 
                {
                    return Convert.ToInt64(value);
                }
            }
            else
                return value;
        }

        public static Expression GetLambdaExpressionBody(Expression expression)
        {
            Expression newExp = expression;
            if (newExp is LambdaExpression)
            {
                newExp = (newExp as LambdaExpression).Body;
            }

            return newExp;
        }

        public static string GetMethodName(Expression expression) 
        {
            if (expression is MethodCallExpression) 
            {
                return (expression as MethodCallExpression).Method.Name;
            }
            return null;
        }

        public static Type GetMemberInfoType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    return null;
            }
        }
        public static bool IsLogicOperator(string operatorValue)
        {
            return operatorValue == "&&" || operatorValue == "||" || operatorValue == "AND" || operatorValue == "OR";
        }

        public static bool IsLogicOperator(Expression expression)
        {
            return expression.NodeType == ExpressionType.And ||
                                        expression.NodeType == ExpressionType.AndAlso ||
                                        expression.NodeType == ExpressionType.Or ||
                                        expression.NodeType == ExpressionType.OrElse;
        }
        public static bool IsComparisonOperator(Expression expression)
        {
            return expression.NodeType != ExpressionType.And &&
                                        expression.NodeType != ExpressionType.AndAlso &&
                                        expression.NodeType != ExpressionType.Or &&
                                        expression.NodeType != ExpressionType.OrElse;
        }
        public static object GetMemberValue(MemberInfo member, Expression expression)
        {
            var rootExpression = expression as MemberExpression;
            var memberInfos = new Stack<MemberInfo>();
            var fieldInfo = member as System.Reflection.FieldInfo;
            object reval = null;
            MemberExpression memberExpr = null;
            while (expression is MemberExpression)
            {
                memberExpr = expression as MemberExpression;
                memberInfos.Push(memberExpr.Member);
                if (memberExpr.Expression == null)
                {
                    var isProperty = memberExpr.Member.MemberType == MemberTypes.Property;
                    var isField = memberExpr.Member.MemberType == MemberTypes.Field;
                    if (isProperty)
                    {
                        try
                        {
                            reval = GetPropertyValue(memberExpr);
                        }
                        catch
                        {
                            reval = null;
                        }
                    }
                    else if (isField)
                    {
                        reval = GetFiledValue(memberExpr);
                    }
                }
                if (memberExpr.Expression == null)
                {

                }
                expression = memberExpr.Expression;
            }
            // fetch the root object reference:
            var constExpr = expression as ConstantExpression;
            if (constExpr == null)
            {
                return DynamicInvoke(rootExpression);
            }
            object objReference = constExpr.Value;
            // "ascend" back whence we came from and resolve object references along the way:
            while (memberInfos.Count > 0)  // or some other break condition
            {
                var mi = memberInfos.Pop();
                if (mi.MemberType == MemberTypes.Property)
                {
                    var objProp = objReference.GetType().GetProperties().Where(it=>it.Name== mi.Name).FirstOrDefault();
                    if (objProp == null)
                    {
                        objReference = DynamicInvoke(expression, rootExpression == null ? memberExpr : rootExpression);
                    }
                    else
                    {
                        objReference = objProp.GetValue(objReference, null);
                    }
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    var objField = objReference.GetType().GetField(mi.Name);
                    if (objField == null)
                    {
                        objReference = DynamicInvoke(expression, rootExpression == null ? memberExpr : rootExpression);
                    }
                    else
                    {
                        objReference = objField.GetValue(objReference);
                    }
                }
            }
            reval = objReference;
            return reval;
        }

        internal static Expression RemoveConvert(Expression item)
        {
            for (int i = 0; i < 10; i++)
            {
                if ((item is UnaryExpression) && (item as UnaryExpression).NodeType == ExpressionType.Convert)
                {
                    item = (item as UnaryExpression).Operand;
                }
                else 
                {
                    break;
                }
            }
            return item;
        }
        internal static Expression RemoveConvertThanOne(Expression item)
        {
            for (int i = 0; i < 10; i++)
            {
                if ((item is UnaryExpression) 
                    && (item as UnaryExpression).NodeType == ExpressionType.Convert
                    && (item as UnaryExpression).Operand is UnaryExpression)
                {
                    item = (item as UnaryExpression).Operand;
                }
                else
                {
                    break;
                }
            }
            return item;
        }
        public static string GetMemberName(Expression expression) 
        {
            if (expression is LambdaExpression) 
            {
                expression = (expression as LambdaExpression).Body;
            }
            if (expression is UnaryExpression) 
            {
                expression = ((UnaryExpression)expression).Operand;   
            }
            var member = (expression as MemberExpression).Member.Name;
            return member;
        }
        internal static object GetExpressionValue(Expression expression)
        {
            try
            {
                if (expression is ConstantExpression)
                {
                    return (expression as ConstantExpression).Value;
                }
                else
                {
                    return GetMemberValue((expression as MemberExpression).Member, expression);
                }
            }
            catch  
            {
                return LambdaExpression.Lambda(expression).Compile().DynamicInvoke();
            }
        }

        public static object GetFiledValue(MemberExpression memberExpr)
        {
            if (!(memberExpr.Member is FieldInfo))
            {
                return DynamicInvoke(memberExpr);
            }
            object reval = null;
            FieldInfo field = (FieldInfo)memberExpr.Member;
            Check.Exception(field.IsPrivate, string.Format(" Field \"{0}\" can't be private ", field.Name));
            reval = field.GetValue(memberExpr.Member);
            if (reval != null && reval.GetType().IsClass() && reval.GetType() != UtilConstants.StringType)
            {
                var fieldName = memberExpr.Member.Name;
                var proInfo = reval.GetType().GetProperty(fieldName);
                if (proInfo != null)
                {
                    reval = proInfo.GetValue(reval, null);
                }
                var fieInfo = reval.GetType().GetField(fieldName);
                if (fieInfo != null)
                {
                    reval = fieInfo.GetValue(reval);
                }
                if (fieInfo == null && proInfo == null)
                {
                    Check.Exception(field.IsPrivate, string.Format(" Field \"{0}\" can't be private ", field.Name));
                }
            }
            return reval;
        }


        public static bool IsConstExpression(MemberExpression memberExpr)
        {
            var result = false;
            while (memberExpr != null && memberExpr.Expression != null)
            {
                var isConst = memberExpr.Expression is ConstantExpression;
                if (isConst)
                {
                    result = true;
                    break;
                }
                else if (memberExpr.Expression is BinaryExpression&&(memberExpr.Expression as BinaryExpression).NodeType==ExpressionType.ArrayIndex) 
                {
                    result = true;
                    break;
                }
                memberExpr = memberExpr.Expression as MemberExpression;
            }
            return result;
        }

        public static object GetPropertyValue(MemberExpression memberExpr)
        {
            if (!(memberExpr.Member is PropertyInfo))
            {
                return DynamicInvoke(memberExpr);
            }
            object reval = null;
            PropertyInfo pro = (PropertyInfo)memberExpr.Member;
            reval = pro.GetValue(memberExpr.Member, null);
            if (reval != null && reval.GetType().IsClass() && reval.GetType() != UtilConstants.StringType)
            {
                var fieldName = memberExpr.Member.Name;
                var proInfo = reval.GetType().GetProperty(fieldName);
                if (proInfo != null)
                {
                    reval = proInfo.GetValue(reval, null);
                }
                var fieInfo = reval.GetType().GetField(fieldName);
                if (fieInfo != null)
                {
                    reval = fieInfo.GetValue(reval);
                }
                if (fieInfo == null && proInfo == null)
                {
                    Check.Exception(true, string.Format(" Property \"{0}\" can't be private ", pro.Name));
                }
            }
            return reval;
        }

        public static object DynamicInvoke(Expression expression, MemberExpression memberExpression = null)
        {
            try
            {
                object value = Expression.Lambda(expression).Compile().DynamicInvoke();
                if (value != null && value.GetType().IsClass() && value.GetType() != UtilConstants.StringType && memberExpression != null)
                {
                    value = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                }

                return value;
            }
            catch (InvalidOperationException ex)
            {
                return new MapperExpressionResolve(expression,ex).GetSql(); ;
            }
            catch (Exception ex)
            {
                throw new Exception("No support "+expression.ToString()+" "+ex.Message);
            }
        }

        public static Type GetPropertyOrFieldType(MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).PropertyType;
            if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).FieldType;
            throw new NotSupportedException();
        }

        public static bool IsEntity(Type type)
        {
            return type.IsClass() && type != UtilConstants.StringType;
        }

        public static bool IsValueType(Type type)
        {
            return !IsEntity(type);
        }

        public static bool IsUnConvertExpress(Expression item)
        {
            return item is UnaryExpression && item.NodeType == ExpressionType.Convert;
        }

        internal static List<NewExpressionInfo> GetNewexpressionInfos(Expression item,ExpressionContext context, BaseResolve baseResolve)
        {
            List<NewExpressionInfo> result = new List<NewExpressionInfo>();
            foreach (MemberBinding binding in ((MemberInitExpression)item).Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }
                MemberAssignment memberAssignment = (MemberAssignment)binding;
                NewExpressionInfo additem = new NewExpressionInfo();
                if (memberAssignment.Expression is MemberExpression)
                {
                    additem.LeftNameName = memberAssignment.Member.Name;
                    var member = (memberAssignment.Expression as MemberExpression).Expression;
                    additem.ShortName = member + "";
                    additem.RightName = (memberAssignment.Expression as MemberExpression).Member.Name;
                    additem.RightDbName = context.GetDbColumnName(member.Type.Name, additem.RightName);
                    result.Add(additem);
                }
                else if (memberAssignment.Expression is ConstantExpression)
                {
                    var value = ((ConstantExpression)memberAssignment.Expression).Value;
                    //var leftInfo = keys[i];
                    additem.Type = nameof(ConstantExpression);
                    additem.RightName = memberAssignment.Member.Name;
                    additem.ShortName = memberAssignment.Member.Name;
                    additem.RightName = memberAssignment.Member.Name;
                    additem.LeftNameName = memberAssignment.Member.Name;
                    additem.RightDbName = UtilMethods.GetSqlValue(value);
                    //additem.Value = "";
                    result.Add(additem);
                }
                else 
                {
                    var value = baseResolve.GetNewExpressionValue(memberAssignment.Expression);
                    //var leftInfo = keys[i];
                    additem.Type = nameof(ConstantExpression);
                    additem.RightName = memberAssignment.Member.Name;
                    additem.ShortName = memberAssignment.Member.Name;
                    additem.RightName = memberAssignment.Member.Name;
                    additem.LeftNameName = memberAssignment.Member.Name;
                    additem.RightDbName = value;
                    //additem.Value = "";
                    result.Add(additem);
                }

                }
                return result;
        }
        internal static List<NewExpressionInfo> GetNewDynamicexpressionInfos(Expression item, ExpressionContext context, BaseResolve baseResolve)
        {
            List<NewExpressionInfo> result = new List<NewExpressionInfo>();
            int i = 0;
            foreach (var binding in ((NewExpression)item).Arguments)
            {
                NewExpressionInfo additem = new NewExpressionInfo();
                var keys = ((NewExpression)item).Members;
                if (binding is MemberExpression)
                {
                    var member = (MemberExpression)binding;
                    var entityName = member.Expression?.Type?.Name;
                    //var memberAssignment = binding;
                    //NewExpressionInfo additem = new NewExpressionInfo();
                    additem.RightName = member.Member.Name;
                    additem.ShortName = member.Expression + "";
                    additem.RightName = member.Member.Name;
                    additem.RightDbName = context.GetDbColumnName(entityName, additem.RightName);
                    additem.LeftNameName = member.Member.Name;
                    //additem.Value = "";
                    result.Add(additem);
                }
                else if (binding is ConstantExpression)
                {
                    var value = ((ConstantExpression)binding).Value;
                    var leftInfo = keys[i];
                    additem.Type = nameof(ConstantExpression);
                    additem.RightName = leftInfo.Name;
                    additem.ShortName = leftInfo.Name;
                    additem.RightName = leftInfo.Name;
                    additem.LeftNameName = leftInfo.Name;
                    additem.RightDbName = UtilMethods.GetSqlValue(value);
                    //additem.Value = "";
                    result.Add(additem);
                }
                else  
                {
                    var value = baseResolve.GetNewExpressionValue(binding);
                    var leftInfo = keys[i];
                    additem.Type = nameof(ConstantExpression);
                    additem.RightName = leftInfo.Name;
                    additem.ShortName = leftInfo.Name;
                    additem.RightName = leftInfo.Name;
                    additem.LeftNameName = leftInfo.Name;
                    additem.RightDbName = value;
                    //additem.Value = "";
                    result.Add(additem);
                }
                i++;
            }
            return result;
        }
    }
}
