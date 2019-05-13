using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DbFirstExtensions
{
    public class RazorService : IRazorService
    {
        public List<KeyValuePair<string,string>> GetClassStringList(string razorTemplate, List<RazorTableInfo> model)
        {
            if (model != null && model.Any())
            {
                var  result = new List<KeyValuePair<string, string>>();
                foreach (var item in model)
                {
                    try
                    {
                        item.ClassName = item.DbTableName;//Format Class Name
                        string key = "RazorService.GetClassStringList"+ razorTemplate.Length;
                        var classString = Engine.Razor.RunCompile(razorTemplate, key, item.GetType(), item);
                        result.Add(new KeyValuePair<string,string>(item.ClassName,classString));
                    }
                    catch (Exception ex)
                    {
                        new Exception(item.DbTableName + " error ." + ex.Message);
                    }
                }
                return result;
            }
            else
            {
                return new List<KeyValuePair<string, string>> ();
            }
        }
    }
}
