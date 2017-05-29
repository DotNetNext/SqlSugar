using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteableProvider<T> : IDeleteable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityProvider.GetEntityInfo<T>();
            }
        }
        public int ExecuteCommand()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityProvider.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters==null?null:DeleteBuilder.Parameters.ToArray();
            RestoreMapping();
            return Db.GetInt(sql, paramters);
        }
        public IDeleteable<T> AS(string tableName)
        {
            var entityName = typeof(T).Name;
            IsAs = true;
            OldMappingTableList = this.Context.MappingTables;
            this.Context.MappingTables = this.Context.RewritableMethods.TranslateCopy(this.Context.MappingTables);
            this.Context.MappingTables.Add(entityName, tableName);
            return this; ;
        }

        public IDeleteable<T> Where(List<T> deleteObjs)
        {
            if (deleteObjs == null || deleteObjs.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            string tableName = this.Context.EntityProvider.GetTableName<T>();
            var primaryFields = this.GetPrimaryKeys();
            var isSinglePrimaryKey = primaryFields.Count == 1;
            Check.ArgumentNullException(primaryFields, string.Format("Table {0} with no primarykey", tableName));
            if (isSinglePrimaryKey)
            {
                List<object> primaryKeyValues = new List<object>();
                var primaryField = primaryFields.Single();
                foreach (var deleteObj in deleteObjs)
                {
                    var entityPropertyName = this.Context.EntityProvider.GetEntityPropertyName<T>(primaryField);
                    var columnInfo = EntityInfo.Columns.Single(it => it.PropertyName == entityPropertyName);
                    var value = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                    primaryKeyValues.Add(value);
                }
                var inValueString = primaryKeyValues.ToArray().ToJoinSqlInVals();
                Where(string.Format(DeleteBuilder.WhereInTemplate,SqlBuilder.GetTranslationColumnName(primaryFields.Single()), inValueString));
            }
            else
            {
                StringBuilder whereInSql = new StringBuilder();
                foreach (var deleteObj in deleteObjs)
                {
                    StringBuilder orString = new StringBuilder();
                    var isFirst = deleteObjs.IndexOf(deleteObj) == 0;
                    if (isFirst)
                    {
                        orString.Append(DeleteBuilder.WhereInOrTemplate + PubConst.Space);
                    }
                    int i = 0;
                    StringBuilder andString = new StringBuilder();
                    foreach (var primaryField in primaryFields)
                    {
                        if (i == 0)
                            andString.Append(DeleteBuilder.WhereInAndTemplate + PubConst.Space);
                        var entityPropertyName = this.Context.EntityProvider.GetEntityPropertyName<T>(primaryField);
                        var columnInfo = EntityInfo.Columns.Single(it => it.PropertyName == entityPropertyName);
                        var entityValue = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                        andString.AppendFormat(DeleteBuilder.WhereInEqualTemplate, primaryField, entityValue);
                        ++i;
                    }
                    orString.AppendFormat(DeleteBuilder.WhereInAreaTemplate, andString);
                    whereInSql.Append(orString);
                }
                Where(string.Format(DeleteBuilder.WhereInAreaTemplate, whereInSql.ToString()));
            }
            return this;
        }

        public IDeleteable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult = DeleteBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            DeleteBuilder.WhereInfos.Add(expResult.GetResultString());
            return this;
        }

        public IDeleteable<T> Where(T deleteObj)
        {
            Where(new List<T>() { deleteObj });
            return this;
        }

        public IDeleteable<T> Where(string whereString, object whereObj = null)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (whereObj != null)
            {
                DeleteBuilder.Parameters.AddRange(Context.Ado.GetParameters(whereObj));
            }
            return this;
        }

        public IDeleteable<T> In<PkType>(List<PkType> primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In<PkType>(primaryKeyValues.ToArray());
        }

        public IDeleteable<T> In<PkType>(PkType[] primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            string tableName = this.Context.EntityProvider.GetTableName<T>();
            string primaryField = null;
            primaryField = GetPrimaryKeys().FirstOrDefault();
            Check.ArgumentNullException(primaryField, "Table " + tableName + " with no primarykey");
            Where(string.Format(DeleteBuilder.WhereInTemplate,SqlBuilder.GetTranslationColumnName(primaryField), primaryKeyValues.ToJoinSqlInVals()));
            return this;
        }

        public IDeleteable<T> In<PkType>(PkType primaryKeyValue)
        {
            In(new PkType[] { primaryKeyValue });
            return this;
        }

        public IDeleteable<T> With(string lockString)
        {
            DeleteBuilder.TableWithString = lockString;
            return this;
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityProvider.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToList();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, paramters);
        }


        private List<string> GetPrimaryKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.EntityInfo.DbTableName);
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.EntityInfo.DbTableName);
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            }
        }
        private void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }
    }
}
