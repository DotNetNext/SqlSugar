using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    internal class CacheKeyBuider
    {
        public static CacheKey GetKey(SqlSugarClient context, QueryBuilder queryBuilder)
        {
            CacheKey result = new CacheKey();
            result.Database = context.Context.Ado.Connection.Database;
            result.Tables = new List<string>();
            result.Tables.Add(context.EntityMaintenance.GetTableName(queryBuilder.EntityName));
            result.Tables.AddRange(queryBuilder.JoinQueryInfos.Select(it=>it.TableName));
            result.IdentificationList = new List<string>();
            result.IdentificationList.Add(queryBuilder.GetTableNameString);
            result.IdentificationList.Add(queryBuilder.GetJoinValueString);
            result.IdentificationList.Add(queryBuilder.GetOrderByString);
            result.IdentificationList.Add(queryBuilder.GetGroupByString);
            result.IdentificationList.Add(queryBuilder.GetWhereValueString);
            result.IdentificationList.Add(queryBuilder.PartitionByValue);
            result.IdentificationList.Add(queryBuilder.Take.ObjToString());
            result.IdentificationList.Add(queryBuilder.Skip.ObjToString());
            return result;
        }
    }
}
