using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    ///Json model to sql
    /// </summary>
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        #region Root
        public KeyValuePair<string, SugarParameter[]> FuncModelToSql(IFuncModel model)
        {
            ObjectFuncModel data = model as ObjectFuncModel;
            var name = data.FuncName;
            var parameters = data.Parameters;
            var dbMethods = this.Context.Queryable<object>().QueryBuilder.LambdaExpressions.DbMehtods;
            var methods = GetAllMethods(dbMethods);
            var methodName = GetMethodName(name, methods);
            var methodInfo = GetMethod(dbMethods, methodName);
            var pars = methodInfo.GetParameters();

            var resSql = "";
            var resPars = new List<SugarParameter>();
            resSql = GetSql(parameters, dbMethods, methodName, methodInfo, pars, resPars);
            if (name.EqualCase("MappingColumn")) 
            {
                if (!(this.Context?. CurrentConnectionConfig?.MoreSettings?.EnableModelFuncMappingColumn == true)) 
                {
                    Check.ExceptionEasy("Enable MappingColumn need in ConnectionConfig - > MoreSettings - > EnableModelFuncMappingColumn set to true", "MappingColumn考虑到风险情况需要开启才能使用，请在 ConnectionConfig->MoreSettings->EnableModelFuncMappingColumn设置为true");
                }
                resSql = parameters.First() +"";
            }
            return new KeyValuePair<string, SugarParameter[]>(resSql, resPars.ToArray());
        }
        #endregion

        #region Level2
        private string GetSql(List<object> parameters, IDbMethods dbMethods, string methodName, System.Reflection.MethodInfo methodInfo, System.Reflection.ParameterInfo[] pars, List<SugarParameter> resPars)
        {
            string resSql;
            if (IsNoParameter(pars))
            {
                resSql = GetNoParameterMehtodSql(dbMethods, methodInfo);
            }
            else if (IsFormatMethod(methodName))
            {
                resSql = GetFormatMethodSql(parameters, resPars);
            }
            else if (IsSqlFuncMethod(pars))
            {
                resSql = GetSqlFuncSql(parameters, dbMethods, methodName, methodInfo, resPars);
            }
            else
            {
                resSql = GetNoSupportMethodSql(methodInfo);
            }

            return resSql;
        }
        private static System.Reflection.MethodInfo GetMethod(IDbMethods dbMethods, string methodName)
        {
            return dbMethods.GetType().GetMethods()
                            .Where(it => it.Name == methodName)
                            .Where(it => it.Name != "Equals" || it.GetParameters().Length == 1 && it.GetParameters().First().ParameterType == typeof(MethodCallExpressionModel))
                            .FirstOrDefault();
        }
        private static string GetMethodName(string name, List<string> methods)
        { 
            var result = methods.FirstOrDefault(it => name.EqualCase("SqlFunc_" + it) || name.EqualCase(it));
            Check.Exception(result == null, $" { name } is error ");
            return result;
        }
        private static List<string> GetAllMethods(IDbMethods dbMethods)
        {
            return new ReflectionInoCacheService().GetOrCreate("Json2SqlGetFuncSql", () =>
                            dbMethods.GetType()
                            .GetMethods().Where(it => it.Name != "GetHashCode").Select(it => it.Name).ToList());
        }
        #endregion

        #region Level3
        private static string GetNoSupportMethodSql(System.Reflection.MethodInfo methodInfo)
        {
            throw new Exception(methodInfo.Name);
        }
        private string GetSqlFuncSql(List<object> parameters, IDbMethods dbMethods, string methodName, System.Reflection.MethodInfo methodInfo, List<SugarParameter> resPars)
        {
            string resSql;
            var args = new List<MethodCallExpressionArgs>();
            foreach (var item in parameters)
            {             
                var value = GetSqlPart(item, resPars);
                args.Add(new MethodCallExpressionArgs
                {
                    MemberName = value,
                    MemberValue = resPars.FirstOrDefault(it => it.ParameterName == value)?.Value?? value,
                    IsMember = true
                });
            }
            resSql = methodInfo.Invoke(dbMethods, new object[] { new MethodCallExpressionModel() {
                  Name=methodName,
                  Args=args
                } }).ObjToString();
            return resSql;
        }
        private string GetFormatMethodSql(List<object> parameters, List<SugarParameter> resPars)
        {
            string resSql;
            var objects = new List<string>();
            foreach (var item in parameters)
            {
                var value = GetSqlPart(item, resPars);
                objects.Add(value.ObjToString());
            }
            resSql = string.Join(" ", string.Join(" ", objects));
            return resSql;
        }
        private static string GetNoParameterMehtodSql(IDbMethods dbMethods, System.Reflection.MethodInfo methodInfo)
        {
            return methodInfo.Invoke(dbMethods, new object[] { }).ObjToString();
        } 
        #endregion

        #region Helper
        private static bool IsSqlFuncMethod(System.Reflection.ParameterInfo[] pars)
        {
            return pars.First().ParameterType == typeof(MethodCallExpressionModel);
        }
        private static bool IsFormatMethod(string methodName)
        {
            return methodName.EqualCase("format");
        }
        private static bool IsNoParameter(System.Reflection.ParameterInfo[] pars)
        {
            return pars.Length == 0;
        } 
        #endregion
    }
}
