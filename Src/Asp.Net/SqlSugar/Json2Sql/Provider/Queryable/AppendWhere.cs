using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{

    /// <summary>
    /// AppendWhere
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private void AppendWhere(JToken item)
        {
            BeforeWhere();
            var sqlObj = jsonCommonProvider.GetWhere(item, sugarQueryable.Context);
            sugarQueryable.Where(sqlObj.Key, sqlObj.Value);
            AfterWhere();
        }

        private void AfterWhere()
        {
            
        }

        private  void BeforeWhere()
        {
            if (!IsExecutedBeforeWhereFunc)
            {
                BeforeWhereFunc();
                IsExecutedBeforeWhereFunc = true;
            }
        }
    }
}
