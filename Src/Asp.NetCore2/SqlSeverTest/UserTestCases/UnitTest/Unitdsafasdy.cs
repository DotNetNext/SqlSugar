using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{ 
    internal class Unitdsfasdfys
    {
        public static  void Init()
        {
            ConnectionConfig config = new ConnectionConfig();
            config.ConnectionString =DbHelper.Connection;
            config.ConfigId = "0";
            config.IsAutoCloseConnection = true;
            config.DbType = SqlSugar.DbType.SqlServer;
            config.MoreSettings = new ConnMoreSettings()
            {
                SqlServerCodeFirstNvarchar = true
            };
            config.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (c, p) =>
                {
                    if (c.PropertyType == typeof(object) && p.DataType == "sql_variant")
                    {
                        p.SqlParameterDbType = SqlDbType.Variant;
                    }
                    if (p.IsPrimarykey == false && c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))//自动可为空
                    {
                        p.IsNullable = true;
                    }
                    if (p.PropertyName.ToLower() == "id" && p.IsPrimarykey)//默认Id这个为主键
                    {
                        p.IsPrimarykey = true;
                        if (p.PropertyInfo.PropertyType == typeof(int))
                        {
                            p.IsIdentity = true;//是id并且是int的是自增
                        }
                    }
                },
                EntityNameService = (type, entity) =>
                {
                    //entity.DbTableName 修改表名
                }
            };
            SqlSugarScope sqlSugar = new SqlSugarScope(new List<ConnectionConfig>() { config },
            db =>
            {
                db.GetConnectionScope("0").Aop.OnError = (exp) =>
                {
                    var sql = exp.Sql;
                    var parameters = exp.Parametres;
                    var str = $"0--SqlSugar异常 ：{exp}";
                };
                db.GetConnectionScope("0").Aop.OnLogExecuting = (sql, pars) =>
                {
                    string msg = $"0--SqlSugar 执行了Sql语句：{sql}";
                };
            });
            sqlSugar.GetConnectionScope("0").DbMaintenance.CreateDatabase();
            var Db = sqlSugar.AsTenant().GetConnectionScope("0");
            Db.CodeFirst.InitTables(new Type[] { typeof(TestDateTime) });
            Db.DbMaintenance.TruncateTable<TestDateTime>();
            TestDateTime testDateTime = new TestDateTime()
            {
                CreateTime = DateTime.Parse("2025-03-29 09:27:37.9991749")
            };
            Db.Insertable(testDateTime).ExecuteCommand();

            var data = Db.Queryable<TestDateTime>().First().CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            if (data != "2025-03-29 09:27:37.9991749") throw new Exception("unit error");
        }
    }
    public class TestDateTime
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn(SqlParameterDbType=System.Data.DbType.DateTime2 , ColumnDataType = "datetime2(7)")]
        public DateTime CreateTime { get; set; }
    }
}
