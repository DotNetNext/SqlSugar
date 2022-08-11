using System.Collections.Generic;

namespace SqlSugar
{
    public interface IJsonClient
    {
        ISqlSugarClient Context { get; set; }

        IJsonProvider<JsonDeleteResult> Deleteable(string json);
        List<string> GetTableNameList(string json);
        IJsonProvider<JsonInsertResult> Insertable(string json);
        IJsonQueryableProvider<JsonQueryResult> Queryable(string json);
        IJsonProvider<JsonUpdateResult> Updateable(string json);
    }
}