using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlSugar
{
    // BulkUpdate/BulkMerge 经临时表 JOIN 更新；ToDdateTable 剥离 identity 列后 JOIN
    // 键列缺值会使 UPDATE 静默命中 0 行——此处按实体声明补回缺失列及其真实值。
    internal static class VastbaseV5BulkCopySupport
    {
        internal static void RestoreWhereColumns<T>(DataTable dt, List<T> datas, string[] whereColumns, EntityInfo entityInfo)
        {
            // temp 链标记：此 dt 是 BulkUpdate/Merge 的临时表载荷（PG 基类 temp 名无 # 前缀，
            // 表名不可判），identity 剥列段见标记即豁免——JOIN 键列须保留真实值。
            dt.ExtendedProperties["VastbaseV5TempChain"] = true;
            foreach (var wc in whereColumns)
            {
                if (!dt.Columns.Contains(wc))
                {
                    // 主库不再预剥 identity 列（零主码方案）：dt 可能已带同名物理列（大小写或与
                    // 实体声明名一致）。精确 Contains 判否后重复加列会使 COPY 列清单出现双列
                    //（列名小写化后冲突）——EqualCase 同名已存在时仅归一列名为实体声明名。
                    var existing = dt.Columns.Cast<DataColumn>().FirstOrDefault(dc => dc.ColumnName.EqualCase(wc));
                    if (existing != null)
                    {
                        existing.ColumnName = wc;
                        continue;
                    }
                    var entityCol = entityInfo.Columns.FirstOrDefault(c => (c.DbColumnName ?? c.PropertyName) == wc);
                    if (entityCol != null)
                    {
                        var colType = Nullable.GetUnderlyingType(entityCol.PropertyInfo.PropertyType) ?? entityCol.PropertyInfo.PropertyType;
                        dt.Columns.Add(wc, colType);
                        for (int i = 0; i < datas.Count && i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i][wc] = entityCol.PropertyInfo.GetValue(datas[i]) ?? DBNull.Value;
                        }
                    }
                }
            }
        }
    }
}
