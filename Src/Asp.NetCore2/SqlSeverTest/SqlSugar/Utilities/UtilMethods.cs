using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class UtilMethods
    {
        internal static Type GetUnderType(Type oldType)
        {
            Type type = Nullable.GetUnderlyingType(oldType);
            return type==null ? oldType : type;
        }

        internal static Type GetRootBaseType(Type entityType)
        {
            var baseType = entityType.BaseType;
            while (baseType != null && baseType.BaseType != UtilConstants.ObjType)
            {
                baseType = baseType.BaseType;
            }
            return baseType;
        }


        internal static Type GetUnderType(PropertyInfo propertyInfo, ref bool isNullable)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            isNullable = unType != null;
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }

        internal static Type GetUnderType(PropertyInfo propertyInfo)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            unType = unType ?? propertyInfo.PropertyType;
            return unType;
        }

        internal static bool IsNullable(PropertyInfo propertyInfo)
        {
            Type unType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
            return unType != null;
        }

        internal static T IsNullReturnNew<T>(T returnObj) where T : new()
        {
            if (returnObj.IsNullOrEmpty())
            {
                returnObj = new T();
            }
            return returnObj;
        }

        internal static T ChangeType<T>(T obj, Type type)
        {
            return (T)Convert.ChangeType(obj, type);
        }

        internal static T ChangeType<T>(T obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        internal static void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex)
        {
            if (appendSql.HasValue() && parameters.HasValue())
            {
                foreach (var parameter in parameters.OrderByDescending(it => it.ParameterName.Length))
                {
                    //Compatible with.NET CORE parameters case
                    var name = parameter.ParameterName;
                    string newName = name + addIndex;
                    appendSql = appendSql.Replace(name, newName);
                    parameter.ParameterName = newName;
                }
            }
        }

        internal static string GetPackTable(string sql, string shortName)
        {
            return string.Format(" ({0}) {1} ", sql, shortName);
        }

        internal static string GetParenthesesValue(string dbTypeName)
        {
            if (Regex.IsMatch(dbTypeName, @"\(.+\)"))
            {
                dbTypeName = Regex.Replace(dbTypeName, @"\(.+\)", "");
            }
            dbTypeName = dbTypeName.Trim();
            return dbTypeName;
        }

        internal static T GetOldValue<T>(T value, Action action)
        {
            action();
            return value;
        }
    }
}
