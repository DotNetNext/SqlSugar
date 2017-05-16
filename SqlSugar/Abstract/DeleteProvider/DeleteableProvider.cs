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
        public IDb Db { get { return Context.Database; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public int ExecuteCommand()
        {
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters.ToArray();
            return Db.GetInt(sql, paramters);
        }

        public IDeleteable<T> Where(List<T> deleteObjs)
        {
            if (deleteObjs == null || deleteObjs.Count() == 0)
            {
                Where("1=2 ");
                return this;
            }
            string tableName = this.Context.GetTableName<T>();
            var entityInfo = this.Context.EntityProvider.GetEntityInfo<T>();
            if (this.Context.IsSystemTablesConfig)
            {
                var primaryFields = this.Db.DbMaintenance.GetPrimaries(tableName).ToArray();
                var isSinglePrimaryKey = primaryFields.Length == 1;
                Check.ArgumentNullException(primaryFields, string.Format("Table {0} with no primarykey", tableName));
                if (isSinglePrimaryKey)
                {
                    List<object> primaryKeyValues = new List<object>();
                    var primaryField = primaryFields.Single();
                    foreach (var deleteObj in deleteObjs)
                    {
                        var entityPropertyName = this.Context.GetEntityPropertyName<T>(primaryField);
                        var columnInfo = entityInfo.Columns.Single(it => it.Name == entityPropertyName);
                        var value = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                        primaryKeyValues.Add(value);
                    }
                    var inValueString = primaryKeyValues.ToArray().ToJoinSqlInVals();
                    Where(string.Format(DeleteBuilder.WhereInTemplate, primaryFields.Single(), inValueString));
                }
                else
                {
                    StringBuilder whereInSql = new StringBuilder();
                    foreach (var deleteObj in deleteObjs)
                    {
                        StringBuilder orString = new StringBuilder();
                        var isFirst = deleteObjs.IndexOf(deleteObj)==0;
                        if (isFirst) {
                            orString.Append(DeleteBuilder.WhereInOrTemplate+PubConst.Space);
                        }
                        int i = 0;
                        StringBuilder andString = new StringBuilder();
                        foreach (var primaryField in primaryFields)
                        {
                            if (i == 0)
                                andString.Append(DeleteBuilder.WhereInAndTemplate + PubConst.Space);
                            var entityPropertyName = this.Context.GetEntityPropertyName<T>(primaryField);
                            var columnInfo = entityInfo.Columns.Single(it => it.Name == entityPropertyName);
                            var entityValue = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                            andString.AppendFormat(DeleteBuilder.WhereInEqualTemplate, primaryField, entityValue);
                            ++i;
                        }
                        orString.AppendFormat(DeleteBuilder.WhereInAreaTemplate,andString);
                        whereInSql.Append(orString);
                    }
                    Where(string.Format(DeleteBuilder.WhereInAreaTemplate,whereInSql.ToString()));
                }
            }
            else
            {

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
                DeleteBuilder.Parameters.AddRange(Context.Database.GetParameters(whereObj));
            }
            return this;
        }
        public IDeleteable<T> In<PkType>(List<PkType> primaryKeyValues) {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where("1=2 ");
                return this;
            }
            return In<PkType>(primaryKeyValues.ToArray());
        }
        public IDeleteable<T> In<PkType>(PkType[] primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where("1=2 ");
                return this;
            }
            string tableName = this.Context.GetTableName<T>();
            string primaryField = null;
            if (this.Context.IsSystemTablesConfig)
            {
                primaryField = this.Db.DbMaintenance.GetPrimaries(tableName).FirstOrDefault();
                Check.ArgumentNullException(primaryField, "Table " + tableName + " with no primarykey");
                Where(string.Format(DeleteBuilder.WhereInTemplate, primaryField, primaryKeyValues.ToJoinSqlInVals()));
            }
            else
            {

            }
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
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters.ToList();
            return new KeyValuePair<string, List<SugarParameter>>(sql, paramters);
        }
    }
}
