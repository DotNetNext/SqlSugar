using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        public KeyValuePair<string, SugarParameter[]> GroupByModelToSql(List<GroupByModel> models)
        {
            StringBuilder sql = new StringBuilder("");
            SugarParameter[] pars = new SugarParameter[] { };
            foreach (var item in models)
            {
                if (item is GroupByModel)
                {
                    var orderByModel = item as GroupByModel;
                    sql.Append($" {this.GetTranslationColumnName(orderByModel.FieldName.ToSqlFilter())} ,");
                }
                else
                {

                }

            }
            return new KeyValuePair<string, SugarParameter[]>(sql.ToString().TrimEnd(','), pars);
        }
    }
}
