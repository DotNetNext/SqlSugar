using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar 
{
    public class OracleInsertable<T> : InsertableProvider<T> where T : class, new()
    {
        protected override List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.IsValuable()).Select(it => it.DbColumnName).ToList();
        }
        protected override void PreToSql() {

        }
    }
}
