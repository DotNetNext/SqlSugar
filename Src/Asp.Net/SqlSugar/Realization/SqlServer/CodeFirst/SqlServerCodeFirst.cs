using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqlServerCodeFirst:CodeFirstProvider
    {
        protected override string GetTableName(EntityInfo entityInfo)
        {
            var table= this.Context.EntityMaintenance.GetTableName(entityInfo.EntityName);
            var tableArray = table.Split('.');
            var noFormat = table.Split(']').Length==1;
            if (tableArray.Length > 1 && noFormat)
            {
                var dbMain = new SqlServerDbMaintenance() { Context = this.Context };
                var schmes = dbMain.GetSchemas();
                if (!schmes.Any(it => it.EqualCase(tableArray.First())))
                {
                    return tableArray.Last();
                }
                else 
                {
                    return dbMain.SqlBuilder.GetTranslationTableName(table);
                }
            }
            else
            {
                return table;
            }
        }

        protected override void GetDbType(EntityColumnInfo item, Type propertyType, DbColumnInfo result)
        {
            if (!string.IsNullOrEmpty(item.DataType))
            {
                result.DataType = item.DataType;
            }
            else if (propertyType.IsEnum())
            {
                result.DataType = this.Context.Ado.DbBind.GetDbTypeName(item.Length > 9 ? UtilConstants.LongType.Name : UtilConstants.IntType.Name);
            }
            else
            {
                var name = GetType(propertyType.Name);
                result.DataType = this.Context.Ado.DbBind.GetDbTypeName(name);
                if (result.DataType == "varbinary" && item.Length == 0)
                {
                    result.DataType = "varbinary(max)";
                }
            }
        }
    }
}
