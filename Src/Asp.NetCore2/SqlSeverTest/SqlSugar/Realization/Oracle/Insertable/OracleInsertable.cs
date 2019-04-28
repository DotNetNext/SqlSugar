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
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.DbColumnName).ToList();
        }
        protected string GetSeqName()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.OracleSequenceName).First();
        }
        protected List<string> GetSeqNames()
        {
            return this.EntityInfo.Columns.Where(it => it.OracleSequenceName.HasValue()).Select(it => it.OracleSequenceName).ToList();
        }
        public override int ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 : GetSeqValue(GetSeqName()).ObjToInt();
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            return result;
        }

        public override long ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 :Convert.ToInt64(GetSeqValue(GetSeqName()));
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            return result;
        }


        public override int ExecuteCommand()
        {
            base.ExecuteCommand();
            return base.InsertObjs.Count();
        }

        private object GetSeqValue(string seqName)
        {
            return Ado.GetScalar(" SELECT " + seqName + ".currval FROM DUAL");
        }
        protected override void PreToSql()
        {
            var identities = GetSeqNames();
            var insertCount = InsertObjs.Count();
            InsertBuilder.OracleSeqInfoList = new Dictionary<string, int>();
            if (identities.HasValue() && insertCount > 1)
            {
                Check.Exception(identities.Count != identities.Distinct().Count(), "The field sequence needs to be unique");
                foreach (var seqName in identities)
                {
                    int seqBeginValue = 0;
                    this.Ado.ExecuteCommand("alter sequence " + seqName + " increment by " + insertCount);
                    seqBeginValue = this.Ado.GetInt("select  " + seqName + ".Nextval  from dual") - insertCount;
                    this.Ado.ExecuteCommand("alter sequence " + seqName + " increment by " + 1);
                    InsertBuilder.OracleSeqInfoList.Add(seqName, seqBeginValue);
                }
            }
            base.PreToSql();
        }
    }
}
