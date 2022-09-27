using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        public KeyValuePair<string, SugarParameter[]> SelectModelToSql(List<SelectModel> models)
        {
            StringBuilder sql = new StringBuilder("");
            var pars = new List<SugarParameter> { };
            foreach (var item in models)
            {
                if (item is SelectModel)
                {
                    var orderByModel = item as SelectModel;
                    orderByModel.AsName=GetAsName(orderByModel);
                    orderByModel.FiledName = GetSqlPart(orderByModel.FiledName, pars).ObjToString();
                    AppendFiledName(sql, orderByModel);
                }
                else
                {

                }
            }
            return new KeyValuePair<string, SugarParameter[]>(sql.ToString().TrimEnd(','), pars.ToArray());
        }

        private string GetAsName(SelectModel orderByModel)
        {
            if (orderByModel.AsName.IsNullOrEmpty())
            {
                orderByModel.AsName = orderByModel.FiledName.ObjToString();
            }
            if (orderByModel.AsName.StartsWith(UtilConstants.ReplaceKey)) 
            {
                return orderByModel.AsName.Replace(UtilConstants.ReplaceKey, string.Empty);
            }
          return this.GetTranslationColumnName(orderByModel.AsName);
        }

        private  void AppendFiledName(StringBuilder sql, SelectModel orderByModel)
        {
            sql.Append($" {orderByModel.FiledName} AS {orderByModel.AsName} ,");
        }
    }
}
