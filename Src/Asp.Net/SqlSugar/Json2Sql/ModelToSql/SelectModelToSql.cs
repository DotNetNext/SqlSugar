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
                    orderByModel.FieldName = GetSqlPart(orderByModel.FieldName, pars).ObjToString();
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
                orderByModel.AsName = orderByModel.FieldName.ObjToString();
            }
            if (orderByModel.AsName.StartsWith(UtilConstants.ReplaceKey)) 
            {
                return orderByModel.AsName.Replace(UtilConstants.ReplaceKey, string.Empty);
            }
            if (orderByModel.AsName?.Contains("[")==true) 
            {
                orderByModel.AsName = orderByModel.AsName.Trim('[').Trim(']');
                return this.SqlTranslationLeft + orderByModel.AsName + this.SqlTranslationRight;
            }
            if (this.SqlTranslationLeft != null && orderByModel.AsName?.Contains(this.SqlTranslationLeft) == true) 
            {
                return orderByModel.AsName;
            }
            return this.SqlTranslationLeft + orderByModel.AsName + this.SqlTranslationRight;
        }

        private  void AppendFiledName(StringBuilder sql, SelectModel orderByModel)
        {
            sql.Append($" {orderByModel.FieldName} AS {orderByModel.AsName} ,");
        }
    }
}
