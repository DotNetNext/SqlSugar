using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class SqlServerDbBind : DbBindProvider
    {
        public override string GetCSharpType(string dbTypeName)
        {
            string reval = string.Empty;
            switch (dbTypeName.ToLower())
            {
                case "int":
                    reval = "int";
                    break;
                case "text":
                    reval = "string";
                    break;
                case "bigint":
                    reval = "long";
                    break;
                case "bit":
                    reval = "bool";
                    break;
                case "char":
                    reval = "string";
                    break;
                case "datetime":
                case "date":
                case "datetime2":
                    reval = "DateTime";
                    break;
                case "single":
                case "decimal":
                    reval = "decimal";
                    break;
                case "float":
                    reval = "double";
                    break;
                case "binary":
                case "image":
                    reval = "byte[]";
                    break;
                case "money":
                    reval = "decimal";
                    break;
                case "nchar":
                    reval = "string";
                    break;
                case "ntext":
                    reval = "string";
                    break;
                case "numeric":
                    reval = "decimal";
                    break;
                case "nvarchar":
                    reval = "string";
                    break;
                case "real":
                    reval = "float";
                    break;
                case "smalldatetime":
                    reval = "DateTime";
                    break;
                case "smallint":
                    reval = "short";
                    break;
                case "smallmoney":
                    reval = "decimal";
                    break;
                case "timestamp":
                    reval = "DateTime";
                    break;
                case "tinyint":
                    reval = "byte";
                    break;
                case "uniqueidentifier":
                    reval = "guid";
                    break;
                case "varbinary":
                    reval = "byte[]";
                    break;
                case "varchar":
                    reval = "string";
                    break;
                case "Variant":
                    reval = "object";
                    break;
                default:
                    reval = "other";
                    break;
            }
            return reval;
        }
        public override string GetCSharpConvert(string dbTypeName)
        {
            string reval = string.Empty;
            switch (dbTypeName.ToLower())
            {
                #region Int
                case "int":
                    reval = "Convert.ToInt32";
                    break;
                #endregion

                #region String
                case "nchar":
                case "char":
                case "ntext":
                case "nvarchar":
                case "varchar":
                case "text":
                    reval = "Convert.ToString";
                    break;
                #endregion

                #region Long
                case "bigint":
                    reval = "Convert.ToInt64";
                    break;
                #endregion

                #region Bool
                case "bit":
                    reval = "Convert.ToBoolean";
                    break;

                #endregion

                #region Datetime
                case "timestamp":
                case "smalldatetime":
                case "datetime":
                case "date":
                case "datetime2":
                    reval = "Convert.ToDateTime";
                    break;
                #endregion

                #region Decimal
                case "smallmoney":
                case "single":
                case "numeric":
                case "money":
                case "decimal":
                    reval = "Convert.ToDecimal";
                    break;
                #endregion

                #region Double
                case "float":
                    reval = "Convert.ToDouble";
                    break;
                #endregion

                #region Byte[]
                case "varbinary":
                case "binary":
                case "image":
                    reval = "byte[]";
                    break;
                #endregion

                #region Float
                case "real":
                    reval = "Convert.ToSingle";
                    break;
                #endregion

                #region Short
                case "smallint":
                    reval = "Convert.ToInt16";
                    break;
                #endregion

                #region Byte
                case "tinyint":
                    reval = "Convert.ToByte";
                    break;

                #endregion

                #region Guid
                case "uniqueidentifier":
                    reval = "Guid.Parse";
                    break;
                #endregion

                #region Null
                default:
                    reval = null;
                    break; 
                    #endregion
            }
            return reval;
        }
    }
}
