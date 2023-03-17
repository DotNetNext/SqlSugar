using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// SqlFunc to model
    /// </summary>
    public partial class ContextMethods : IContextMethods
    {
        #region Root
        public ObjectFuncModel JsonToSqlFuncModels(JToken sqlfunc)
        {
            var key = sqlfunc.First();
            if (IsObjct(sqlfunc))
            {
                return GetFuncModelByObject(key);
            }
            else
            {
                return GetFuncModelByArray(sqlfunc);
            }
        }
        public IFuncModel JsonToSqlFuncModels(string sqlfunc)
        {
            if (IsArray(sqlfunc))
            {
                return GetFuncModelByArray(sqlfunc);
            }
            else
            {
                return GetFuncModelByObject(sqlfunc);
            }
        }
        #endregion

        #region Level 1
        private ObjectFuncModel GetFuncModelByArray(JToken sqlfunc)
        {
            ObjectFuncModel result = new ObjectFuncModel();
            result.Parameters = new List<object>();
            result.FuncName = "Sqlfunc_Format";
            StringBuilder sb = new StringBuilder();
            foreach (var item in sqlfunc)
            {
                result.Parameters.Add(GetParameter(item));
            }
            return result;
        }
        private ObjectFuncModel GetFuncModelByObject(JToken key)
        {
            ObjectFuncModel result = new ObjectFuncModel();
            JProperty jProperty = key.ToObject<JProperty>();
            result.FuncName = jProperty.Name;
            var parameters = jProperty.Value;
            result.Parameters = GetParameter(parameters);
            return result;
        }
        private IFuncModel GetFuncModelByObject(string sqlfunc)
        {
            var jObject = this.Context.Utilities.DeserializeObject<JObject>(sqlfunc);
            return JsonToSqlFuncModels(jObject);
        }
        private IFuncModel GetFuncModelByArray(string sqlfunc)
        {
            ObjectFuncModel result = new ObjectFuncModel();
            result.Parameters = new List<object>();
            result.FuncName = "Sqlfunc_Format";
            var jArrary = this.Context.Utilities.DeserializeObject<JArray>(sqlfunc);
            StringBuilder sb = new StringBuilder();
            foreach (var item in jArrary)
            {
                result.Parameters.Add(GetParameter(item));
            }
            return result;
        }

        #endregion

 

    }
}
