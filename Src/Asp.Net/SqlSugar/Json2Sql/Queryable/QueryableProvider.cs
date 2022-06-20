using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{

    public partial class QueryableProvider<T> : QueryableAccessory, ISugarQueryable<T>
    {
        public ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, IFuncModel models, JoinType type = JoinType.Left) 
        {
            var sqlobj = this.SqlBuilder.FuncModelToSql(models);
            this.QueryBuilder.Parameters.AddRange(sqlobj.Value);
            return this.AddJoinInfo(tableName, shortName, sqlobj.Key, type);
        }
        public ISugarQueryable<T> AS(string tableName, string shortName) 
        {
            return this.AS($"{this.SqlBuilder.GetTranslationTableName(tableName)} {shortName}");
        }
        public ISugarQueryable<T> OrderBy(List<OrderByModel> models) 
        {
            var orderObj = this.SqlBuilder.OrderByModelToSql(models);
            this.OrderBy(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
        public ISugarQueryable<T> GroupBy(List<GroupByModel> models)
        {
            var orderObj = this.SqlBuilder.GroupByModelToSql(models);
            this.GroupBy(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
        public ISugarQueryable<T> Select(List<SelectModel> models)
        {
            var orderObj = this.SqlBuilder.SelectModelToSql(models);
            this.Select(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
        public ISugarQueryable<T> Having(IFuncModel model)
        {
            var orderObj = this.SqlBuilder.FuncModelToSql(model);
            this.Having(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
    }
}
