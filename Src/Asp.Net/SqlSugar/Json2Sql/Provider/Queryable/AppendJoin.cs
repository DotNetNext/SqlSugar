using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    /// <summary>
    /// AppendJoin
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private bool AppendJoin(JToken item)
        {
            BeforeJoin();
            bool isJoin = true;
            var value = item.First().ToString();
            var obj = context.Utilities.JsonToJoinModels(value);
            sugarQueryable.AddJoinInfo(obj.TableName, obj.ShortName, obj.OnWhereList, obj.JoinType);
            AddTableInfos(obj.TableName,obj.ShortName);
            AfterJoin();
            return isJoin;
        }

        private void AfterJoin()
        {
            
        }

        private void BeforeJoin()
        {
            
        }
    }
}
