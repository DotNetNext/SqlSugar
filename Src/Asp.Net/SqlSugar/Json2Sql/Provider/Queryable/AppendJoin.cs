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
            sugarQueryable.AddJoinInfo(obj.TableName, obj.ShortName, obj.OnWhereList, GetJoinType(item));
            AddTableInfos(obj.TableName, obj.ShortName);
            AfterJoin();
            return isJoin;
        }

        private static JoinType GetJoinType(JToken obj)
        {
            var key = obj.Path.ToLower();
            if (key.Contains("right"))
            {
                return JoinType.Right;
            }
            else if (key.Contains("left"))
            {
                return JoinType.Left;
            }
            else if (key.Contains("full"))
            {
                return JoinType.Full;
            }
            else
            {
                return JoinType.Inner;
            }
        }

        private void AfterJoin()
        {
            
        }

        private void BeforeJoin()
        {
            
        }
    }
}
