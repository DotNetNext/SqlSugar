using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DateSplitTableService : ISplitTableService
    {
        #region Core
        public virtual List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            CheckTableName(EntityInfo.DbTableName);
            var regex = EntityInfo.DbTableName.Replace("{year}", "([0-9]{2,4})").Replace("{day}", "([0-9]{1,2})").Replace("{month}", "([0-9]{1,2})");
            var currentTables = tableInfos.Where(it => Regex.IsMatch(it.Name, regex, RegexOptions.IgnoreCase)).Select(it => it.Name).Reverse().ToList();
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in currentTables)
            {
                SplitTableInfo tableInfo = new SplitTableInfo();
                tableInfo.TableName = item;
                var math = Regex.Match(item, regex,RegexOptions.IgnoreCase);
                var group1 = math.Groups[1].Value;
                var group2 = math.Groups[2].Value;
                var group3 = math.Groups[3].Value;
                tableInfo.Date = GetDate(group1, group2, group3, EntityInfo.DbTableName);
                //tableInfo.String = null;  Time table, it doesn't work
                //tableInfo.Long = null;  Time table, it doesn't work
                result.Add(tableInfo);
            }
            result = result.OrderByDescending(it => it.Date).ToList();
            return result;
        }
        public virtual string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo)
        {
            var splitTableAttribute = EntityInfo.Type.GetCustomAttribute<SplitTableAttribute>();
            if (splitTableAttribute != null)
            {
                var type=(splitTableAttribute as SplitTableAttribute).SplitType;
                return GetTableName(db, EntityInfo, type);
            }
            else
            {
                return GetTableName(db, EntityInfo, SplitType.Day);
            }
        }
        public virtual string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo, SplitType splitType)
        {
            var date = db.GetDate();
            return GetTableNameByDate(EntityInfo, splitType, date);
        }
        public virtual string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            var value = Convert.ToDateTime(fieldValue);
            return GetTableNameByDate(entityInfo, splitType, value);
        }
        public virtual object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            if (splitColumn == null)
            {
                return db.GetDate();
            }
            else
            {
                var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
                if (value == null)
                {
                    return db.GetDate();
                }
                else if (UtilMethods.GetUnderType(value.GetType()) != UtilConstants.DateType)
                {
                    throw new Exception($"DateSplitTableService Split column {splitColumn.PropertyName} not DateTime " + splitType.ToString());
                }
                else if (Convert.ToDateTime(value) == DateTime.MinValue)
                {
                    return db.GetDate();
                }
                else
                {
                    return value;
                }
            }
        }
        public void VerifySplitType(SplitType splitType)
        {
            switch (splitType)
            {
                case SplitType.Day:
                    break;
                case SplitType.Week:
                    break;
                case SplitType.Month:
                    break;
                case SplitType.Season:
                    break;
                case SplitType.Year:
                    break;
                default:
                    throw new Exception("DateSplitTableService no support " + splitType.ToString());
            }
        }

        #endregion

        #region Common Helper
        private string GetTableNameByDate(EntityInfo EntityInfo,SplitType splitType,DateTime date)
        {
            date = ConvertDateBySplitType(date, splitType);
            return EntityInfo.DbTableName.Replace("{year}", date.Year + "").Replace("{day}", PadLeft2(date.Day + "")).Replace("{month}", PadLeft2(date.Month + ""));
        }

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
            Check.Exception(!dbTableName.Contains("{year}"), ErrorMessage.GetThrowMessage("table name need {{year}}", "分表表名需要占位符 {{year}}"));
            Check.Exception(!dbTableName.Contains("{month}"), ErrorMessage.GetThrowMessage("table name need {{month}}", "分表表名需要占位符 {{month}} "));
            Check.Exception(!dbTableName.Contains("{day}"), ErrorMessage.GetThrowMessage("table name need {{day}}", "分表表名需要占位符{{day}}"));
            Check.Exception(Regex.Matches(dbTableName, @"\{year\}").Count > 1, ErrorMessage.GetThrowMessage(" There can only be one {{year}}", " 只能有一个 {{year}}"));
            Check.Exception(Regex.Matches(dbTableName, @"\{month\}").Count > 1, ErrorMessage.GetThrowMessage("There can only be one {{month}}", "只能有一个 {{month}} "));
            Check.Exception(Regex.Matches(dbTableName, @"\{day\}").Count > 1, ErrorMessage.GetThrowMessage("There can only be one {{day}}", "只能有一个{{day}}"));
            Check.Exception(Regex.IsMatch(dbTableName, @"\d\{|\}\d"), ErrorMessage.GetThrowMessage(" '{{' or  '}}'  can't be numbers nearby", "占位符相令一位不能是数字,比如 : 1{{day}}2 错误 , 正确: 1_{{day}}_2"));
        }
        #endregion

        #region Date Helper
        private DateTime ConvertDateBySplitType(DateTime time, SplitType type)
        {
            switch (type)
            {
                case SplitType.Day:
                    return Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
                case SplitType.Week:
                    return GetMondayDate(time);
                case SplitType.Month:
                    return Convert.ToDateTime(time.ToString("yyyy-MM-01"));
                case SplitType.Season:
                    if (time.Month <= 3)
                    {
                        return Convert.ToDateTime(time.ToString("yyyy-01-01"));
                    }
                    else if (time.Month <= 6)
                    {
                        return Convert.ToDateTime(time.ToString("yyyy-04-01"));
                    }
                    else if (time.Month <= 9)
                    {
                        return Convert.ToDateTime(time.ToString("yyyy-07-01"));
                    }
                    else
                    {
                        return Convert.ToDateTime(time.ToString("yyyy-10-01"));
                    }
                case SplitType.Year:
                    return Convert.ToDateTime(time.ToString("yyyy-01-01"));
                default:
                    throw new Exception($"SplitType paramter error ");
            }
        }
        private DateTime GetMondayDate()
        {
            return GetMondayDate(DateTime.Now);
        }
        private DateTime GetSundayDate()
        {
            return GetSundayDate(DateTime.Now);
        }
        private DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        private DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }

        #endregion

        #region Private Models
        internal class SplitTableSort
        {
            public string Name { get; set; }
            public int Sort { get; set; }
        }
        #endregion

    }
}
