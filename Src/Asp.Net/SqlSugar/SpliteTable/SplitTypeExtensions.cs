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
    }
}
