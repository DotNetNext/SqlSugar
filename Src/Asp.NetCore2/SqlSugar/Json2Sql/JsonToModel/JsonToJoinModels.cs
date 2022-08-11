using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    /// <summary>
    /// Json to model
    /// </summary>
    public partial class ContextMethods : IContextMethods
    {

        public JoinModel JsonToJoinModels(string json)
        {
            JoinModel conditionalModels = new JoinModel();
            var array = JArray.Parse(json);
            Check.Exception(array.Count != 3, json + " format error");
            var tableName = array[0];
            var shortName = array[1];
            var onWhere = array[2];
            JoinModel result = new JoinModel();
            result.TableName = tableName.ObjToString().ToCheckField();
            result.ShortName = shortName.ObjToString().ToCheckField();
            result.OnWhereList = JsonToSqlFuncModels(onWhere);
            return result;
        }
    }
}
