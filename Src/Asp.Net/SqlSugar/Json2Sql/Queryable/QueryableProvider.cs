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
        public ISugarQueryable<T> AddJoinInfo(List<JoinInfoParameter> joinInfoParameters) 
        {
            if (joinInfoParameters != null)
            {
                foreach (var item in joinInfoParameters)
                {
                    this.AddJoinInfo(item.TableName,item.ShortName,item.Models,item.Type);
                }
            }
            return this;
        }
        public ISugarQueryable<T> AS(string tableName, string shortName) 
        {
            return this.AS($"{this.SqlBuilder.GetTranslationTableName(tableName)} {shortName}");
        }
        public ISugarQueryable<T> OrderBy(List<OrderByModel> models) 
        {
            if (models == null || models.Count == 0) 
            {
                return this;
            }
            var orderObj = this.SqlBuilder.OrderByModelToSql(models);
            this.OrderBy(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
        public ISugarQueryable<T> GroupBy(List<GroupByModel> models)
        {
            if (models == null || models.Count == 0)
            {
                return this;
            }
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

        public ISugarQueryable<TResult> Select<TResult>(List<SelectModel> models) 
        {
            var orderObj = this.SqlBuilder.SelectModelToSql(models);
            var result=this.Select<TResult>(orderObj.Key);
            result.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return result;
        }



        public ISugarQueryable<T> Select(List<SelectModel> models, AsNameFormatType type) 
        {
            if (type == AsNameFormatType.NoConvert) 
            {
                foreach (var model in models)
                {
                    if (!string.IsNullOrEmpty(model.AsName))
                    {
                        model.AsName = (UtilConstants.ReplaceKey + SqlBuilder.SqlTranslationLeft + model.AsName + SqlBuilder.SqlTranslationRight);
                        model.AsName.ToCheckField();
                    }
                }
            }
            return Select(models);
        }
        public ISugarQueryable<T> Having(IFuncModel model)
        {
            this.QueryBuilder.WhereIndex++;
            var orderObj = this.SqlBuilder.FuncModelToSql(model);
            this.Having(orderObj.Key);
            this.QueryBuilder.Parameters.AddRange(orderObj.Value);
            return this;
        }
    }
}
