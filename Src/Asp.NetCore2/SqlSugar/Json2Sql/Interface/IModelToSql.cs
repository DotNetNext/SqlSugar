using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    /// <summary>
    /// Json Model to sql
    /// </summary>
    public partial interface ISqlBuilder
    {
        KeyValuePair<string, SugarParameter[]> OrderByModelToSql(List<OrderByModel> models);
        KeyValuePair<string, SugarParameter[]> GroupByModelToSql(List<GroupByModel> models);
        KeyValuePair<string, SugarParameter[]> SelectModelToSql(List<SelectModel> models);
        KeyValuePair<string, SugarParameter[]> FuncModelToSql(IFuncModel model);
    }

}
