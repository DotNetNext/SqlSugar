using SqlSugar;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.TDSQLForOracleODBC
{
    public class TDSQLForOracleODBCDeleteable<T> : DeleteableProvider<T> where T : class, new()
    {
        protected override List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.DbColumnName).ToList();
        }
    }
}
