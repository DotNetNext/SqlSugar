using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar.IoTDB
{
    public class IoTDBQueryable<T> : QueryableProvider<T>
    {
        public override ISugarQueryable<T> With(string withString)
        {
            throw new System.NotSupportedException("IoTDB tree model CTE queries are not supported by this provider.");
        }

        public override DataTable ToDataTable()
        {
            InitMapping();
            var sql = ToSql();
            RestoreMapping();
            if (!Regex.IsMatch(sql.Key, @"\bWHERE\s*\(?\s*1\s*=\s*2", RegexOptions.IgnoreCase))
                return base.ToDataTable();

            var table = new DataTable();
            var entity = Context.EntityMaintenance.GetEntityInfo(typeof(T));
            foreach (var column in entity.Columns.Where(item => !item.IsIgnore))
            {
                var type = column.UnderType ?? column.PropertyInfo?.PropertyType ?? typeof(object);
                type = Nullable.GetUnderlyingType(type) ?? type;
                table.Columns.Add(column.DbColumnName, type);
            }
            return table;
        }
    }

    public class IoTDBQueryable<T, T2> : QueryableProvider<T, T2> { }
    public class IoTDBQueryable<T, T2, T3> : QueryableProvider<T, T2, T3> { }
    public class IoTDBQueryable<T, T2, T3, T4> : QueryableProvider<T, T2, T3, T4> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4, T5> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5, T6> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T, T2, T3, T4, T5, T6, T7> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> { }
    public class IoTDBQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> { }
}
