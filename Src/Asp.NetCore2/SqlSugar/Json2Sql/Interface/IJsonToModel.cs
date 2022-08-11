using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// Json to model
    /// </summary>
    public partial interface IContextMethods
    {
         List<OrderByModel> JsonToOrderByModels(string json);
         List<GroupByModel> JsonToGroupByModels(string json);
         List<Dictionary<string, object>> JsonToColumnsModels(string json);
         List<SelectModel> JsonToSelectModels(string json);
         IFuncModel JsonToSqlFuncModels(string json);
         JoinModel JsonToJoinModels(string json);
    }
}
