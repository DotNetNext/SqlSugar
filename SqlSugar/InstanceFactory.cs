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
        public static ISugarQueryable<T> GetQueryable<T>(IConnectionConfig currentConnectionConfig)where T:class,new()
        {
            CheckConfig(currentConnectionConfig);
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T> reval = CreateInstance<T,ISugarQueryable<T>>(className, currentConnectionConfig.DbType);
            return reval;
        }

        public static LambadaQueryBuilder GetGetLambadaQueryBuilder(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            LambadaQueryBuilder reval = CreateInstance<LambadaQueryBuilder>(GetClassName(currentConnectionConfig.DbType, "LambadaQueryBuilder"), currentConnectionConfig.DbType);
            return reval;
        }
        public static ISugarSqlable GetSqlable(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            ISugarSqlable reval = CreateInstance<ISugarSqlable>(GetClassName(currentConnectionConfig.DbType, "Sqlable"), currentConnectionConfig.DbType);
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

        public static IDb GetDb(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            IDb reval = CreateInstance<IDb>(GetClassName(currentConnectionConfig.DbType, "Db"), currentConnectionConfig.DbType);
            return reval;
        }

        public static ILambdaExpressions GetLambdaExpressions(IConnectionConfig currentConnectionConfig)
        {
            CheckConfig(currentConnectionConfig);
            ILambdaExpressions reval = CreateInstance<ILambdaExpressions>(GetClassName(currentConnectionConfig.DbType, "LambdaExpressions"), currentConnectionConfig.DbType);
            return reval;
        }

        private static string GetClassName(string type, string name)
        {
            return PubConst.AssemblyName + "." + type + name;
        }
        private static Restult CreateInstance<T,Restult>(string className, string dbType) where T:class,new()
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
                    type=assembly.GetType(className);
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
        private static void CheckConfig(IConnectionConfig currentConnectionConfig)
        {
            Check.ArgumentNullException(currentConnectionConfig, ErrorMessage.ConnectionConfigIsNull);
        }

    }
}
