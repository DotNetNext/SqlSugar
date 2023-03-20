using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DynamicBuilder
    {
        private CustomAttributeBuilder GetEntity(SugarTable sugarTable)
        {
            Type attributeType = typeof(SugarTable);
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] { typeof(string) });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { "" },
            new PropertyInfo[] {
                  attributeType.GetProperty(nameof(SugarTable.TableName)),
                  attributeType.GetProperty(nameof(SugarTable.TableDescription)) ,
                  attributeType.GetProperty(nameof(SugarTable.IsDisabledUpdateAll)) ,
                  attributeType.GetProperty(nameof(SugarTable.IsDisabledDelete))
            }
            , new object[] {
                    sugarTable.TableName,
                    sugarTable.TableDescription ,
                    sugarTable.IsDisabledUpdateAll,
                    sugarTable.IsDisabledDelete
             });
            return attributeBuilder;
        }
        private CustomAttributeBuilder GetProperty(SugarColumn sugarTable)
        {
            Type attributeType = typeof(SugarColumn);
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] { });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { },
            new PropertyInfo[] {
                  attributeType.GetProperty(nameof(SugarColumn.IsPrimaryKey)),
                  attributeType.GetProperty(nameof(SugarColumn.IsIdentity)),
                  attributeType.GetProperty(nameof(SugarColumn.DefaultValue)),
                  attributeType.GetProperty(nameof(SugarColumn.Length)),
                  attributeType.GetProperty(nameof(SugarColumn.DecimalDigits)),
                  attributeType.GetProperty(nameof(SugarColumn.ColumnDataType)),
                  attributeType.GetProperty(nameof(SugarColumn.IsNullable)),
                  attributeType.GetProperty(nameof(SugarColumn.ColumnDescription)),
                  attributeType.GetProperty(nameof(SugarColumn.OracleSequenceName)),
                  attributeType.GetProperty(nameof(SugarColumn.IsIgnore)),
                  attributeType.GetProperty(nameof(SugarColumn.IsJson)),
                  attributeType.GetProperty(nameof(SugarColumn.IsOnlyIgnoreInsert)),
                  attributeType.GetProperty(nameof(SugarColumn.IsOnlyIgnoreUpdate)),
                  attributeType.GetProperty(nameof(SugarColumn.OldColumnName)),
                  attributeType.GetProperty(nameof(SugarColumn.SqlParameterDbType)),
                  attributeType.GetProperty(nameof(SugarColumn.SqlParameterSize)),
                  attributeType.GetProperty(nameof(SugarColumn.IsArray))
            }
            , new object[] {
                    sugarTable.IsPrimaryKey,
                    sugarTable.IsIdentity,
                    sugarTable.DefaultValue,
                    sugarTable.Length,
                    sugarTable.DecimalDigits,
                    sugarTable.ColumnDataType,
                    sugarTable.IsNullable,
                    sugarTable.ColumnDescription,
                    sugarTable.OracleSequenceName,
                    sugarTable.IsIgnore,
                    sugarTable.IsJson,
                    sugarTable.IsOnlyIgnoreInsert,
                    sugarTable.IsOnlyIgnoreUpdate,
                    sugarTable.OldColumnName,
                    sugarTable.SqlParameterDbType,
                    sugarTable.SqlParameterSize,
                    sugarTable.IsArray
             });
            return attributeBuilder;
        }
    }
}
