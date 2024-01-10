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
            var memberName = string.Empty;
            if (rowSelector.Body is MemberExpression)
            {
                memberName = ((MemberExpression)rowSelector.Body).Member.Name;
                rowName.Add(memberName);
            }
            else
                rowName.AddRange(((NewExpression)rowSelector.Body).Arguments.Select(it => it as MemberExpression).Select(it => it.Member.Name));


            table.Columns.AddRange(rowName.Select(x => new DataColumn(x)).ToArray());
            var columns = source.Select(columnSelector).Distinct();
            table.Columns.AddRange(columns.Select(x => new DataColumn(x?.ToString())).ToArray());

            Action<DataRow, IGrouping<TRow, T>> action;
            if (string.IsNullOrEmpty(memberName))
            {
                action = (row, rowGroup) =>
                {
                    var properties = rowGroup.Key.GetType().GetProperties();
                    foreach (var item in properties)
                        row[item.Name] = item.GetValue(rowGroup.Key, null);
                };
            }
            else
            {
                action = (row, rowGroup) => row[memberName] = rowGroup.Key;
            }

            var rows = source.GroupBy(rowSelector.Compile())
             .Select(rowGroup =>
             {
                 var row = table.NewRow();
                 action(row, rowGroup);
                 columns.GroupJoin(rowGroup, c => c, r => columnSelector(r),
                                          (c, columnGroup) =>
                                          {

                                              var dic = new Dictionary<string, object>();
                                              if (c != null)
                                                  dic[c.ToString()] = dataSelector(columnGroup);
                                              return dic;
                                          })
                       .SelectMany(x => x)
                       .Select(x => row[x.Key] = x.Value)
                       .ToArray();
                 table.Rows.Add(row);
                 return row;
             })
             .ToList();

            return table;
        }

        public static IEnumerable<dynamic> ToPivotList<T, TColumn, TRow, TData>(
            this IEnumerable<T> source,
            Func<T, TColumn> columnSelector,
            Expression<Func<T, TRow>> rowSelector,
            Func<IEnumerable<T>, TData> dataSelector)
        {

            var memberName = string.Empty;

            if (rowSelector.Body is MemberExpression)
                memberName = ((MemberExpression)rowSelector.Body).Member.Name;

            var columns = source.Select(columnSelector).Distinct();

            Action<IDictionary<string, object>, IGrouping<TRow, T>> action;
            if (string.IsNullOrEmpty(memberName))
            {
                action = (row, rowGroup) =>
                {
                    var properties = rowGroup.Key.GetType().GetProperties();
                    foreach (var item in properties)
                        row[item.Name] = item.GetValue(rowGroup.Key, null);
                };
            }
            else
            {
                action = (row, rowGroup) => row[memberName] = rowGroup.Key;
            }

            var rows = source.GroupBy(rowSelector.Compile())
            .Select(rowGroup =>
            {
                IDictionary<string, object> row = new ExpandoObject();
                action(row, rowGroup);
                columns.GroupJoin(rowGroup, c => c, r => columnSelector(r),
                                        (c, columnGroup) =>
                                        {
                                            var dic = new Dictionary<string, object>();
                                            if (c != null)
                                                dic[c.ToString()] = dataSelector(columnGroup);
                                            return dic;
                                        })
                     .SelectMany(x => x)
                     .Select(x => row[x.Key] = x.Value)
                     .ToList();
                return row;
            });
            return rows;

        }
    }
}