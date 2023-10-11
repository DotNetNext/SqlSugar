using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MySqlDbFirst : DbFirstProvider
    {
        protected override string GetPropertyTypeName(DbColumnInfo item)
        {
            if (item.DataType == "tinyint" && item.Length == 1&&this.Context.CurrentConnectionConfig.ConnectionString.ToLower().Contains("treattinyasboolea")==false) 
            {
                item.DataType = "bit";
                item.DefaultValue = "true";
                return "bool";
            }
            if (item.DataType == "mediumint")
            {
                item.DataType = "int"; 
                return "int";
            }
            if (item.DataType == "mediumint unsigned")
            {
                item.DataType = "mediumint unsigned";
                return "uint";
            }
            if (item.DataType == "double unsigned") 
            {
                return "double";
            }
            return base.GetPropertyTypeName(item);
        }
    }
}
