using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    internal class SplitTableHelper
    {
        public SqlSugarProvider Context { get; set; }
        public EntityInfo EntityInfo { get; set; }
        public List<string> GetTables()
        {

            var tableInfos = this.Context.DbMaintenance.GetTableInfoList(false);
            var regex = EntityInfo.DbTableName.Replace("{year}", "[0-9]{4}").Replace("{day}", "[0-9]{2}").Replace("{month}", "[0-9]{4}");
            var currentTables = tableInfos.Where(it => Regex.IsMatch(it.Name, regex, RegexOptions.IgnoreCase)).Select(it => it.Name).Reverse().ToList();
            return currentTables;
        }

        public string GetDefaultTableName()
        {
            throw new NotImplementedException();
        }
    }
}
