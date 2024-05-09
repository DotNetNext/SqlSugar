using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void UConfig()
        {
            SqlSugarScope db = new SqlSugarScope(new ConnectionConfig()
            {
                ConfigId="1",
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (s, p) =>
                    {
                        Console.WriteLine(s);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                },
                MoreSettings=new ConnMoreSettings() { 
                  IsWithNoLockQuery=true
                }
            });


            var sql=db.Queryable<Order>().ToSql();
            if (!sql.Key.Contains("WITH(NOLOCK)")) throw new Exception("unit config error");

            db.CurrentConnectionConfig.MoreSettings = null;

            sql = db.Queryable<Order>().ToSql();
            if (sql.Key.Contains("WITH(NOLOCK)")) throw new Exception("unit config error");
            if (db.CurrentConnectionConfig.ConfigId!="1") throw new Exception("unit config error");

            db.CurrentConnectionConfig.IsAutoCloseConnection = false;

            Task.Run(() =>
            {
                if (db.CurrentConnectionConfig.IsAutoCloseConnection==false) throw new Exception("unit config error");
            });
            System.Threading.Thread.Sleep(1000);
            if (db.CurrentConnectionConfig.IsAutoCloseConnection == true) throw new Exception("unit config error");

            if (!db.IsAnyConnection("0")) 
            {
                db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = "0",
                    DbType = DbType.SqlServer,
                    ConnectionString = Config.ConnectionString,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true,
                    AopEvents = new AopEvents
                    {
                        OnLogExecuting = (s, p) =>
                        {
                            Console.WriteLine(s);
                            Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                        }
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsWithNoLockQuery = true
                    }
                });
            }
            var x=db.GetConnection("0");
            if (db.IsAnyConnection("0")==false) throw new Exception("unit config error");
            Task.Run(() =>
            {
                if (db.IsAnyConnection("0"))
                {
                    throw new Exception("unit config error");
                }
                else 
                {
                    db.AddConnection(new ConnectionConfig()
                    {
                        ConfigId = "11",
                        DbType = DbType.SqlServer,
                        ConnectionString = Config.ConnectionString,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        AopEvents = new AopEvents
                        {
                            OnLogExecuting = (s, p) =>
                            {
                                Console.WriteLine(s);
                                Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                            }
                        },
                        MoreSettings = new ConnMoreSettings()
                        {
                            IsWithNoLockQuery = true
                        }
                    });
                }
            });
            System.Threading.Thread.Sleep(1000);
        }
    }


}
