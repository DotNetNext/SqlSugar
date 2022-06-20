using Newtonsoft.Json.Linq;
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
        private  bool IsObjct(JToken sqlfunc)
        {
            return sqlfunc.Type == JTokenType.Object;
        }
        private  bool IsArray(string sqlfunc)
        {
            return sqlfunc.StartsWith("[");
        }
        public static bool IsSqlFunc(JToken item, string fileName)
        {
            return item.Type == JTokenType.Object || fileName.ToLower().Contains("SqlFunc_");
        }
        private  bool IsObject(JToken parameters)
        {
            return parameters.Type == JTokenType.Object;
        }
        private  bool IsArray(JToken parameters)
        {
            return parameters.Type == JTokenType.Array;
        }
        private  bool IsString(JToken parameters)
        {
            return parameters.Type == JTokenType.String;
        }
        private bool IsFieldName(JToken item)
        {
            return item.ObjToString().ToLower().Contains("fieldname");
        }
        private bool IsArraySingleItem(JToken item)
        {
            return IsArray(item) && item.Count() == 1;
        }
    }
}
