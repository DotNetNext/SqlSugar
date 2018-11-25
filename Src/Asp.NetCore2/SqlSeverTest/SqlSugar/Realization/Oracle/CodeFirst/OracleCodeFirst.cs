using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OracleCodeFirst : CodeFirstProvider
    {
        public OracleCodeFirst() {
            if (DefultLength == 0)
                DefultLength = 40;
        }
        protected override int DefultLength { get; set; }

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
                if (propertyType.Name.Equals("Guid", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.DataType = this.Context.Ado.DbBind.GetDbTypeName(UtilConstants.StringType.Name);
                }
                else
                {
                    result.DataType = this.Context.Ado.DbBind.GetDbTypeName(propertyType.Name);
                }
            }
        }

        protected override void KeyAction(EntityColumnInfo item, DbColumnInfo dbColumn, out bool pkDiff, out bool idEntityDiff)
        {
            pkDiff = item.IsPrimarykey != dbColumn.IsPrimarykey;
            idEntityDiff = false;
        }
    }
}
