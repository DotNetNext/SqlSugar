using Dm.util;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.MongoDb
{
    public class MongoDbQueryable<T> : QueryableProvider<T>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            return BuildJoinQueryInfo(joinExpression, joinType);
        }

        public JoinQueryInfo BuildJoinQueryInfo(Expression joinExpression, JoinType joinType)
        {
            var exp = (joinExpression as LambdaExpression);
            var queryBuilder = (MongoDbQueryBuilder)this.QueryBuilder;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, this.Context);
        }
        public override ISugarQueryable<T> WhereClassByPrimaryKey(List<T> list)
        {
            var filterDoc = new BsonDocument();

            if (list.HasValue())
            {
                var columns = this.Context.EntityMaintenance.GetEntityInfo<T>()
                    .Columns.Where(it => it.IsIgnore == false && it.IsPrimarykey == true).ToList();

                Check.Exception(columns == null || columns.Count == 0, "{0} no primary key, Can not use WhereClassByPrimaryKey ", typeof(T).Name);
                Check.Exception(this.QueryBuilder.IsSingle() == false, "No support join query");

                var orArray = new BsonArray();

                foreach (var item in list)
                {
                    var andDoc = new BsonDocument();

                    foreach (var column in columns)
                    {
                        var value = column.PropertyInfo.GetValue(item, null);
                        BsonValue bsonValue;

                        if (value is Enum && this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString != true)
                        {
                            bsonValue = new BsonInt64(Convert.ToInt64(value));
                        }
                        else if (value != null && column.UnderType == UtilConstants.DateType)
                        {
                            bsonValue = new BsonDateTime(Convert.ToDateTime(value));
                        }
                        else
                        {
                            if (column.IsPrimarykey&& value!=null) 
                            {
                                value = ObjectId.Parse(value?.ToString());
                            }
                            bsonValue = BsonValue.Create(value);
                        }

                        andDoc[column.DbColumnName] = bsonValue;
                    }

                    if (andDoc.ElementCount > 0)
                    {
                        orArray.Add(andDoc);
                    }
                }

                if (orArray.Count == 1)
                {
                    filterDoc = orArray[0].AsBsonDocument;
                }
                else
                {
                    filterDoc = new BsonDocument("$or", orArray);
                }
            }
            else
            {
                // 等价于 WHERE 1=2
                filterDoc = new BsonDocument("$expr", new BsonDocument("$eq", new BsonArray { 1, 2 }));
            }
            this.QueryBuilder.WhereInfos.Add(filterDoc.ToJson(UtilMethods.GetJsonWriterSettings()));
            return this;
        }
        public override int Count()
        { 
            return  GetCount(); ;
        }
        public override Task<int> CountAsync()
        {
            return GetCountAsync();
        }
        public override ISugarQueryable<T> With(string withString)
        {
            return this;
        }

        public override ISugarQueryable<T> PartitionBy(string groupFileds)
        {
            this.GroupBy(groupFileds);
            return this;
        }
    }
    public class MongoDbQueryable<T, T2> : QueryableProvider<T, T2>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        { 
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
        public new ISugarQueryable<T, T2> With(string withString)
        {
            return this;
        }
    }
    public class MongoDbQueryable<T, T2, T3> : QueryableProvider<T, T2, T3>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4> : QueryableProvider<T, T2, T3, T4>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5> : QueryableProvider<T, T2, T3, T4, T5>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6> : QueryableProvider<T, T2, T3, T4, T5, T6>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7> : QueryableProvider<T, T2, T3, T4, T5, T6, T7>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7, T8> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
    public class MongoDbQueryable<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : QueryableProvider<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        public override JoinQueryInfo GetJoinInfo(Expression joinExpression, JoinType joinType)
        {
            var queryBuilder = this.QueryBuilder;
            var context = this.Context;
            return UtilMethods.BuilderJoinInfo(joinExpression, joinType, queryBuilder, context);
        }
    }
}
