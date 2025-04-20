using SqlSugar;

Console.Read();
/// <summary>
/// Helper class for database operations
/// 数据库操作的辅助类
/// </summary>
public class DbHelper
{
    /// <summary>
    /// Database connection string
    /// 数据库连接字符串
    /// </summary>
    public readonly static string Connection = "DataSource = train_services.db";

    /// <summary>
    /// Get a new SqlSugarClient instance with specific configurations
    /// 获取具有特定配置的新 SqlSugarClient 实例
    /// </summary>
    /// <returns>SqlSugarClient instance</returns>
    public static SqlSugarClient GetNewDb()
    {
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            IsAutoCloseConnection = true,
            DbType = DbType.DuckDB,
            ConnectionString = Connection,
            LanguageType = LanguageType.Default//Set language

        },
        it => {
            // Logging SQL statements and parameters before execution
            // 在执行前记录 SQL 语句和参数
            it.Aop.OnLogExecuting = (sql, para) =>
            {
                Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
            };
        });
        return db;
    }