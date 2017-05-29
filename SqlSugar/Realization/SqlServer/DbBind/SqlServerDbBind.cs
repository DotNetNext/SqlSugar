using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public class SqlServerDbBind : DbBindProvider
    {
        public override string ChangeDBTypeToCSharpType(string typeName)
        {
                string reval = string.Empty;
                switch (typeName.ToLower())
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
                    case "binary":
                        reval = "object";
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
                        reval = "dateTime";
                        break;
                    case "single":
                    case "decimal":
                        reval = "decimal";
                        break;
                    case "float":
                        reval = "double";
                        break;
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
                        reval = "dateTime";
                        break;
                    case "smallint":
                        reval = "short";
                        break;
                    case "smallmoney":
                        reval = "decimal";
                        break;
                    case "timestamp":
                        reval = "dateTime";
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
    }
}
