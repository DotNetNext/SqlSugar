using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    /// <summary>
    /// JsonQueryableProvider
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {

        public JsonQueryableProvider(ISqlSugarClient context, JObject jobject)
        {
            this.jobject = jobject;
            this.context = context;
            this.jsonCommonProvider = new JsonCommonProvider(context);
        }
        public IJsonQueryableProvider<JsonQueryResult> ShowDesciption() 
        {
            this.IsDescription = true;
            return this;
        }
        public IJsonQueryableProvider<JsonQueryResult> UseAuthentication(JsonTableConfig config) 
        {
            if (config == null)
            {
                jsonTableConfigs =new List<JsonTableConfig>() { config };
            }
            else
            {
                jsonTableConfigs.Add(config);
            }
            return this;
        }
        public IJsonQueryableProvider<JsonQueryResult> UseAuthentication(List<JsonTableConfig> configs) 
        {
            foreach (JsonTableConfig config in configs) 
            {
                UseAuthentication(config);
            }
            return this;
        }
       
        public SqlObjectResult ToSql()
        {
            return this.ToSqlList().First();
        }
        public JsonQueryResult ToResult()
        {
            return ToResultDefault();
        }

        public List<SqlObjectResult> ToSqlList()
        {
            var result = ToSqlDefault();
            return result;
        }

        public List<string> ToSqlString()
        {
            throw new NotImplementedException();
        }

        private void AppendQueryableAll(JsonQueryParameter jsonQueryParameter, JToken item)
        {
            SetQueryableParameterIndex(); 
            var name = item.Path.ToLower();
            if (IsForm(name))
            {
                AppendFrom(item);
            }
            else if (IsWhere(name))
            {
                AppendWhere(item);
            }
            else if (IsOrderBy(name))
            {
                AppendOrderBy(item);
            }
            else if (IsJoinLastAfter(name))
            {
                ApendJoinLastAfter(item);
            }
            else if (IsGroupBy(name))
            {
                AppendGroupBy(item);
            }
            else if (IsHaving(name))
            {
                AppendHaving(item);
            }
            else if (IsSelect(name))
            {
                jsonQueryParameter.IsSelect = AppendSelect(item);
            }
            else if (IsPageNumber(name))
            {
                jsonQueryParameter.PageIndex = AppendPageNumber(item);
            }
            else if (IsPageSize(name))
            {
                jsonQueryParameter.PageSize = AppendPageSize(item);
            }
            else if (IsJoin(name))
            {
                jsonQueryParameter.IsSelect = AppendJoin(item);
            }
        }

        private void SetQueryableParameterIndex()
        {
            ((SqlBuilderProvider)sugarQueryable.SqlBuilder).GetParameterNameIndex = jsonCommonProvider.ParameterIndex;
        }
    }
}
