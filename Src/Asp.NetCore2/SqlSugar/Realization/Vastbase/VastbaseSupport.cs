using System.Reflection;
using System;

namespace SqlSugar
{
    /// <summary>
    /// VastbaseV5 构造期装配器：由 SqlSugarAccessory.CheckDbDependency 调用（构造期先于元数据缓存读，免竞态）。装配：Utilities 换入 Vastbase 专属 ContextMethods（缓存按兼容模式分键）；EntityService 组合（保留用户回调，追加元数据归一）。
    /// </summary>
    internal static class VastbaseV5Support
    {
        public static void Install(SqlSugarProvider context)
        {
            // 兼容模式探测与回填（原 SqlSugarAccessory 的 VastbaseV5 case 体，构造期同点
            // 搬入）：连接串 Babelfish 标记或显式 MoreSettings 设置即锁定 Babelfish 模式；
            // 显式设置而无标记时回填标记，驱动连接与模式判定同源。先于下方 isBabelfish
            // 快照执行，保证快照看到终态。
            var cfg = context.CurrentConnectionConfig;
            if (cfg.MoreSettings == null)
                cfg.MoreSettings = new ConnMoreSettings();
            var hasBabelfishMarker = VastbaseV5Helper.IsBabelfishConnectionString(cfg.ConnectionString);
            if (hasBabelfishMarker
                || cfg.MoreSettings.VastbaseCompatibility == VastbaseCompatibilityMode.Babelfish)
            {
                cfg.MoreSettings.VastbaseCompatibility = VastbaseCompatibilityMode.Babelfish;
                if (!hasBabelfishMarker && !string.IsNullOrEmpty(cfg.ConnectionString))
                {
                    cfg.ConnectionString += ";Server Compatibility Mode=Babelfish";
                }
            }
            var isBabelfish = VastbaseV5Helper.IsBabelfish(context);
            context.Utilities = new VastbaseV5ContextMethods(context, isBabelfish);
            var config = context.CurrentConnectionConfig;
            if (config.ConfigureExternalServices == null)
                config.ConfigureExternalServices = new ConfigureExternalServices();
            var externalServices = config.ConfigureExternalServices;
            var userCallback = externalServices.EntityService;
            externalServices.EntityService = (property, column) =>
            {
                if (userCallback != null) userCallback(property, column);
                Normalize(property, column, isBabelfish);
            };
        }

        private static void Normalize(PropertyInfo property, EntityColumnInfo column, bool isBabelfish)
        {
            // Babelfish:T-SQL 语义下标识符尾部空格无意义(CodeFirst DDL 生成时同样
            // 被剥);元数据层归一,使绑定、映射注册、表达式解析三面读到同一形态。
            if (isBabelfish && !string.IsNullOrEmpty(column.DbColumnName))
                column.DbColumnName = column.DbColumnName.TrimEnd();
            // Vastbase PG 模式:IsJson 列落 json(json 运算符仅对 json 类型定义;
            // Babelfish T-SQL 仍按 nvarchar 语义)。重读特性精确重建「IsJson 且未
            // 显式指定列类型」前置,显式 ColumnDataType 不受影响。
            if (!isBabelfish && property != null)
            {
                var attr = property.GetCustomAttribute<SugarColumn>();
                if (attr != null && attr.IsJson && string.IsNullOrEmpty(attr.ColumnDataType))
                    column.DataType = "json";
            }
        }
    }
}
