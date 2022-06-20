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
            SugarParameter[] pars = new SugarParameter[] { };
            foreach (var item in models)
            {
                if (item is OrderByModel)
                {
                    var orderByModel = item as OrderByModel;
                    sql.Append($" {this.GetTranslationColumnName(orderByModel.FieldName.ToSqlFilter())} {orderByModel.OrderByType.ToString().ToUpper()} ,");
                }
                else
                {

                }

            }
            return new KeyValuePair<string, SugarParameter[]>(sql.ToString().TrimEnd(','), pars);
        }
    }
}
