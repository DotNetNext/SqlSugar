using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.Access
{
    public class AccessUpdateBuilder : UpdateBuilder
    {
        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            if (groupList.Count == 0) 
            {
                return null;
            }
            else if (groupList.GroupBy(it => it.Key).Count() > 1)
            {
                throw new Exception("access no support batch update");
            }
            else 
            {
                return ToSingleSqlString(groupList);
            }
        }
         
    }
}
