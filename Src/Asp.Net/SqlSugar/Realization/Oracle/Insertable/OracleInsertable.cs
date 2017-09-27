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
        protected override void PreToSql()
        {
            var identities = GetIdentityKeys();
            var insertCount = InsertObjs.Count();
            InsertBuilder.OracleSeqInfoList = new Dictionary<string, int>();
            if (identities.IsValuable()&& insertCount > 1)
            {
                Check.Exception(identities.Count != identities.Distinct().Count(), "The field sequence needs to be unique");
                foreach (var seqName in identities)
                {
                    int seqBeginValue = 0;
                    this.Ado.ExecuteCommand("alter sequence " + seqName + " increment by " + insertCount);
                    seqBeginValue = this.Ado.GetInt("select  " + seqName + ".Nextval  from dual") - insertCount;
                    this.Ado.ExecuteCommand("alter sequence " + seqName + " increment by " + 1);
                    InsertBuilder.OracleSeqInfoList.Add(seqName,seqBeginValue);
                }
            }
            base.PreToSql();
        }
    }
}
