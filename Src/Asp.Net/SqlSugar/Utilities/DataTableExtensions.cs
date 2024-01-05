using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    internal static class DataTableExtensions
    {
        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
    this IEnumerable<T> source,
    Func<T, TColumn> columnSelector,
    Expression<Func<T, TRow>> rowSelector,
    Func<IEnumerable<T>, TData> dataSelector)
        {
            DataTable table = new DataTable();
            var rowName = new List<string>();
            if (rowSelector.Body is MemberExpression)
                rowName.Add(((MemberExpression)rowSelector.Body).Member.Name);
            else
                rowName.AddRange(((NewExpression)rowSelector.Body).Arguments.Select(it => it as MemberExpression).Select(it => it.Member.Name));

            table.Columns.AddRange(rowName.Select(x => new DataColumn(x)).ToArray());
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column?.ToString()));

            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                // 获取匿名对象的动态类型  
                var anonymousType = row.Key.GetType();

                // 获取匿名对象的所有属性  
                var properties = anonymousType.GetProperties();

                for (var i = 0; i < rowName.Count; i++)
                {
                    items.Insert(i, properties[i].GetValue(row.Key, null));
                }
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }

            return table;
        }

        public static List<dynamic> ToPivotList<T, TColumn, TRow, TData>(
                                                                        this IEnumerable<T> source,
                                                                        Func<T, TColumn> columnSelector,
                                                                        Expression<Func<T, TRow>> rowSelector,
                                                                        Func<IEnumerable<T>, TData> dataSelector)
        {

            var arr = new List<dynamic>();
            var cols = new List<string>();
            var rowName = new List<string>();
            if (rowSelector.Body is MemberExpression)
                rowName.Add(((MemberExpression)rowSelector.Body).Member.Name);
            else
                rowName.AddRange(((NewExpression)rowSelector.Body).Arguments.Select(it => it as MemberExpression).Select(it => it.Member.Name));

            var columns = source.Select(columnSelector).Distinct();

            cols = rowName.Concat(columns.Select(x => x?.ToString())).ToList();

            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });

            foreach (var row in rows)
            {
                var items = row.Values.Cast<object>().ToList();

                // 获取匿名对象的动态类型  
                var anonymousType = row.Key.GetType();

                // 获取匿名对象的所有属性  
                var properties = anonymousType.GetProperties();

                for (var i = 0; i < rowName.Count; i++)
                {
                    items.Insert(i, properties[i].GetValue(row.Key, null));
                }
                var obj = GetAnonymousObject(cols, items);
                arr.Add(obj);
            }
            return arr;
        }
        private static dynamic GetAnonymousObject(IEnumerable<string> columns, IEnumerable<object> values)
        {
            IDictionary<string, object> eo = new ExpandoObject() as IDictionary<string, object>;
            int i;
            for (i = 0; i < columns.Count(); i++)
            {
                eo.Add(columns.ElementAt<string>(i), values.ElementAt<object>(i));
            }
            return eo;
        }
    }
}
