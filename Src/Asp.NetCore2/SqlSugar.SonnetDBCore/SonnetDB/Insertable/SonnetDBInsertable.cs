using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBInsertable<T> : InsertableProvider<T>, IInsertable<T> where T : class, new()
    {
        public override int ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string identityColumn = GetIdentityColumn();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(identityColumn));
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
            var result = GetIdentityValue(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            After(sql, result);
            return result;
        }
        public override async Task<int> ExecuteReturnIdentityAsync()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string identityColumn = GetIdentityColumn();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(identityColumn));
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
            var obj = await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = GetIdentityValue(obj);
            After(sql, result);
            return result;
        }
        public override KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            var result= base.ToSql();
            var primaryKey = GetPrimaryKeys().FirstOrDefault();
            if (primaryKey != null)
            {
                primaryKey = this.SqlBuilder.GetTranslationColumnName(primaryKey);
            }
            else
            {
                return new KeyValuePair<string, List<SugarParameter>>(result.Key.Replace(" RETURNING $PrimaryKey", ""), result.Value);
            }
            return new KeyValuePair<string, List<SugarParameter>>(result.Key.Replace("$PrimaryKey", primaryKey), result.Value);
        }

        public override long ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(GetIdentityColumn()));
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
            var result = GetBigIdentityValue(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            After(sql, result);
            return result;
        }
        public override async Task<long> ExecuteReturnBigIdentityAsync()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(GetIdentityColumn()));
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
            var result = GetBigIdentityValue(await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            After(sql, result);
            return result;
        }

        private string GetIdentityColumn()
        {
            var identityColumn = GetIdentityKeys().FirstOrDefault();
            if (identityColumn == null)
            {
                var columns = this.Context.DbMaintenance.GetColumnInfosByTableName(InsertBuilder.GetTableNameString);
                identityColumn = columns.First(it => it.IsIdentity || it.IsPrimarykey).DbColumnName;
            }
            return identityColumn;
        }

        private static int GetIdentityValue(object value)
        {
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private static long GetBigIdentityValue(object value)
        {
            return value == null || value == DBNull.Value ? 0L : Convert.ToInt64(value);
        }

    }

}
