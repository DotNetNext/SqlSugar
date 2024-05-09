using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        public KeyValuePair<string, SugarParameter[]> OrderByModelToSql(List<OrderByModel> models)
        {
            StringBuilder sql = new StringBuilder("");
            List<SugarParameter>  pars = new List<SugarParameter>(){ };
            foreach (var item in models)
            {
                if (item is OrderByModel && item.FieldName is IFuncModel) 
                {
                    var orderByModel = item as OrderByModel;
                    sql.Append($" {GetSqlPart(item.FieldName,pars)} {orderByModel.OrderByType.ToString().ToUpper()} ,");
                }
                else if (item is OrderByModel)
                {
                    var orderByModel = item as OrderByModel;
                    sql.Append($" {this.GetTranslationColumnName(orderByModel.FieldName.ObjToString().ToSqlFilter())} {orderByModel.OrderByType.ToString().ToUpper()} ,");
                }
                else
                {

                }

            }
            return new KeyValuePair<string, SugarParameter[]>(sql.ToString().TrimEnd(','), pars?.ToArray());
        }
    }
}
