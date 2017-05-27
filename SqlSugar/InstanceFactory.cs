using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class InstanceFactory
    {
        static Assembly assembly = Assembly.Load(PubConst.AssemblyName);
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        #region Queryable
        public static ISugarQueryable<T> GetQueryable<T>(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T> reval = CreateInstance<T, ISugarQueryable<T>>(className, currentConnectionConfig.DbType);
            return reval;
        }
        public static ISugarQueryable<T, T2> GetQueryable<T, T2>(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2> reval = CreateInstance<T, T2, ISugarQueryable<T, T2>>(className, currentConnectionConfig.DbType);
            return reval;
        }
        public static ISugarQueryable<T, T2, T3> GetQueryable<T, T2, T3>(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3> reval = CreateInstance<T, T2, T3, ISugarQueryable<T, T2, T3>>(className, currentConnectionConfig.DbType);
            return reval;
        }
        public static ISugarQueryable<T, T2, T3, T4> GetQueryable<T, T2, T3, T4>(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4> reval = CreateInstance<T, T2, T3, T4, ISugarQueryable<T, T2, T3, T4>>(className, currentConnectionConfig.DbType);
            return reval;
        } 
        #endregion

        public static QueryBuilder GetQueryBuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            QueryBuilder reval = CreateInstance<QueryBuilder>(GetClassName(currentConnectionConfig.DbType, "QueryBuilder"), currentConnectionConfig.DbType);
            return reval;
        }
        public static InsertBuilder GetInsertBuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            InsertBuilder reval = CreateInstance<InsertBuilder>(GetClassName(currentConnectionConfig.DbType, "InsertBuilder"), currentConnectionConfig.DbType);
            return reval;
        }
        public static UpdateBuilder GetUpdateBuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            UpdateBuilder reval = CreateInstance<UpdateBuilder>(GetClassName(currentConnectionConfig.DbType, "UpdateBuilder"), currentConnectionConfig.DbType);
            return reval;
        }
        public static DeleteBuilder GetDeleteBuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            DeleteBuilder reval = CreateInstance<DeleteBuilder>(GetClassName(currentConnectionConfig.DbType, "DeleteBuilder"), currentConnectionConfig.DbType);
            return reval;
        }

        public static ILambdaExpressions GetLambdaExpressions(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            ILambdaExpressions reval = CreateInstance<ILambdaExpressions>(GetClassName(currentConnectionConfig.DbType, "ExpressionContext"), currentConnectionConfig.DbType);
            return reval;
        }

        public static ISqlBuilder GetSqlbuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            ISqlBuilder reval = CreateInstance<ISqlBuilder>(GetClassName(currentConnectionConfig.DbType, "Builder"), currentConnectionConfig.DbType);
            return reval;
        }

        public static IDbBind GetDbBind(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            IDbBind reval = CreateInstance<IDbBind>(GetClassName(currentConnectionConfig.DbType, "DbBind"), currentConnectionConfig.DbType);
            return reval;
        }

        public static IDbMaintenance GetDbMaintenance(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            IDbMaintenance reval = CreateInstance<IDbMaintenance>(GetClassName(currentConnectionConfig.DbType, "DbMaintenance"), currentConnectionConfig.DbType);
            return reval;
        }

        public static IDbFirst GetDbFirst(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            IDbFirst reval = CreateInstance<IDbFirst>(GetClassName(currentConnectionConfig.DbType, "DbFirst"), currentConnectionConfig.DbType);
            return reval;
        }

        public static ICodeFirst GetCodeFirst(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            ICodeFirst reval = CreateInstance<ICodeFirst>(GetClassName(currentConnectionConfig.DbType, "CodeFirst"), currentConnectionConfig.DbType);
            return reval;
        }

        public static IAdo GetAdo(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            IAdo reval = CreateInstance<IAdo>(GetClassName(currentConnectionConfig.DbType, "Provider"), currentConnectionConfig.DbType);
            return reval;
        }

        private static string GetClassName(string type, string name)
        {
            return PubConst.AssemblyName + "." + type + name;
        }

        #region CreateInstance
        private static Restult CreateInstance<T, Restult>(string className, string dbType)
        {
            var cacheKey = className + typeof(T).FullName;
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`1", true).MakeGenericType(typeof(T));
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var reval = (Restult)Activator.CreateInstance(type, true);
            return reval;
        }
        private static Restult CreateInstance<T, T2, Restult>(string className, string dbType)
        {
            var cacheKey = className + typeof(T).FullName + typeof(T2).FullName;
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`2", true).MakeGenericType(typeof(T), typeof(T2));
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var reval = (Restult)Activator.CreateInstance(type, true);
            return reval;
        }
        private static Restult CreateInstance<T, T2, T3, Restult>(string className, string dbType)
        {
            var cacheKey = className + typeof(T).FullName + typeof(T2).FullName + typeof(T3).FullName;
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`3", true).MakeGenericType(typeof(T), typeof(T2), typeof(T3));
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var reval = (Restult)Activator.CreateInstance(type, true);
            return reval;
        }
        private static Restult CreateInstance<T, T2, T3, T4, Restult>(string className, string dbType)
        {
            var cacheKey = className + typeof(T).FullName + typeof(T2).FullName + typeof(T3).FullName + typeof(T4).FullName;
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`4", true).MakeGenericType(typeof(T), typeof(T2), typeof(T4), typeof(T4));
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var reval = (Restult)Activator.CreateInstance(type, true);
            return reval;
        }
        private static T CreateInstance<T>(string className, string dbType)
        {
            Type type;
            if (typeCache.ContainsKey(className))
            {
                type = typeCache[className];
            }
            else
            {
                lock (typeCache)
                {
                    type = assembly.GetType(className);
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(className))
                    {
                        typeCache.Add(className, type);
                    }
                }
            }
            var reval = (T)Activator.CreateInstance(type, true);
            return reval;
        } 
        #endregion

        private static void CheckConfig(IConnectionConfig currentConnectionConfig)
        {
            Check.ArgumentNullException(currentConnectionConfig, ErrorMessage.ConnectionConfigIsNull);
        }

    }
}
