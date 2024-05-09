using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{

    /// <summary>
    /// AppendSelect
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private bool AppendSelect(JToken item)
        {
            bool isSelect = true;
            if (item.Type == JTokenType.Property)
            {
                var value = item.First().ToString();
                var obj = context.Utilities.JsonToSelectModels(value);
                obj =FilterSelect(obj);
                sugarQueryable.Select(obj);
            }
            else 
            {
                var obj = context.Utilities.JsonToSelectModels(item.ToString());
                obj = FilterSelect(obj);
                sugarQueryable.Select(obj);
            }
            return isSelect;
        }

        private List<SelectModel> FilterSelect(List<SelectModel> obj)
        {
            if (!this.jsonTableConfigs.Any()) 
            {
                return obj;
            }
            List<SelectModel> result = new List<SelectModel>();
            foreach (var item in obj) 
            {
                if (item.FieldName is string)
                {
                    var tableName = GetTableName(item.FieldName + "");
                    var columnName = GetColumnName(item.FieldName + "");
                    if (IsMyColums(tableName, columnName)) 
                    {
                        result.Add(item);
                    }
                }
                else
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private bool IsMyColums(string tableName, string columnName)
        {
            return this.jsonTableConfigs.Any(it => it.TableName.EqualCase(tableName)
                                && it.Columns.Any(z => z.Name.EqualCase(columnName)));
        }

        private string GetColumnName(string filedName)
        {
            return filedName.Split('.').Last();
        }

        private string GetTableName(string filedName)
        {
            if (!filedName.Contains("."))
            {
                return TableInfos.First(it => it.IsMaster).Table;
            }
            else 
            {
                var shortName=filedName.Split('.').First();
                return TableInfos.First(it => it.ShortName==shortName ).Table;
            }
        }
    }
}
