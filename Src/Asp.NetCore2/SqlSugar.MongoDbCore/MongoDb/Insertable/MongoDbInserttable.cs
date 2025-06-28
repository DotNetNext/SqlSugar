using MongoDb.Ado.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.MongoDb
{
    public class MongoDbInsertable<T> : InsertableProvider<T> where T : class, new()
    {
        public override List<Type> ExecuteReturnPkList<Type>()
        {
            base.ExecuteCommand();
            return ((MongoDbConnection)this.Ado.Connection).ObjectIds.Select(it=>(Type)(object)it).ToList();
        }
        public new async Task<List<Type>> ExecuteReturnPkListAsync<Type>()
        {
            await base.ExecuteCommandAsync();
            return ((MongoDbConnection)this.Ado.Connection).ObjectIds.Select(it => (Type)(object)it).ToList();
        }
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
        public override async Task<bool> ExecuteCommandIdentityIntoEntityAsync() 
        {
            await base.ExecuteCommandAsync();
            var ids = ((MongoDbConnection)this.Ado.Connection).ObjectIds;
            var insertObjects = this.InsertObjs;
            if (ids != null && insertObjects != null && ids.Count() == insertObjects.Length)
            {
                var idProp = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
                if (idProp != null)
                {
                    for (int i = 0; i < insertObjects.Length; i++)
                    {
                        idProp.SetValue(insertObjects[i], ids[i]);
                    }
                }
            }
            return true;
        }
        public override bool ExecuteCommandIdentityIntoEntity()
        {
            base.ExecuteCommand();
            var ids = ((MongoDbConnection)this.Ado.Connection).ObjectIds;
            var insertObjects = this.InsertObjs;
            if (ids != null && insertObjects != null && ids.Count() == insertObjects.Length)
            {
                var idProp = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
                if (idProp != null)
                {
                    for (int i = 0; i < insertObjects.Length; i++)
                    {
                        idProp.SetValue(insertObjects[i], ids[i]);
                    }
                }
            }
            return true;
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
