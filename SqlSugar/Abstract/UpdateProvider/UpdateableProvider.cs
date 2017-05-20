using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableProvider<T> : IUpdateable<T>
    {
        public SqlSugarClient Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; internal set; }
        public IAdo Ado { get { return Context.Ado; } }
        public T[] UpdateObjs { get;  set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.UpdateObjs.Length == 1; } }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private bool IsOffIdentity { get; set; }
        public int ExecuteCommand()
        {
            PreToSql();
            return this.Ado.ExecuteCommand(UpdateBuilder.ToSqlString(), UpdateBuilder.Parameters.ToArray());
        }

        public IUpdateable<T> IgnoreColumns(Func<string, bool> ignoreColumMethod)
        {
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumMethod(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> ReSetValue(Expression<Func<T, bool>> setValueExpression)
        {
            var expResult = UpdateBuilder.GetExpressionValue(setValueExpression, ResolveExpressType.WhereSingle);
            var resultString=expResult.GetResultString();
            LambdaExpression lambda = setValueExpression as LambdaExpression;
            var expression = lambda.Body;
            Check.Exception(!(expression is BinaryExpression), "Expression  format error");
            var leftExpression = (expression as BinaryExpression).Left;
            Check.Exception(!(leftExpression is MemberExpression), "Expression  format error");
            var leftResultString=UpdateBuilder.GetExpressionValue(leftExpression, ResolveExpressType.WhereSingle).GetString();
            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(leftResultString, resultString));
            return this;
        }

        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, UpdateBuilder.Parameters);
        }

        public IUpdateable<T> Update(T InsertObj)
        {
            return this;
        }

        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Array).GetResultArray();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => ignoreColumns.Contains(it.EntityPropertyName)).ToList();
            return this;
        }

        public IUpdateable<T> UpdateRange(List<T> InsertObjs)
        {
            return this;
        }

        public IUpdateable<T> Where(bool isUpdateNull)
        {
            return this;
        }
        public IUpdateable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult = UpdateBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            UpdateBuilder.WhereValues.Add(expResult.GetResultString());
            return this;
        }
        public IUpdateable<T> With(string lockString)
        {
            this.UpdateBuilder.TableWithString = lockString;
            return this;
        }

        internal void Init()
        {
            this.UpdateBuilder.TableName = EntityInfo.EntityName;
            if (IsMappingTable)
            {
                var mappingInfo = this.Context.MappingTables.SingleOrDefault(it => it.EntityName == EntityInfo.EntityName);
                if (mappingInfo != null)
                {
                    this.UpdateBuilder.TableName = mappingInfo.DbTableName;
                }
            }
            Check.Exception(UpdateObjs == null || UpdateObjs.Count() == 0, "UpdateObjs is null");
            int i = 0;
            foreach (var item in UpdateObjs)
            {
                List<DbColumnInfo> insertItem = new List<DbColumnInfo>();
                foreach (var column in EntityInfo.Columns)
                {
                    var columnInfo = new DbColumnInfo()
                    {
                        Value = column.PropertyInfo.GetValue(item),
                        ColumnName = GetDbColumnName(column.PropertyName),
                        EntityPropertyName = column.PropertyName,
                        TableId = i
                    };
                    insertItem.Add(columnInfo);
                }
                this.UpdateBuilder.DbColumnInfoList.AddRange(insertItem);
                ++i;
            }
        }
        private void PreToSql()
        {
            #region Identities
            if (!IsOffIdentity)
            {
                List<string> identities = GetIdentityKeys();
                if (identities != null && identities.Any())
                {
                    this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it =>
                    {
                        return !identities.Any(i => it.ColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
                    }).ToList();
                }
            }
            #endregion

            #region IgnoreColumns
            if (this.Context.IgnoreColumns != null && this.Context.IgnoreColumns.Any())
            {
                var currentIgnoreColumns = this.Context.IgnoreColumns.Where(it => it.EntityName == this.EntityInfo.EntityName).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it =>
                {
                    return !currentIgnoreColumns.Any(i => it.EntityPropertyName.Equals(i.EntityPropertyName, StringComparison.CurrentCulture));
                }).ToList();
            }
            #endregion
            if (this.IsSingle)
            {
                foreach (var item in this.UpdateBuilder.DbColumnInfoList)
                {
                    if (this.UpdateBuilder.Parameters == null) this.UpdateBuilder.Parameters = new List<SugarParameter>();
                    this.UpdateBuilder.Parameters.Add(new SugarParameter(this.SqlBuilder.SqlParameterKeyWord + item.ColumnName, item.Value));
                }
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
    }
}
