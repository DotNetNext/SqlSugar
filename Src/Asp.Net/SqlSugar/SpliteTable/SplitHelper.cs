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
        public List<SplitTableInfo> GetTables()
        {

            var oldIsEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            var tableInfos = this.Context.DbMaintenance.GetTableInfoList(false);
            SplitTableHelper.CheckTableName(EntityInfo.DbTableName);
            var regex = EntityInfo.DbTableName.Replace("{year}", "([0-9]{2,4})").Replace("{day}", "([0-9]{1,2})").Replace("{month}", "([0-9]{1,2})");
            var currentTables = tableInfos.Where(it => Regex.IsMatch(it.Name, regex, RegexOptions.IgnoreCase)).Select(it => it.Name).Reverse().ToList();
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in currentTables)
            {
                SplitTableInfo tableInfo = new SplitTableInfo();
                tableInfo.TableName = item;
                var math = Regex.Match(item, regex);
                var group1 = math.Groups[1].Value;
                var group2 = math.Groups[2].Value;
                var group3 = math.Groups[3].Value;
                tableInfo.Date = GetDate(group1,group2,group3, EntityInfo.DbTableName);
                result.Add(tableInfo);
            }
            result = result.OrderByDescending(it => it.Date).ToList();
            this.Context.Ado.IsEnableLogEvent = oldIsEnableLogEvent;
            return result;
        }

        #region Helper
        private DateTime GetDate(string group1, string group2, string group3, string dbTableName)
        {
            var yearIndex = dbTableName.IndexOf("{year}");
            var dayIndex = dbTableName.IndexOf("{day}");
            var monthIndex = dbTableName.IndexOf("{month}");
            List<SplitTableSort> tables = new List<SplitTableSort>();
            tables.Add(new SplitTableSort() { Name = "{year}", Sort = yearIndex });
            tables.Add(new SplitTableSort() { Name = "{day}", Sort = dayIndex });
            tables.Add(new SplitTableSort() { Name = "{month}", Sort = monthIndex });
            tables = tables.OrderBy(it => it.Sort).ToList();
            var year = "";
            var month = "";
            var day = "";
            if (tables[0].Name == "{year}")
            {
                year = group1;
            }
            if (tables[1].Name == "{year}")
            {
                year = group2;
            }
            if (tables[2].Name == "{year}")
            {
                year = group3;
            }
            if (tables[0].Name == "{month}")
            {
                month = group1;
            }
            if (tables[1].Name == "{month}")
            {
                month = group2;
            }
            if (tables[2].Name == "{month}")
            {
                month = group3;
            }
            if (tables[0].Name == "{day}")
            {
                day = group1;
            }
            if (tables[1].Name == "{day}")
            {
                day = group2;
            }
            if (tables[2].Name == "{day}")
            {
                day = group3;
            }
            return Convert.ToDateTime($"{year}-{month}-{day}");
        }

        public string GetDefaultTableName()
        {
            var date = this.Context.GetDate();
            var result = EntityInfo.DbTableName.Replace("{year}", date.Year + "").Replace("{day}", PadLeft2(date.Day + "")).Replace("{month}", PadLeft2(date.Month + ""));
            return result;
        }

        private string PadLeft2(string str)
        {
            if (str.Length < 2)
            {
                return str.PadLeft(2, '0');
            }
            else
            {
                return str;
            }
        }

        private static void CheckTableName(string dbTableName)
        {
            Check.Exception(!dbTableName.Contains("{year}"), ErrorMessage.GetThrowMessage("table name need {year}", "分表表名需要占位符 {year}"));
            Check.Exception(!dbTableName.Contains("{month}"), ErrorMessage.GetThrowMessage("table name need {month}", "分表表名需要占位符 {month} "));
            Check.Exception(!dbTableName.Contains("{day}"), ErrorMessage.GetThrowMessage("table name need {day}", "分表表名需要占位符{day}"));
            Check.Exception(Regex.Matches(dbTableName, @"\{year\}").Count > 1, ErrorMessage.GetThrowMessage(" There can only be one {year}", " 只能有一个 {year}"));
            Check.Exception(Regex.Matches(dbTableName, @"\{month\}").Count > 1, ErrorMessage.GetThrowMessage("There can only be one {month}", "只能有一个 {month} "));
            Check.Exception(Regex.Matches(dbTableName, @"\{day\}").Count > 1, ErrorMessage.GetThrowMessage("There can only be one {day}", "只能有一个{day}"));
            Check.Exception(Regex.IsMatch(dbTableName, @"\d\{|\}\d"), ErrorMessage.GetThrowMessage(" '{' or  '}'  can't be numbers nearby", "占位符相令一位不能是数字,比例错误:1{day}2,正确: 1_{day}_2"));
        } 
        #endregion
    }
}
