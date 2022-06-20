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
        public List<SelectModel> JsonToSelectModels(string json)
        {
            List<SelectModel> conditionalModels = new List<SelectModel>();
            var jarray = this.Context.Utilities.DeserializeObject<JArray>(json);
            foreach (var item in jarray)
            {
                if (IsFieldName(item))
                {
                    var model = item.ToObject<SelectModel>();
                    conditionalModels.Add(model);
                }
                else if (IsString(item))
                {
                    conditionalModels.Add(new SelectModel() { FiledName = item.ObjToString(), AsName = item.ObjToString().Replace(".", "_") });
                }
                else if (IsArraySingleItem(item))
                {
                    object fileName = item[0].ObjToString();
                    var asName = item[0].ObjToString().Replace(".", "_");
                    if (IsSqlFunc(item[0], fileName.ObjToString()))
                    {
                        fileName = JsonToSqlFuncModels(item[0]);
                    }
                    conditionalModels.Add(new SelectModel()
                    {
                        FiledName = fileName,
                        AsName = asName
                    });

                }
                else if (IsArray(item))
                {
                    object fileName = item[0].ObjToString();
                    var asName = item[1].ObjToString().Replace(".", "_");
                    if (IsSqlFunc(item[0], fileName.ObjToString()))
                    {
                        fileName = JsonToSqlFuncModels(item[0]);
                    }
                    conditionalModels.Add(new SelectModel()
                    {
                        FiledName = fileName,
                        AsName = asName
                    });

                }
                else
                {
                    conditionalModels.Add(new SelectModel()
                    {
                        FiledName = item.ObjToString().Trim(),
                        AsName = item.ObjToString().Trim()
                    });
                }

            }
            return conditionalModels;
        }
    }
}
