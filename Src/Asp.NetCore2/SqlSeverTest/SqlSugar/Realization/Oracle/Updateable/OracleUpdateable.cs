using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OracleUpdateable<T> : UpdateableProvider<T> where T : class, new()
    {
        protected override List<string> GetIdentityKeys()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.DbColumnName).ToList();
        }
        public override int ExecuteCommand()
        {
            if (base.UpdateObjs.Count() == 1)
            {
                return base.ExecuteCommand();
            }
            else
            {
                base.ExecuteCommand();
                return base.UpdateObjs.Count();
            }
        }
    }
}
