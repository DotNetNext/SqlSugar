using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheKeyBuider
    {
        public static CacheKey GetKey(SqlSugarProvider context, QueryBuilder queryBuilder)
        {
            CacheKey result = new CacheKey();
            result.Database = context.Context.Ado.Connection.Database;
            AddTables(context, queryBuilder, result);
            AddIdentificationList(queryBuilder, result);
            return result;
        }

        private static void AddIdentificationList(QueryBuilder queryBuilder, CacheKey result)
        {
            result.IdentificationList = new List<string>();
            result.IdentificationList.Add(queryBuilder.GetTableNameString);
            result.IdentificationList.Add(queryBuilder.GetJoinValueString);
            result.IdentificationList.Add(queryBuilder.GetOrderByString);
            result.IdentificationList.Add(queryBuilder.GetGroupByString);
            result.IdentificationList.Add(queryBuilder.GetWhereValueString);
            result.IdentificationList.Add(queryBuilder.PartitionByValue);
            result.IdentificationList.Add(queryBuilder.Take.ObjToString());
            result.IdentificationList.Add(queryBuilder.Skip.ObjToString());
            result.IdentificationList.Add(queryBuilder.IsCount.ObjToString());
            result.IdentificationList.Add(UtilMethods.GetMD5(queryBuilder.GetSelectValue.ObjToString()));
            if (queryBuilder.Parameters.HasValue())
            {
                foreach (var item in queryBuilder.Parameters)
                {
                    result.IdentificationList.Add(item.ParameterName + "_" + item.Value);
                }
            }
        }

        private static void AddTables(ISqlSugarClient context, QueryBuilder queryBuilder, CacheKey result)
        {
            result.Tables = new List<string>();
            result.Tables.Add(context.EntityMaintenance.GetTableName(queryBuilder.EntityName));
            if (queryBuilder.EasyJoinInfos.HasValue())
            {
                foreach (var item in queryBuilder.EasyJoinInfos)
                {
                    result.Tables.Add(context.EntityMaintenance.GetTableName(item.Value));
                }
            }
            if (queryBuilder.JoinQueryInfos.HasValue())
            {
                foreach (var item in queryBuilder.JoinQueryInfos)
                {
                    result.Tables.Add(queryBuilder.Builder.GetNoTranslationColumnName(item.TableName));
                }
            }
        }
    }
}
