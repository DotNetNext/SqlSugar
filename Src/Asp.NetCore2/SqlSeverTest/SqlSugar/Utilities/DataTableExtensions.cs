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
            var rowName = "";
            if (rowSelector.Body is MemberExpression)
                rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            else
                rowName =string.Join(UtilConstants.ReplaceKey, ((NewExpression)rowSelector.Body).Arguments.Select(it=>it as MemberExpression).Select(it=>it.Member.Name));
            table.Columns.Add(new DataColumn(rowName));
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column.ToString()));

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
                items.Insert(0, row.Key);
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }
            var firstName = table.Columns[0]?.ColumnName;
            if (firstName.ObjToString().Contains(UtilConstants.ReplaceKey)) 
            {
                int i = 0;
                foreach (var item in Regex.Split(firstName,UtilConstants.ReplaceKey))
                {
                    i++;
                    table.Columns.Add(item);
                    table.Columns[item].SetOrdinal(i);
                }
                foreach (DataRow row in table.Rows)
                {
                    var json =row[firstName];
                    var list = json.ToString().TrimStart('{').TrimEnd('}').Split(',').Select(it=>it.Split('=')).ToList();
                    foreach (var item in Regex.Split(firstName, UtilConstants.ReplaceKey))
                    {
                        var x = list.First(it => it.First().Trim() == item.Trim());
                        row[item] =x[1] ;
                    }
                }
                table.Columns.Remove(firstName);
            }
            return table;
        }
        public static List<dynamic> ToPivotList<T, TColumn, TRow, TData>(
                                                                        this IEnumerable<T> source,
                                                                        Func<T, TColumn> columnSelector,
                                                                        Expression<Func<T, TRow>> rowSelector,
                                                                        Func<IEnumerable<T>, TData> dataSelector)
        {

            var arr = new List<object>();
            var cols = new List<string>();
            String rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            var columns = source.Select(columnSelector).Distinct();

            cols = (new[] { rowName }).Concat(columns.Select(x => x.ToString())).ToList();


            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             }).ToList();


            foreach (var row in rows)
            {
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                var obj = GetAnonymousObject(cols, items);
                arr.Add(obj);
            }
            return arr.ToList();
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
