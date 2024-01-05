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
            table.Columns.AddRange(columns.Select(x => new DataColumn(x?.ToString())).ToArray());

            var rows = source.GroupBy(rowSelector.Compile())
                  .Select(rowGroup =>
                  {
                      var anonymousType = rowGroup.Key.GetType();
                      var properties = anonymousType.GetProperties();
                      var row = table.NewRow();
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
                            .SelectMany(x => properties, (x, y) => row[y.Name] = y.GetValue(rowGroup.Key, null))
                            .ToArray();
                      table.Rows.Add(row);
                      return row;
                  }
                  ).ToList();
            return table;
        }

        public static IEnumerable<dynamic> ToPivotList<T, TColumn, TRow, TData>(
            this IEnumerable<T> source,
            Func<T, TColumn> columnSelector,
            Expression<Func<T, TRow>> rowSelector,
            Func<IEnumerable<T>, TData> dataSelector)
        {

            var rowName = new List<string>();
            if (rowSelector.Body is MemberExpression)
                rowName.Add(((MemberExpression)rowSelector.Body).Member.Name);
            else
                rowName.AddRange(((NewExpression)rowSelector.Body).Arguments.Select(it => it as MemberExpression).Select(it => it.Member.Name));

            var columns = source.Select(columnSelector).Distinct();

            var rows = source.GroupBy(rowSelector.Compile())
                .Select(rowGroup =>
                {
                    var anonymousType = rowGroup.Key.GetType();
                    var properties = anonymousType.GetProperties();
                    IDictionary<string, object> row = new ExpandoObject();
                    columns.GroupJoin(rowGroup, c => c, r => columnSelector(r),
                                            (c, columnGroup) =>
                                            {
                                                IDictionary<string, object> dic = new ExpandoObject();
                                                if (c != null)
                                                    dic[c.ToString()] = dataSelector(columnGroup);
                                                return dic;
                                            })
                         .SelectMany(x => x)
                         .Select(x => row[x.Key] = x.Value)
                         .SelectMany(x => properties, (x, y) => row[y.Name] = y.GetValue(rowGroup.Key, null))
                         .ToList();
                    return row;
                });
            return rows;
        }
    }
}
