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
        public IDb Db { get { return Context.Database; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }

        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.InsertObjs.Length == 1; } }

        public EntityInfo EntityInfo { get; set; }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        public T[] InsertObjs { get; set; }

        public int ExecuteCommand()
        {
            PreToSql();
            return Db.ExecuteCommand(InsertBuilder.ToSqlString(), InsertBuilder.Parameters);
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            PreToSql();
            string sql = InsertBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }

        public int ExecuteReutrnIdentity()
        {
            PreToSql();
            return Db.GetInt(InsertBuilder.ToSqlString(), InsertBuilder.Parameters);
        }

        public IInsertable<T> IgnoreColumns(Expression<Func<T, object[]>> columns)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object[]>> columns)
        {
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            this.InsertBuilder.TableWithString = lockString;
            return this;
        }

        public IInsertable<T> Where(bool isInsertNull)
        {
            if (this.InsertBuilder.LambdaExpressions == null)
                this.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.InsertBuilder.IsInsertNull = isInsertNull;
            return this;
        }

        #region Private Methods
        private void PreToSql()
        {
            #region Identities
            if (this.Context.IsSystemTablesConfig)
            {
                List<string> identities = Db.DbMaintenance.GetIsIdentities(this.InsertBuilder.TableName);
                if (identities != null && identities.Any())
                {
                    var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.Name).ToList();
                    this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                    {
                        return !identities.Any(i => it.ColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    }).ToList();
                }
            }
            else {

            }
            #endregion

            #region IgnoreColumns
            if (this.Context.IgnoreColumns != null && this.Context.IgnoreColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.Name).ToList();
                this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.EntityPropertyName.Equals(i.EntityPropertyName,StringComparison.CurrentCulture));
                }).ToList();
            } 
            #endregion
            if (this.IsSingle)
            {
                foreach (var item in this.InsertBuilder.DbColumnInfoList)
                {
                    if (this.InsertBuilder.Parameters == null) this.InsertBuilder.Parameters = new List<SugarParameter>();
                    this.InsertBuilder.Parameters.Add(new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.ColumnName, item.Value));
                }
            }
        }
        public void Init()
        {
            this.InsertBuilder.TableName = EntityInfo.Name;
            if (IsMappingTable)
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == EntityInfo.Name);
                if (mappingInfo != null)
                {
                    this.InsertBuilder.TableName = mappingInfo.DbTableName;
                }
            }
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
                        ColumnName = GetDbColumnName(column.Name),
                        EntityPropertyName = column.Name,
                        TableId = i
                    };
                    insertItem.Add(columnInfo);
                }
                this.InsertBuilder.DbColumnInfoList.AddRange(insertItem);
            }
        }
        private string GetDbColumnName(string entityName)
        {
            if (!IsMappingColumns)
            {
                return entityName;
            }
            if (this.Context.MappingColumns.Any(it => it.EntityName.Equals(EntityInfo.Name,StringComparison.CurrentCultureIgnoreCase)))
            {
                this.MappingColumnList = this.Context.MappingColumns.Where(it => it.EntityName.Equals(EntityInfo.Name,StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            if (MappingColumnList == null || !MappingColumnList.Any())
            {
                return entityName;
            }
            else
            {
                var mappInfo = this.Context.MappingColumns.FirstOrDefault(it => it.EntityPropertyName.Equals(entityName,StringComparison.CurrentCultureIgnoreCase));
                return mappInfo == null ? entityName : mappInfo.DbColumnName;
            }
        }
        #endregion
    }
}
