﻿using System;
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
        public static ISugarQueryable<T> GetQueryable<T>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T> result = CreateInstance<T, ISugarQueryable<T>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2> GetQueryable<T, T2>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2> result = CreateInstance<T, T2, ISugarQueryable<T, T2>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3> GetQueryable<T, T2, T3>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3> result = CreateInstance<T, T2, T3, ISugarQueryable<T, T2, T3>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4> GetQueryable<T, T2, T3, T4>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4> result = CreateInstance<T, T2, T3, T4, ISugarQueryable<T, T2, T3, T4>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5> GetQueryable<T, T2, T3, T4, T5>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4, T5> result = CreateInstance<T, T2, T3, T4, T5, ISugarQueryable<T, T2, T3, T4, T5>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6> GetQueryable<T, T2, T3, T4, T5, T6>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4, T5, T6> result = CreateInstance<T, T2, T3, T4, T5, T6, ISugarQueryable<T, T2, T3, T4, T5, T6>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7> GetQueryable<T, T2, T3, T4, T5, T6, T7>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, ISugarQueryable<T, T2, T3, T4, T5, T6, T7>>(className);
            return result;
        }
        public static ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> GetQueryable<T, T2, T3, T4, T5, T6, T7, T8>(ConnectionConfig currentConnectionConfig)
        {
            string className = "Queryable";
            className = GetClassName(currentConnectionConfig.DbType, className);
            ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8> result = CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, ISugarQueryable<T, T2, T3, T4, T5, T6, T7, T8>>(className);
            return result;
        }
        #endregion

        public static QueryBuilder GetQueryBuilder(ConnectionConfig currentConnectionConfig)
        {
            QueryBuilder result = CreateInstance<QueryBuilder>(GetClassName(currentConnectionConfig.DbType, "QueryBuilder"));
            return result;
        }
        public static InsertBuilder GetInsertBuilder(ConnectionConfig currentConnectionConfig)
        {
            InsertBuilder result = CreateInstance<InsertBuilder>(GetClassName(currentConnectionConfig.DbType, "InsertBuilder"));
            return result;
        }
        public static UpdateBuilder GetUpdateBuilder(ConnectionConfig currentConnectionConfig)
        {
            UpdateBuilder result = CreateInstance<UpdateBuilder>(GetClassName(currentConnectionConfig.DbType, "UpdateBuilder"));
            return result;
        }
        public static DeleteBuilder GetDeleteBuilder(ConnectionConfig currentConnectionConfig)
        {
            DeleteBuilder result = CreateInstance<DeleteBuilder>(GetClassName(currentConnectionConfig.DbType, "DeleteBuilder"));
            return result;
        }

        public static ILambdaExpressions GetLambdaExpressions(ConnectionConfig currentConnectionConfig)
        {
            ILambdaExpressions result = CreateInstance<ILambdaExpressions>(GetClassName(currentConnectionConfig.DbType, "ExpressionContext"));
            return result;
        }

        public static ISqlBuilder GetSqlbuilder(ConnectionConfig currentConnectionConfig)
        {
            ISqlBuilder result = CreateInstance<ISqlBuilder>(GetClassName(currentConnectionConfig.DbType, "Builder"));
            return result;
        }

        public static IDbBind GetDbBind(ConnectionConfig currentConnectionConfig)
        {
            IDbBind result = CreateInstance<IDbBind>(GetClassName(currentConnectionConfig.DbType, "DbBind"));
            return result;
        }

        public static IDbMaintenance GetDbMaintenance(ConnectionConfig currentConnectionConfig)
        {
            IDbMaintenance result = CreateInstance<IDbMaintenance>(GetClassName(currentConnectionConfig.DbType, "DbMaintenance"));
            return result;
        }

        public static IDbFirst GetDbFirst(ConnectionConfig currentConnectionConfig)
        {
            IDbFirst result = CreateInstance<IDbFirst>(GetClassName(currentConnectionConfig.DbType, "DbFirst"));
            return result;
        }

        public static ICodeFirst GetCodeFirst(ConnectionConfig currentConnectionConfig)
        {
            ICodeFirst result = CreateInstance<ICodeFirst>(GetClassName(currentConnectionConfig.DbType, "CodeFirst"));
            return result;
        }

        public static IAdo GetAdo(ConnectionConfig currentConnectionConfig)
        {
            IAdo result = CreateInstance<IAdo>(GetClassName(currentConnectionConfig.DbType, "Provider"));
            return result;
        }

        private static string GetClassName(string type, string name)
        {
            return PubConst.AssemblyName + "." + type + name;
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
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5),typeof(T6));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, Restult>(string className)
        {
            return CreateInstance< Restult > (className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5),typeof(T6),typeof(T7));
        }
        private static Restult CreateInstance<T, T2, T3, T4, T5, T6, T7, T8, Restult>(string className)
        {
            return CreateInstance<Restult>(className, typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7),typeof(T8));
        }
        private static Restult CreateInstance<Restult>(string className, params Type[] types)
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
        private static T CreateInstance<T>(string className)
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
        #endregion
    }
}
