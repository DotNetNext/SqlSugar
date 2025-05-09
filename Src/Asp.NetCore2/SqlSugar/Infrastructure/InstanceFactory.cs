﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{

    public class InstanceFactory
    {
        static Assembly assembly = Assembly.GetExecutingAssembly();
        static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
        private static string _CustomDllName = "";
        private static List<string> CustomDlls = new List<string>();
        public static Assembly[] CustomAssemblies = new Assembly[]{};
        public static string CustomDllName {
            get { return _CustomDllName; }
            set 
            {
                if (!CustomDlls.Contains(value)) 
                {
                    CustomDlls.Add(value);
                }
                _CustomDllName = value;
            }
        }
        public static string CustomDbName = "";
        public static string CustomNamespace = "";
        public static bool NoCache = false;
        public static bool IsWebFrom = false;
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
            else if (currentConnectionConfig.DbType == DbType.MySql)
            {
                return new MySqlQueryable<T>();
            }
            else if (currentConnectionConfig.DbType == DbType.Sqlite)
            {
                return new SqliteQueryable<T>();
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

        public static QueryBuilder GetQueryBuilderWithContext(ISqlSugarClient db)
        {
            if (db is SqlSugarClient)
            {
                db = (db as SqlSugarClient).Context;
            }
            else if (db is SqlSugarScope)
            {
                db = (db as SqlSugarScope).ScopedContext.Context;
            }
            if (!(db is SqlSugarProvider)) 
            {
                db = new SqlSugarClient(db.CurrentConnectionConfig).Context;
            }
            var QueryBuilder = InstanceFactory.GetQueryBuilder(db.CurrentConnectionConfig);
            QueryBuilder.Context = (SqlSugarProvider)db;
            QueryBuilder.Builder = InstanceFactory.GetSqlbuilder(db.CurrentConnectionConfig);
            QueryBuilder.Builder.Context = (SqlSugarProvider)db;
            return QueryBuilder;
        }

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
        public static ISqlBuilder GetSqlBuilderWithContext(ISqlSugarClient db) 
        {
           var result= GetQueryBuilderWithContext(db).Builder;
            return result;
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
            else if (IsCustomDb(currentConnectionConfig))
            {
                var name =
                    "SqlSugar." + currentConnectionConfig.DbType +
                    "." + currentConnectionConfig.DbType
                    + "Updateable`1";
                var type = GetCustomTypeByClass<T>(name);
                if (type == null)
                {
                    name =
                    InstanceFactory.CustomNamespace +
                    "." + InstanceFactory.CustomDbName
                    + "Updateable`1";
                    type = GetCustomTypeByClass<T>(name);
                }
                if (type == null)
                {
                    return new UpdateableProvider<T>();
                }
                else
                {
                    return (UpdateableProvider<T>)Activator.CreateInstance(type, true);
                }
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
            else if (IsCustomDb(currentConnectionConfig))
            {
                var name =
                    "SqlSugar." + currentConnectionConfig.DbType +
                    "." + currentConnectionConfig.DbType
                    + "Deleteable`1";
                var type = GetCustomTypeByClass<T>(name);
                if (type == null)
                {
                    name =
                    InstanceFactory.CustomNamespace +
                    "." + InstanceFactory.CustomDbName
                    + "Deleteable`1";
                    type = GetCustomTypeByClass<T>(name);
                }
                if (type == null)
                {
                    return new DeleteableProvider<T>();
                }
                else
                {
                    return (DeleteableProvider<T>)Activator.CreateInstance(type, true);
                }
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
            else if (currentConnectionConfig.DbType == DbType.Oscar)
            {
                return new KdbndpInserttable<T>();
            }
            else if (IsCustomDb(currentConnectionConfig))
            {
                var name =
                    "SqlSugar." + currentConnectionConfig.DbType +
                    "." + currentConnectionConfig.DbType
                    + "Insertable`1";
                var type = GetCustomTypeByClass<T>(name);
                if (type == null) 
                {
                    name =
                    InstanceFactory.CustomNamespace +
                    "." + InstanceFactory.CustomDbName
                    + "Insertable`1";
                    type = GetCustomTypeByClass<T>(name);
                }
                if (type == null)
                {
                    return new InsertableProvider<T>();
                }
                else
                {
                    return (InsertableProvider<T>)Activator.CreateInstance(type, true);
                }
            }
            else
            { 
                return new InsertableProvider<T>();
            }
        }

        private static bool IsCustomDb(ConnectionConfig currentConnectionConfig)
        {
            if (currentConnectionConfig.DbType == DbType.Custom) 
            {
                return true;
            }
            return currentConnectionConfig.DbType != DbType.SqlServer &&
                            currentConnectionConfig.DbType != DbType.Dm &&
                            currentConnectionConfig.DbType != DbType.Oscar &&
                            currentConnectionConfig.DbType != DbType.Access &&
                            currentConnectionConfig.DbType != DbType.QuestDB &&
                            currentConnectionConfig.DbType != DbType.MySql &&
                            currentConnectionConfig.DbType != DbType.Oracle &&
                            currentConnectionConfig.DbType != DbType.PostgreSQL &&
                            currentConnectionConfig.DbType != DbType.ClickHouse &&
                            currentConnectionConfig.DbType != DbType.GBase &&
                            currentConnectionConfig.DbType != DbType.Sqlite &&
                            GetCustomTypeByClass("SqlSugar." + currentConnectionConfig.DbType + "." + currentConnectionConfig.DbType + "Provider") != null;
      
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
            if (type == "MySqlConnector")
            {
                return "SqlSugar.MySqlConnector.MySql" + name;
            }
            else if (type == "Access")
            {
                return "SqlSugar.Access.Access" + name;
            }
            else if (type == "ClickHouse")
            {
                return "SqlSugar.ClickHouse.ClickHouse" + name;
            }
            else if (type == "GBase")
            {
                return "SqlSugar.GBase.GBase" + name;
            }
            else if (type == "Odbc")
            {
                return "SqlSugar.Odbc.Odbc" + name;
            }
            else if (type == "Custom")
            {
                return CustomNamespace + "."+CustomDbName + name;
            }
            else if (type == "HANA")
            {
                return InstanceFactory.CustomDllName + "." + type + name;
            }
            else if (type == "DB2")
            {
                return "SqlSugar.DB2."+ type+ name;
            }
            else if (type == "GaussDBNative") 
            {
                return "SqlSugar.GaussDB.GaussDB"  + name;
            }
            else
            {
                //if (!string.IsNullOrEmpty(CustomDllName)) 
                //{
                //    type = CustomDllName;
                //}
                return UtilConstants.AssemblyName + "." + type + name;
            }
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
            Type type=null;
            if (typeCache.ContainsKey(cacheKey))
            {
                type = typeCache[cacheKey];
            }
            else
            {
                lock (typeCache)
                {
                    if (string.IsNullOrEmpty(CustomDllName))
                    {
                        type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
                    }
                    else 
                    {
                        var custom = GetCustomTypeByClass(className + "`" + types.Length);
                        if (custom != null)
                        {
                            type = custom.MakeGenericType(types);
                        }
                        if (type == null) 
                        {
                            type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
                        }
                    }
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

            Type type = null;
            if (string.IsNullOrEmpty(CustomDllName))
            {
                type = Type.GetType(className + "`" + types.Length, true).MakeGenericType(types);
            }
            else 
            {
                var custom = GetCustomTypeByClass(className + "`" + types.Length);
                if (custom != null) 
                {
                    type = custom.MakeGenericType(types);
                }
                if (type == null) 
                {
                    type = Type.GetType(className + "`" + types.Length)?.MakeGenericType(types);
                    if (type == null) 
                    {
                        type = GetCustomDbType(className + "`" + types.Length, type).MakeGenericType(types);
                    }
                }
            }
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
                    if (string.IsNullOrEmpty(CustomDllName))
                    {
                        type = assembly.GetType(className);
                    }
                    else 
                    {
                        type= GetCustomTypeByClass(className);
                        if (type == null) 
                        {
                            type = assembly.GetType(className);
                        }
                    }
                    if (type == null)
                    {
                        type = GetCustomDbType(className, type);
                    }
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
            Type type = null;
            if (string.IsNullOrEmpty(CustomDllName))
            {
                type=assembly.GetType(className);
            }
            else
            {
                type = GetCustomTypeByClass(className);
            }
            if (type == null)
            {
                type = GetCustomDbType(className, type);
            }
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }

        private static Type GetCustomDbType(string className, Type type)
        {
            if (className.Replace(".", "").Length + 1 == className.Length)
            {
                var array = className.Split('.');
                foreach (var item in UtilMethods.EnumToDictionary<DbType>())
                {
                    if (array.Last().StartsWith(item.Value.ToString()))
                    {

                        var newName = array.First() + "." + item.Value.ToString() + "." + array.Last();
                        type = GetCustomTypeByClass(newName);
                        break;
                    }
                }

            }

            return type;
        }

        internal static Type GetCustomTypeByClass(string className)
        {
            Type type = null;
            foreach (var item in CustomDlls.ToArray())
            {
                if (type == null)
                {
                    type = GetCustomTypeByClass(className, item);
                }
                if(type != null) 
                {
                    break;
                }
            }
            return type;
        }
        internal static Type GetCustomTypeByClass(string className,string customDllName)
        {
            var key = "Assembly_" + customDllName + assembly.GetHashCode();
            var newAssembly = new ReflectionInoCacheService().GetOrCreate<Assembly>(key, () => {
                try
                {
                    if (CustomAssemblies?.Any(it => it.FullName.StartsWith(customDllName))==true) 
                    {
                        return CustomAssemblies?.First(it => it.FullName.StartsWith(customDllName));
                    }
                    var path = Assembly.GetExecutingAssembly().Location;
                    if (path.HasValue())
                    {
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), customDllName + ".dll");
                    }
                    if (path.HasValue() && FileHelper.IsExistFile(path))
                    {
                        return Assembly.LoadFrom(path);
                    }
                    else
                    {
                        if (IsWebFrom)
                        {
                            string newpath = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\" + customDllName + ".dll").Replace("file:\\", "");
                            return Assembly.LoadFrom(newpath);
                        }
                        return Assembly.LoadFrom(customDllName + ".dll");
                    }
                }
                catch
                {
                    var message = "Not Found " + customDllName + ".dll";
                    Check.Exception(true, message);
                    return null;
                }
            });
            Type type = newAssembly.GetType(className);
            if (type == null)
            {
                type = assembly.GetType(className);
            }
            return type;
        }
        internal static Type GetCustomTypeByClass<T>(string className)
        {
            Type type = null;
            foreach (var item in CustomDlls.ToArray())
            {
                if (type == null)
                {
                    type = GetCustomTypeByClass<T>(className, item);
                }
                if (type != null)
                {
                    break;
                }
            }
            return type;
        }
        internal static Type GetCustomTypeByClass<T>(string className,string customDllName)
        {
            var key = "Assembly_" + customDllName + assembly.GetHashCode();
            var newAssembly = new ReflectionInoCacheService().GetOrCreate<Assembly>(key, () => {
                try
                {
                    if (CustomAssemblies?.Any(it => it.FullName.StartsWith(customDllName)) == true)
                    {
                        return CustomAssemblies?.First(it => it.FullName.StartsWith(customDllName));
                    }
                    var path = Assembly.GetExecutingAssembly().Location;
                    if (path.HasValue())
                    {
                        path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), customDllName + ".dll");
                    }
                    if (path.HasValue() && FileHelper.IsExistFile(path))
                    {
                        return Assembly.LoadFrom(path);
                    }
                    else
                    {
                        if (IsWebFrom)
                        {
                            string newpath = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\" + customDllName + ".dll").Replace("file:\\", "");
                            return Assembly.LoadFrom(newpath);
                        }
                        return Assembly.LoadFrom(customDllName + ".dll");
                    }
                }
                catch
                {
                    var message = "Not Found " + customDllName + ".dll";
                    Check.Exception(true, message);
                    return null;
                }
            });
            Type typeArgument = typeof(T); 
            string fullTypeName = className + "[[" + typeArgument.FullName+","+ typeArgument.Assembly.FullName+ "]]";
            Type type = newAssembly.GetType(fullTypeName);
            return type;
        }
        #endregion
    }
}
