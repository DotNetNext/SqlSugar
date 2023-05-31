using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.HG
{
    public class HGInsertable<T> : InsertableProvider<T> where T : class, new()
    {
        public override int ExecuteReturnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string identityColumn = GetIdentityColumn();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(identityColumn));
            RestoreMapping();
            var result = Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()).ObjToInt();
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
            var obj = await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            var result = obj.ObjToInt();
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
            return new KeyValuePair<string, List<SugarParameter>>(result.Key.Replace("$PrimaryKey", primaryKey), result.Value);
        }

        public override long ExecuteReturnBigIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(GetIdentityKeys().FirstOrDefault()));
            RestoreMapping();
            var result = Convert.ToInt64(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()) ?? "0");
            After(sql, result);
            return result;
        }
        public override async Task<long> ExecuteReturnBigIdentityAsync()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString().Replace("$PrimaryKey", this.SqlBuilder.GetTranslationColumnName(GetIdentityKeys().FirstOrDefault()));
            RestoreMapping();
            var result = Convert.ToInt64(await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()) ?? "0");
            After(sql, result);
            return result;
        }

        public override bool ExecuteCommandIdentityIntoEntity()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0) { return this.ExecuteCommand() > 0; }
            var idValue = ExecuteReturnBigIdentity();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue = 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else
                setValue = Convert.ToInt32(idValue);
            var propertyName = this.Context.EntityMaintenance.GetPropertyName<T>(identityKey);
            typeof(T).GetProperties().First(t => t.Name.ToUpper() == propertyName.ToUpper()).SetValue(result, setValue, null);
            return idValue > 0;
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

    }
}
