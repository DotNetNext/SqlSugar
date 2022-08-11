using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public interface IJsonQueryableProvider<JsonQueryResult> : IJsonProvider<JsonQueryResult>
    {
        IJsonQueryableProvider<JsonQueryResult> ShowDesciption();
        IJsonQueryableProvider<JsonQueryResult> UseAuthentication(JsonTableConfig config);
        IJsonQueryableProvider<JsonQueryResult> UseAuthentication(List<JsonTableConfig> config);
    }
}
