using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    /// <summary>
    /// AppendFrom
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {

        private void AppendFrom(JToken item)
        {
            var tableNameInfo=jsonCommonProvider.GetTableName(item);
            tableNameInfo.TableName.ToCheckField();
            AddMasterTableInfos(tableNameInfo);
            if (tableNameInfo.ShortName.HasValue())
            {
                this.sugarQueryable.AS(tableNameInfo.TableName, tableNameInfo.ShortName);
            }
            else 
            {
                this.sugarQueryable.AS(tableNameInfo.TableName, tableNameInfo.ShortName);
            }
        }
    }
}
