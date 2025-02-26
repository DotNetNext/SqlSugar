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

        public int ExecuteCommand()
        {
            var provider = (InsertableProvider<T>) thisValue;
            var inserObjects = provider.InsertObjs;
            var attr = typeof(T).GetCustomAttribute<STableAttribute>();
            Check.ExceptionEasy(attr == null|| attr?.Tag1==null, $"", $"{nameof(T)}缺少特性STableAttribute和Tag1");
            // 根据所有非空的 Tag 进行分组
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
            return inserObjects.Count();
        }
    }
}
