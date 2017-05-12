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
            throw new NotImplementedException();
        }

        public IDeleteable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult=DeleteBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            DeleteBuilder.WhereInfos.Add(expResult.GetResultString());
            return this;
        }

        public IDeleteable<T> Where(T deleteObj)
        {
            string tableName = this.Context.GetTableName<T>();

            return this;
        }

        public IDeleteable<T> Where(string whereString, object whereObj=null)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (whereObj != null)
            {
                DeleteBuilder.Parameters.AddRange(Context.Database.GetParameters(whereObj));
            }
            return this;
        }

        public IDeleteable<T> In<PkType>(PkType[] primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0) {
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
            else {

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
            return new KeyValuePair<string, List<SugarParameter>>(sql,paramters);
        }
    }
}
