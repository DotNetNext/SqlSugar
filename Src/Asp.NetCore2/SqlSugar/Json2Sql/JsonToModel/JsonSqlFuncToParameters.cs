using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class ContextMethods : IContextMethods
    {
        #region Root
        public List<object> GetParameter(JToken parameters)
        {
            List<object> result = new List<object>();
            if (IsString(parameters))
            {
                result.Add(GetStringParameters(parameters));
            }
            else if (IsArray(parameters))
            {
                result.AddRange(GetArrayParameters(parameters));
            }
            else if (IsObject(parameters))
            {
                result.Add(GetObjectParameters(parameters));
            }
            else
            {
                result.Add(GetObjectErrorParameters(parameters));
            }
            return result;
        } 
        #endregion

        #region Level1
        private static List<object> GetObjectErrorParameters(JToken parameters)
        {
            Check.Exception(true, ErrorMessage.GetThrowMessage($" {parameters.ToString()} format is error ", $" {parameters.ToString()} 格式错误"));
            return null;
        }

        public List<object> GetArrayParameters(JToken parameters)
        {
            List<object> result = new List<object>();
            foreach (var item in parameters)
            {
                result.Add(GetParameter(item));
            }
            return result;
        }

        public object GetObjectParameters(JToken parameters)
        {
            return JsonToSqlFuncModels(parameters);
        }

        public object GetStringParameters(JToken parameters)
        {
            return parameters.ObjToString().ToCheckField();
        } 
        #endregion

      

    }
}
