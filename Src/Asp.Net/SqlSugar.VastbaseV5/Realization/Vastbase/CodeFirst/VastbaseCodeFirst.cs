using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    public class VastbaseV5CodeFirst : PostgreSQLCodeFirst
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        protected override void ConvertColumns(List<DbColumnInfo> dbColumns)
        {
            base.ConvertColumns(dbColumns);
            var mode = Mode;
            if (mode == null || !mode.IsBabelfish)
                return;
            foreach (var item in dbColumns)
            {
                mode.NormalizeColumn(item);
                FixBinaryMaxLength(item);
            }
        }

        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var result = base.EntityColumnToDbColumn(entityInfo, tableName, item);
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                mode.NormalizeColumn(result);
                FixBinaryMaxLength(result);
            }
            return result;
        }

        // Babelfish：DDL 生成物理列时已剥尾部空格（VastbaseV5Builder.GetTranslationColumnName TrimEnd），实体列名带尾部空格在 ExistLogic 对比失配 → 误判缺列 → 重复 ADD 报 already exists。同步前对实体列名做同一归一（临时改写 + finally 还原，不动按类型缓存的元数据）。表名不归一：base 经 GetTableName 先查 MappingTables（未 trim 的注册值），且读回/建表由 T-SQL 忽略尾部空格语义 + TrimTrailingSpace 处理。
        public override void ExistLogic(EntityInfo entityInfo)
        {
            var mode = Mode;
            if (mode == null || !mode.IsBabelfish)
            {
                base.ExistLogic(entityInfo);
                return;
            }
            var origNames = entityInfo.Columns.ToDictionary(c => c, c => c.DbColumnName);
            foreach (var c in entityInfo.Columns)
                if (c.DbColumnName != null) c.DbColumnName = c.DbColumnName.TrimEnd();
            try
            {
                base.ExistLogic(entityInfo);
            }
            finally
            {
                foreach (var kv in origNames) kv.Key.DbColumnName = kv.Value;
            }
        }

        private static void FixBinaryMaxLength(DbColumnInfo col)
        {
            if (col.DataType != null && col.DataType.EqualCase("varbinary") && col.Length <= 0)
                col.Length = -1;
        }

        // Babelfish 数组列降级 text：实体侧显式 ColumnDataType（"int8[]"）经 NormalizeTypeName
        // 归一为 text，已存在列侧也归一 → 重跑 InitTables 类型比较须按归一后的 text 比较，
        // 否则每跑一次都触发无害但多余的 ALTER COLUMN（text→text）。
        // 比较规则：自身对归一前后的对比用 Ordinal（区分 "int8[]" 与 "int8[] " 这类空白变体，
        // 空白变体也需归一）；与 DB 侧类型对比用 OrdinalIgnoreCase（T-SQL 类型名大小写不敏感）。
        protected override bool IsNoSamgeType(EntityColumnInfo ec, DbColumnInfo dc)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish && !string.IsNullOrEmpty(ec.DataType))
            {
                var normalized = mode.NormalizeTypeName(ec.DataType);
                if (!string.Equals(normalized, ec.DataType, StringComparison.Ordinal))
                    return !string.Equals(normalized, dc.DataType, StringComparison.OrdinalIgnoreCase);
            }
            return base.IsNoSamgeType(ec, dc);
        }
    }

    public class VastbaseV5DbFirst : PostgreSQLDbFirst
    {
    }
}