using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlSugar 
{
    public class TagInserttable<T> where T : class, new()
    {
        internal IInsertable<T> thisValue;
        internal Func<string, T, string> getChildTableNamefunc;

        internal SqlSugarProvider Context;

        public int ExecuteCommand()
        {
            var provider = (InsertableProvider<T>)thisValue;
            var inserObjects = provider.InsertObjs;
            var attr = typeof(T).GetCustomAttribute<STableAttribute>();
            Check.ExceptionEasy(attr == null || attr?.Tag1 == null, $"", $"{nameof(T)}缺少特性STableAttribute和Tag1");
            // 根据所有非空的 Tag 进行分组
            var groups = GetGroupInfos(inserObjects, attr);
            foreach (var item in groups)
            {
                var childTableName = getChildTableNamefunc(attr.STableName, item.First());
                this.Context.Utilities.PageEach(item, 500, pageItems =>
                {
                    var sTableName = provider.SqlBuilder.GetTranslationColumnName(attr.STableName);
                    var tags = new List<string>();
                    List<string> tagValues = GetTagValues(pageItems, attr); 
                    var tagString = string.Join(",", tagValues.Where(v => !string.IsNullOrEmpty(v)).Select(v => $"'{v.ToSqlFilter()}'"));
                    tags.Add(tagString);
                    this.Context.Ado.ExecuteCommand($"CREATE TABLE IF NOT EXISTS {childTableName} USING {sTableName} TAGS ({tagString})");
                    this.Context.Insertable(pageItems).AS(childTableName).ExecuteCommand();
                });
            }
            return inserObjects.Count();
        }

        private static List<string> GetTagValues(List<T> pageItems, STableAttribute attr)
        {
            var tagValues = new List<string>();
            var obj = pageItems.First();
            if (attr.Tag1 != null)
                tagValues.Add(obj.GetType().GetProperty(attr.Tag1)?.GetValue(obj)?.ToString());

            if (attr.Tag2 != null)
                tagValues.Add(obj.GetType().GetProperty(attr.Tag2)?.GetValue(obj)?.ToString());

            if (attr.Tag3 != null)
                tagValues.Add(obj.GetType().GetProperty(attr.Tag3)?.GetValue(obj)?.ToString());

            if (attr.Tag4 != null)
                tagValues.Add(obj.GetType().GetProperty(attr.Tag4)?.GetValue(obj)?.ToString());
            return tagValues;
        }

        private static IEnumerable<IGrouping<string, T>> GetGroupInfos(T[] inserObjects, STableAttribute? attr)
        {
            var groups = inserObjects.GroupBy(it =>
            {
                // 动态生成分组键
                var groupKey = new List<string>();

                if (attr.Tag1 != null)
                    groupKey.Add(it.GetType().GetProperty(attr.Tag1)?.GetValue(it)?.ToString());

                if (attr.Tag2 != null)
                    groupKey.Add(it.GetType().GetProperty(attr.Tag2)?.GetValue(it)?.ToString());

                if (attr.Tag3 != null)
                    groupKey.Add(it.GetType().GetProperty(attr.Tag3)?.GetValue(it)?.ToString());

                if (attr.Tag4 != null)
                    groupKey.Add(it.GetType().GetProperty(attr.Tag4)?.GetValue(it)?.ToString());

                // 将非空的 Tag 值用下划线连接作为分组键
                return string.Join("_", groupKey.Where(k => !string.IsNullOrEmpty(k)));
            });
            return groups;
        }
    }
}
