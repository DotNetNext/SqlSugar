using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SqlSugar.OceanBaseForOracle
{
    public class OceanBaseForOracleDeleteable<T> : DeleteableProvider<T> where T : class, new()
    {
        protected override List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.DbColumnName).ToList();
        }
    }
}
