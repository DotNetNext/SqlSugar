using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public static class SplitTableInfoExtensions
    {
        public static IEnumerable<SplitTableInfo> InTableNames(this List<SplitTableInfo> tables, params string[] tableNames)
        {
            return tables.Where(it => tableNames.Any(y => y.Equals(it.TableName, StringComparison.OrdinalIgnoreCase)));
        }
        public static IEnumerable<SplitTableInfo> ContainsTableNames(this List<SplitTableInfo> tables, params string[] tableNames)
        {
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in tables)
            {
                if (tableNames.Any(it => item.TableName.Contains(it))) 
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
