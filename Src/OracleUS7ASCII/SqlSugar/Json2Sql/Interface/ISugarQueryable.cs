using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public partial interface ISugarQueryable<T>
    {
        ISugarQueryable<T> Having(IFuncModel model);
        ISugarQueryable<T> OrderBy(List<OrderByModel> models);
        ISugarQueryable<T> GroupBy(List<GroupByModel> models);
        ISugarQueryable<T> Select(List<SelectModel> models);
        ISugarQueryable<TResult> Select<TResult>(List<SelectModel> models);
        ISugarQueryable<T> Select(List<SelectModel> models,AsNameFormatType type);
        ISugarQueryable<T> AS(string tableName, string shortName);
        ISugarQueryable<T> AddJoinInfo(string tableName, string shortName, IFuncModel models, JoinType type = JoinType.Left);
        ISugarQueryable<T> AddJoinInfo(List<JoinInfoParameter> joinInfoParameters);
    }
}
