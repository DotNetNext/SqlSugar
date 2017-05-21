using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }

        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.InsertObjs.Length == 1; } }

        public EntityInfo EntityInfo { get; set; }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private bool IsOffIdentity { get; set; }
        public T[] InsertObjs { get; set; }

        #region Core
        public int ExecuteCommand()
        {
            InsertBuilder.IsReturnIdentity = false;
            PreToSql();
            return Ado.ExecuteCommand(InsertBuilder.ToSqlString(), InsertBuilder.Parameters.ToArray());
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }
        public int ExecuteReutrnIdentity()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            return Ado.GetInt(InsertBuilder.ToSqlString(), InsertBuilder.Parameters.ToArray());
        }
        #endregion

        #region Setting
        public IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName)).ToList();
            return this;
        }
        public IInsertable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumMethod(it.PropertyName)).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => ignoreColumns.Contains(it.PropertyName)).ToList();
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            this.InsertBuilder.TableWithString = lockString;
            return this;
        }

        public IInsertable<T> Where(bool isInsertNull, bool isOffIdentity = false)
        {
            this.IsOffIdentity = IsOffIdentity;
            if (this.InsertBuilder.LambdaExpressions == null)
                this.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.InsertBuilder.IsInsertNull = isInsertNull;
            return this;
        }
        #endregion

        #region Private Methods
        private void PreToSql()
        {
            #region Identities
            if (!IsOffIdentity)
            {
                List<string> identities = GetIdentityKeys();
                if (identities != null && identities.Any())
                {
                    this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                    {
                        return !identities.Any(i => it.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    }).ToList();
                }
            }
            #endregion

            #region IgnoreColumns
            if (this.Context.IgnoreColumns != null && this.Context.IgnoreColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.EntityName).ToList();
                this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.PropertyName.Equals(i.EntityPropertyName, StringComparison.CurrentCulture));
                }).ToList();
            }
            #endregion
            if (this.IsSingle)
            {
                foreach (var item in this.InsertBuilder.DbColumnInfoList)
                {
                    if (this.InsertBuilder.Parameters == null) this.InsertBuilder.Parameters = new List<SugarParameter>();
                    var paramters = new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.DbColumnName, item.Value);
                    if (InsertBuilder.IsInsertNull && paramters.Value == null) {
                        continue;
                    }
                    this.InsertBuilder.Parameters.Add(paramters);
                }
            }
        }
        internal void Init()
        {
            Check.Exception(InsertObjs == null || InsertObjs.Count() == 0, "InsertObjs is null");
            int i = 0;
            foreach (var item in InsertObjs)
            {
                List<DbColumnInfo> insertItem = new List<DbColumnInfo>();
                foreach (var column in EntityInfo.Columns)
                {
                    var columnInfo = new DbColumnInfo()
                    {
                        Value = column.PropertyInfo.GetValue(item),
                        DbColumnName = GetDbColumnName(column.PropertyName),
                        PropertyName = column.PropertyName,
                        TableId = i
                    };
                    insertItem.Add(columnInfo);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            }
        }
        private string GetDbColumnName(string entityName)
        {
            if (!IsMappingColumns)
            {
                return entityName;
            }
            if (this.Context.MappingColumns.Any(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)))
            {
                this.MappingColumnList = this.Context.MappingColumns.Where(it => it.EntityName.Equals(EntityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (MappingColumnList == null || !MappingColumnList.Any())
            {
                return entityName;
            }
            else
            {
                var mappInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityPropertyName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                return mappInfo == null ? entityName : mappInfo.DbColumnName;
            }
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
        #endregion
    }
}
