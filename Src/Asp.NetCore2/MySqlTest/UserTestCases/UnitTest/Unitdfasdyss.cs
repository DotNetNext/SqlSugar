using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    using System.Reflection;
    using SqlSugar;

    internal class Unitsdfasfasa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.SlaveConnectionConfigs =
            new List<SlaveConnectionConfig>() {
                    new()
                    {
                        HitRate=10,
                        ConnectionString = NewUnitTest.Db.CurrentConnectionConfig.ConnectionString,
                    }
                };
            var types = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => !type.IsGenericType)
                    .Where(s=>s.Namespace== "WebApplication4")
                    .Where(type => !type.IsInterface)
                    .Where(type => !type.IsAbstract)
                    .Where(type => IntrospectionExtensions.GetTypeInfo(type).IsClass)
                    .Where(type => type.GetCustomAttribute<SqlSugar.SugarTable>() != null)
                    .ToArray();

            #region 问题复现，报错不够清楚，不知道哪个实体有问题
            //var diffString2 = db.CodeFirst.GetDifferenceTables(types).ToDiffString();
            #endregion

            #region 问题复现
            foreach (var type in types)
            {
                try
                {
                    var diffString = db.CodeFirst.GetDifferenceTables(type).ToDiffString();
                    db.CodeFirst.InitTables(type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"type = {type.Name} 错误: {ex}");
                }
            }
            #endregion 
        }
    }
}
