using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SqlSugar.MongoDb
{
    public class MongoDbDeleteBuilder : DeleteBuilder
    {
        public override string ToSqlString()
        {
            var sb = new StringBuilder("deleteMany b ");
            var jsonObjects = new List<string>();
            foreach (var item in this.WhereInfos)
            {
                var key = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
                var startWithValue = $"{Builder.GetTranslationColumnName(key.DbColumnName)} IN (";
                if (item.StartsWith(startWithValue))
                {
                    var sql = item;
                    sql = sql.TrimEnd(')');
                    sql = sql.Replace(startWithValue, "").Replace("'", "");
                    var dict = new Dictionary<string, object>();
                    var array = sql.Split(",");
                    dict["_id"] = new Dictionary<string, object> { { "$in", array } }; // Fixed syntax for dictionary initialization
                    string json = JsonSerializer.Serialize(dict, new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });
                    jsonObjects.Add(json);
                }
            }
            sb.Append("[{\"filter\":");
            sb.Append(string.Join(", ", jsonObjects));
            sb.Append("}]");
            return sb.ToString();
        }
    }
}
