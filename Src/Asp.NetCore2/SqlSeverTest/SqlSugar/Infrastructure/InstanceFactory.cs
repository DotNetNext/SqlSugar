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
        static Assembly assembly = Assembly.Load(UtilConstants.AssemblyName);
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        public static bool NoCache = false;

        public static void RemoveCache()
        {
            typeCache = new Dictionary<string, Type>();
        }

        #region Queryable
        public static ISugarQueryable<T> GetQueryable<T>(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerQueryable<T>();
            }
            else  if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlQueryable<T>();
            }
            else if (currentConnectionConfig.DbType == DbType.PostgreSQL)
            {
                return new PostgreSQLQueryable<T>();
            }
            else
            {
                string className = "Queryable";
                className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
                ISugarQueryable<T> result = CreateInstance<T, ISugarQueryable<T>>(className);
                return result;
            }
        }
        public static ISugarQueryable<T, T2> GetQueryable<T, T2>(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerQueryable<T, T2>();
            }
            else
            {
                string className = "Queryable";
                className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
                ISugarQueryable<T, T2> result = CreateInstance<T, T2, ISugarQueryable<T, T2>>(className);
                return result;
            }
        }
        public static ISugarQueryable<T, T2, T3> GetQueryable<T, T2, T3>(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerQueryable<T, T2, T3>();
            }
            else
            {
                string className = "Queryable";
                className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
                ISugarQueryable<T, T2, T3> result = CreateInstance<T, T2, T3, ISugarQueryable<T, T2, T3>>(className);
                return result;
            }
        }
        public static ISugarQueryable<T, T2, T3, T4> GetQueryable<T, T2, T3, T4>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4> result = CreateInstance<T, T2, T3, T4, ISugarQueryable<T, T2, T3, T4>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5> GetQueryable<T, T2, T3, T4, T5>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5> result = CreateInstance<T, T2, T3, T4, T5, ISugarQueryable<T, T2, T3, T4, T5>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6> GetQueryable<T, T2, T3, T4, T5, T6>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6> result = CreateInstance<T, T2, T3, T4, T5, T6, ISugarQueryable<T, T2, T3, T4, T5, T6>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GetQueryable<T, T2, T3, T4, T5, T6, T7>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, ISugarQueryable<T, T2, T3, T4, T5, T6, T7>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>>(className);
            return result;
        }

        #region 9-12
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType.ToString(), className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(className);
            return result;
        }
        #endregion

        #endregion

        public static QueryBuilder GetQueryBuilder(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerQueryBuilder();
            }
            else if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlQueryBuilder();
            }
            else
            {
                QueryBuilder result = CreateInstance<QueryBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "QueryBuilder"));
                return result;
            }
        }
        public static InsertBuilder GetInsertBuilder(ConnectionConfig currentConnectionConfig)
        {
            InsertBuilder result = CreateInstance<InsertBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "InsertBuilder"));
            return result;
        }
        public static UpdateBuilder GetUpdateBuilder(ConnectionConfig currentConnectionConfig)
        {
            UpdateBuilder result = CreateInstance<UpdateBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "UpdateBuilder"));
            return result;
        }
        public static DeleteBuilder GetDeleteBuilder(ConnectionConfig currentConnectionConfig)
        {
            DeleteBuilder result = CreateInstance<DeleteBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "DeleteBuilder"));
            return result;
        }

        public static ILambdaExpressions GetLambdaExpressions(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerExpressionContext();
            }
            else if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlExpressionContext();
            }
            else
            {
                ILambdaExpressions result = CreateInstance<ILambdaExpressions>(GetClassName(currentConnectionConfig.DbType.ToString(), "ExpressionContext"));
                return result;
            }
        }

        public static ISqlBuilder GetSqlbuilder(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerBuilder();
            }
            else if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlBuilder();
            }
            else
            {
                ISqlBuilder result = CreateInstance<ISqlBuilder>(GetClassName(currentConnectionConfig.DbType.ToString(), "Builder"));
                return result;
            }
        }

        public static UpdateableProvider<T> GetUpdateableProvider<T>(ConnectionConfig currentConnectionConfig) where T : class, new()
        {
            if (currentConnectionConfig.DbType == DbType.Oracle)
            {
                return new OracleUpdateable<T>();
            }
            else
            {
                return new UpdateableProvider<T>();
            }
        }

        public static DeleteableProvider<T> GetDeleteableProvider<T>(ConnectionConfig currentConnectionConfig) where T : class, new()
        {
            if (currentConnectionConfig.DbType == DbType.Oracle)
            {
                return new OracleDeleteable<T>();
            }
            else
            {
                return new DeleteableProvider<T>();
            }
        }

        public static InsertableProvider<T> GetInsertableProvider<T>(ConnectionConfig currentConnectionConfig) where T : class, new()
        {
            if (currentConnectionConfig.DbType == DbType.Oracle)
            {
                return new OracleInsertable<T>();
            }
            else if (currentConnectionConfig.DbType == DbType.PostgreSQL)
            {
                return new PostgreSQLInserttable<T>();
            }
            else if (currentConnectionConfig.DbType == DbType.Kdbndp)
            {
                return new KdbndpInserttable<T>();
            }
            else
            {
                return new InsertableProvider<T>();
            }
        }

        public static IDbBind GetDbBind(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerDbBind();
            }
            else if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlDbBind();
            }
            else
            {
                IDbBind result = CreateInstance<IDbBind>(GetClassName(currentConnectionConfig.DbType.ToString(), "DbBind"));
                return result;
            }
        }

        public static IDbMaintenance GetDbMaintenance(ConnectionConfig currentConnectionConfig)
        {
            IDbMaintenance result = CreateInstance<IDbMaintenance>(GetClassName(currentConnectionConfig.DbType.ToString(), "DbMaintenance"));
            return result;
        }

        public static IDbFirst GetDbFirst(ConnectionConfig currentConnectionConfig)
        {
            IDbFirst result = CreateInstance<IDbFirst>(GetClassName(currentConnectionConfig.DbType.ToString(), "DbFirst"));
            return result;
        }

        public static ICodeFirst GetCodeFirst(ConnectionConfig currentConnectionConfig)
        {
            ICodeFirst result = CreateInstance<ICodeFirst>(GetClassName(currentConnectionConfig.DbType.ToString(), "CodeFirst"));
            return result;
        }

        public static IAdo GetAdo(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.SqlServer)
            {
                return new SqlServerProvider();
            }
            else
            {
                IAdo result = CreateInstance<IAdo>(GetClassName(currentConnectionConfig.DbType.ToString(), "Provider"));
                return result;
            }
        }

        private static string GetClassName(string type, string name)
        {
            return UtilConstants.AssemblyName + "." + type + name;
        }

        #region CreateInstance
        private static Restult CreateInstance<T, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T));
        }
        private static Restult CreateInstance<T, T2, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2));
        }
        private static Restult CreateInstance<T, T2, T3, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3));
        }
        private static Restult CreateInstance<T, T2, T3, T4, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        }

        #region 9-12
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12));
        }
        #endregion

        private static Restult CreateInstance<Restult>(string className, params Type[] types)
        {
            try
            {
                if (NoCache)
                {
                    return NoCacheGetCacheInstance<Restult>(className, types);
                }
                else
                {
                    return GetCacheInstance<Restult>(className, types);
                }
            }
            catch  
            {
                NoCache = true;
                return NoCacheGetCacheInstance<Restult>(className, types);
            }
        }

        private static Restult GetCacheInstance<Restult>(string className, Type[] types)
        {
            var cacheKey = className + string.Join(",", types.Select(it => it.FullName));
            Type type;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
                    Check.ArgumentNullException(type, string.Format(ErrorMessage.ObjNotExist, className));
                    if (!typeCache.ContainsKey(cacheKey))
                    {
                        typeCache.Add(cacheKey, type);
                    }
                }
            }
            var result = (Restult)Activator.CreateInstance(type, true);
            return result;
        }
        private static Restult NoCacheGetCacheInstance<Restult>(string className, Type[] types)
        {
          
            Type type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
            var result = (Restult)Activator.CreateInstance(type, true);
            return result;
        }

        public static T CreateInstance<T>(string className)
        {
            try
            {
                if (NoCache)
                {
                    return NoCacheGetCacheInstance<T>(className);
                }
                else
                {
                    return GetCacheInstance<T>(className);
                }
            }
            catch  
            {
                return NoCacheGetCacheInstance<T>(className);
            }
        }

        private static T GetCacheInstance<T>(string className)
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
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }
        private static T NoCacheGetCacheInstance<T>(string className)
        {
            Type  type = assembly.GetType(className);
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }
        #endregion
    }
}
