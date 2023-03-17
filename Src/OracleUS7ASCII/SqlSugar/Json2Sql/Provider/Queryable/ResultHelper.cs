using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// ResultHelper
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        #region SqlHelper
        private List<SqlObjectResult> ToPageDefault(List<SqlObjectResult> result, JsonQueryParameter jsonQueryParameter)
        {
            if (jsonQueryParameter.IsPage)
            {
                AddPageSql(result, jsonQueryParameter);
            }
            else
            {
                AddDefaultSql(result);
            }
            Check.ExceptionEasy(jsonQueryParameter.JoinNoSelect, "join query need Select", "联表查询需要设置Select");
            return result;
        }

        private void AddDefaultSql(List<SqlObjectResult> result)
        {
            result.Add(new SqlObjectResult(sugarQueryable.Clone().ToSql(), JsonProviderType.Queryable));
        }

        private void AddPageSql(List<SqlObjectResult> result, JsonQueryParameter jsonQueryParameter)
        {
            var skipValue = (jsonQueryParameter.PageIndex.Value - 1) * jsonQueryParameter.PageSize.Value;
            var takeValue = jsonQueryParameter.PageSize.Value;
            result.Add(new SqlObjectResult(sugarQueryable.Clone().Skip(skipValue).Take(takeValue).ToSql(), JsonProviderType.Queryable));
            var countQueryable = sugarQueryable.Select("COUNT(1)");
            countQueryable.QueryBuilder.OrderByValue = null;
            result.Add(new SqlObjectResult(countQueryable.ToSql(), JsonProviderType.QueryableCount));
        }
        #endregion

        #region ObjectHeper
        private void AddDescription()
        {
            if (this.IsDescription)
            {
            }
        }

        private void AddList(JsonQueryResult result, SqlObjectResult SqlList)
        {
            if (SqlList != null)
            {
                result.Data = this.context.Ado.SqlQuery<dynamic>(SqlList.Sql, SqlList.Parameters);
            }
        }

        private void AddCount(JsonQueryResult result, SqlObjectResult SqlCount)
        {
            if (SqlCount != null)
            {
                result.ToTalRows = this.context.Ado.GetInt(SqlCount.Sql, SqlCount.Parameters);
            }
        }
        #endregion
    }
}
