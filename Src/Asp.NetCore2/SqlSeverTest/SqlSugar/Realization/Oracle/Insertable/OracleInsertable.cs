using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            bool oldIsAuto = AutoBegin();

            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 : GetSeqValue(GetSeqName()).ObjToInt();
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;

            AutoEnd(oldIsAuto);
            return result;
        }

        public override long ExecuteReturnBigIdentity()
        {
            bool oldIsAuto = AutoBegin();

            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 : Convert.ToInt64(GetSeqValue(GetSeqName()));
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;

            AutoEnd(oldIsAuto);
            return result;
        }

        public async override Task<int> ExecuteReturnIdentityAsync()
        {
            bool oldIsAuto = AutoBegin();

            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = await Ado.ExecuteCommandAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 : GetSeqValue(GetSeqName()).ObjToInt();
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;

            AutoEnd(oldIsAuto);
            return result;
        }

        public async override Task<long> ExecuteReturnBigIdentityAsync()
        {
            bool oldIsAuto = AutoBegin();

            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
            this.Context.Ado.IsDisableMasterSlaveSeparation = true;
            var count = await Ado.ExecuteCommandAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = (this.GetIdentityKeys().IsNullOrEmpty() || count == 0) ? 0 : Convert.ToInt64(GetSeqValue(GetSeqName()));
            this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;

            AutoEnd(oldIsAuto);
            return result;
        }

        private void AutoEnd(bool oldIsAuto)
        {
            if (oldIsAuto)
            {
                this.Context.Context.CurrentConnectionConfig.IsAutoCloseConnection = oldIsAuto;
                if (this.Ado.Transaction == null)
                    this.Context.Ado.Close();
            }
        }

        private bool AutoBegin()
        {
            var oldIsAuto = this.Context.Context.CurrentConnectionConfig.IsAutoCloseConnection;
            if (this.Context.Context.CurrentConnectionConfig.IsAutoCloseConnection)
            {
                this.Context.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
            }

            return oldIsAuto;
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
                    seqBeginValue = this.Ado.GetInt("select  " + seqName + ".Nextval  from dual");
                    //Console.WriteLine(seqBeginValue);
                    var nextLength = insertCount - 1;
                    if (nextLength > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(" select " + seqName + ".nextval,t.* from (");
                        for (int i = 0; i < nextLength; i++)
                        {
                            sb.AppendLine(" select 1 from dual");
                            if (i < (nextLength - 1))
                            {
                                sb.AppendLine("union all");
                            }
                        }
                        sb.AppendLine(" )t");
                        this.Ado.SqlQuery<int>(sb.ToString());
                    }
                    InsertBuilder.OracleSeqInfoList.Add(seqName, seqBeginValue);
                }
            }
            base.PreToSql();
        }
    }
}
