using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    /// <summary>
    /// RegisterAop
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private void RegisterAop()
        {
            this.BeforeWhereFunc = () =>
            {
                var masterTable = GetMasterTable();
                var masterFilters = this.jsonTableConfigs.Where(it => it.TableName.EqualCase(masterTable.Table)).ToList();
                if (masterFilters.Any())
                {
                    foreach (var filter in masterFilters)
                    {
                        var conditions = filter.Conditionals;
                        conditions=GetConvertConditions(conditions);
                        var p = this.sugarQueryable.SqlBuilder.ConditionalModelToSql(conditions);
                        var sql = p.Key;
                        sugarQueryable.SqlBuilder.RepairReplicationParameters(ref sql, p.Value, appendIndex);
                        appendIndex++;
                        sugarQueryable.Where(sql, p.Value);
                    };

                }
            };
        }

        private List<IConditionalModel> GetConvertConditions(List<IConditionalModel> conditions)
        {
            return conditions;
        }
    }
}
