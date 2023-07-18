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
        internal CustomAttributeBuilder GetSplitEntityAttr(SplitTableAttribute sugarTable)
        {
            Type attributeType = typeof(SplitTableAttribute);
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] {  typeof(SplitType)  });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { sugarTable.SplitType },
            new PropertyInfo[] {
                  attributeType.GetProperty(nameof(SplitTableAttribute.SplitType)),
            }
            , new object[] {
                    sugarTable.SplitType
             });
            return attributeBuilder;
        }
        internal CustomAttributeBuilder GetSplitFieldAttr(SplitFieldAttribute fieldAttribute)
        {
            Type attributeType = typeof(SplitFieldAttribute);
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] {   });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { });
            return attributeBuilder;
        }
        internal CustomAttributeBuilder GetEntity(SugarTable sugarTable)
        {
            Type attributeType = typeof(SugarTable);
            ConstructorInfo attributeCtor = attributeType.GetConstructor(new Type[] { typeof(string) });
            CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(attributeCtor, new object[] { "" },
            new PropertyInfo[] {
                  attributeType.GetProperty(nameof(SugarTable.TableName)),
                  attributeType.GetProperty(nameof(SugarTable.TableDescription)) ,
                  attributeType.GetProperty(nameof(SugarTable.IsDisabledUpdateAll)) ,
                  attributeType.GetProperty(nameof(SugarTable.IsDisabledDelete)),
                  attributeType.GetProperty(nameof(SugarTable.IsCreateTableFiledSort)),
                  attributeType.GetProperty(nameof(SugarTable.Discrimator))
            }
            , new object[] {
                    sugarTable.TableName,
                    sugarTable.TableDescription ,
                    sugarTable.IsDisabledUpdateAll,
                    sugarTable.IsDisabledDelete,
                    sugarTable.IsCreateTableFiledSort,
                    sugarTable.Discrimator
             });
            return attributeBuilder;
        }
        internal CustomAttributeBuilder GetProperty(SugarColumn sugarTable)
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
                  attributeType.GetProperty(nameof(SugarColumn.IsArray)),
                  attributeType.GetProperty(nameof(SugarColumn.ColumnName))
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
                    sugarTable.IsArray,
                    sugarTable.ColumnName
             });
            return attributeBuilder;
        }
    }
}
