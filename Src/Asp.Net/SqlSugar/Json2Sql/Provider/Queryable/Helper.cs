using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    /// <summary>
    /// Helper
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private static bool IsJoin(string name)
        {
            return name.StartsWith("LeftJoin".ToLower()) || name.StartsWith("RightJoin".ToLower()) || name.StartsWith("InnertJoin".ToLower());
        }
        private static bool IsJoinLastAfter(string name)
        {
            return name== "joinlastafter";
        }

        private static bool IsPageSize(string name)
        {
            return name == "PageSize".ToLower();
        }

        private static bool IsPageNumber(string name)
        {
            return name == "PageNumber".ToLower();
        }

        private static bool IsSelect(string name)
        {
            return name == "Select".ToLower();
        }

        private static bool IsHaving(string name)
        {
            return name == "Having".ToLower();
        }

        private static bool IsGroupBy(string name)
        {
            return name == "GroupBy".ToLower();
        }

        private static bool IsOrderBy(string name)
        {
            return name == "OrderBy".ToLower();
        }

        private static bool IsWhere(string name)
        {
            return name == "Where".ToLower();
        }

        private static bool IsForm(string name)
        {
            return name == JsonProviderConfig.KeyQueryable.Get().ToLower();
        }

        private static bool IsAnySelect(List<JToken> appendTypeNames)
        {
            return appendTypeNames.Any(it => IsSelect(it.Path.ToLower()));
        }
        private static bool IsAnyJoin(List<JToken> appendTypeNames)
        {
            return appendTypeNames.Any(it => IsJoin(it.Path.ToLower()));
        }
        private int GetSort(string name)
        {
            if (IsForm(name))
            {
                return 0;
            }
            else if (IsJoin(name))
            {
                return 1;
            }
            else if (IsJoinLastAfter(name)) 
            {
                return 2;
            }
            else
            {
                return 100;
            }
        }
        private void AddMasterTableInfos(JsonTableNameInfo tableNameInfo)
        {
            AddTableInfos(tableNameInfo.TableName, tableNameInfo.ShortName,true);
        }
        private void AddTableInfos(string tableName,string shortName,bool isMaster=false)
        {
            UtilMethods.IsNullReturnNew(TableInfos);
            TableInfos.Add(new JsonQueryableProvider_TableInfo() { Table = tableName, ShortName = shortName, IsMaster = true });
        }
        private JsonQueryableProvider_TableInfo GetMasterTable()
        {
            return this.TableInfos.First(it => it.IsMaster);
        }
    }
}
