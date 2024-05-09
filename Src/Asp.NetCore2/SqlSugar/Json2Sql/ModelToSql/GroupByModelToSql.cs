using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.CompilerServices;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        public KeyValuePair<string, SugarParameter[]> GroupByModelToSql(List<GroupByModel> models)
        {
            StringBuilder sql = new StringBuilder("");
            var pars = new List<SugarParameter> { };
            foreach (var item in models)
            {
                if (item is GroupByModel && item.FieldName is IFuncModel) 
                {
                    var orderByModel = item as GroupByModel;
                    sql.Append($" {GetSqlPart(item.FieldName, pars)} ,");
                }
                else if (item is GroupByModel)
                {
                    var orderByModel = item as GroupByModel;
                    sql.Append($" {this.GetTranslationColumnName(orderByModel.FieldName.ObjToString().ToSqlFilter())} ,");
                }
                else
                {

                }

            }
            return new KeyValuePair<string, SugarParameter[]>(sql.ToString().TrimEnd(','), pars?.ToArray());
        }
    }
}
