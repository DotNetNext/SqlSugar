using DB2CoreTest.CURD;
using SqlSugar;

public class Program
{
    static void Main(string[] args)
    {
        //CodeFirst.Init();
        CodeFirst.Insertable();
        CodeFirst.Queryable();
        CodeFirst.QueryableSelect();
        CodeFirst.JoinQuery();
        CodeFirst.PageQuery();
        CodeFirst.OrderBy();
        CodeFirst.GroupBy();
        CodeFirst.Updateable();
        CodeFirst.Deleteable();

        Console.ReadLine();
    }
}

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
   public readonly static string Connection = "server=127.0.0.1:50000;database=Test;uid=db2inst1;pwd=123456;currentSchema=db2inst1;";

    /// <summary>
    /// Get a new SqlSugarClient instance with specific configurations
    /// 获取具有特定配置的新 SqlSugarClient 实例
    /// </summary>
    /// <returns>SqlSugarClient instance</returns>
    public static SqlSugarClient GetNewDb()
    { 

        //这行代码扔程序启动时
        InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
            typeof(SqlSugar.DB2.DB2Provider).Assembly };


        //创建数据库
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            IsAutoCloseConnection = true, 
            DbType=DbType.DB2, 
            ConnectionString = Connection,
            InitKeyType = InitKeyType.Attribute,
            LanguageType = LanguageType.Default//Set language
        },
        it =>
        {
            // Logging SQL statements and parameters before execution
            // 在执行前记录 SQL 语句和参数
            it.Aop.OnLogExecuting = (sql, para) =>
            {
                Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
            };
        });
        return db;
    }
}