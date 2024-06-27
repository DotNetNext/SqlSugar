namespace SqlSugar.Xugu
{
    ///// <summary>
    ///// 入口点
    ///// </summary>
    //public static class XuguEntry
    //{
    //    /// <summary>
    //    /// 使用虚谷数据库，并进行配置。
    //    /// <code>
    //    /// 引用包 SqlSugar.XuguCore
    //    /// using SqlSugar.Xugu;
    //    /// 
    //    /// protected static SqlSugarScope db = new SqlSugarScope(new ConnectionConfig()
    //    /// {
    //    ///     ConnectionString = "IP=127.0.0.1;DB=SYSTEM;User=SYSDBA;PWD=SYSDBA;Port=5138;AUTO_COMMIT=on;CHAR_SET=GBK",
    //    ///     DbType = DbType.Custom.UseXugu(),
    //    ///     IsAutoCloseConnection = true,
    //    /// });
    //    /// 不需要对 InstanceFactory.CustomDbName 等进行配置，已经配置好了
    //    /// 仅实现了简单的增删改查，未实现函数，未实现返回插入主键等高级用法
    //    /// </code>
    //    /// </summary>
    //    /// <param name="type">任意数据库类型，建议Custom</param>
    //    /// <returns>DbType.Custom</returns>
    //    public static DbType UseXugu(this DbType type)
    //    {
    //        InstanceFactory.CustomDbName = "Xugu";//文件名前缀
    //        InstanceFactory.CustomDllName = "SqlSugar.XuguCore";//扩展的dll名字
    //        InstanceFactory.CustomNamespace = "SqlSugar.Xugu";//扩展dll的命名空间
    //        return DbType.Custom;
    //    }
    //}
}