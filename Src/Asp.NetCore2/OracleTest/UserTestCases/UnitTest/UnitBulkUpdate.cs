using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OrmTest;

internal class UnitBulkUpdate
{
    public static void Init()
    {
        ConnectionConfig config = new ConnectionConfig();
        config.ConfigId = 3;
        config.ConnectionString = Config.ConnectionString;
        config.DbType = DbType.Oracle;
        var db = new SqlSugarScope(config);
        try
        {
            db.Aop.OnLogExecuting = (sql, p) =>
            {
                string key = "【SQL参数】：" + "\n";

                foreach (var param in p)

                {
                    key += $"{param.ParameterName}:{param.Value}\n";
                }

                Console.WriteLine(string.Join("\r\n", new string[] { "--------", "【SQL语句】：" + sql, key }));
            };
            //建表 
            if (!db.DbMaintenance.IsAnyTable("TempStudent", false))
            {
                db.CodeFirst.InitTables<TempStudent>();
            }

            db.Deleteable<TempStudent>().ExecuteCommand();
            var list = new List<TempStudent>();
            for (int i = 0; i < 300; i++)
            {
                var info = new TempStudent()
                {
                    guid = Guid.NewGuid().ToString().ToUpper(),
                    num = i,
                    name = "北京第" + i + "小学"
                };
                list.Add(info);
            }
            var newlist = new List<TempStudent>();
            foreach (var item in list)
            {
                var info = new TempStudent();
                info.guid = item.guid;
                info.num = item.num;
                info.name = null;
                newlist.Add(info);
            }

            db.Insertable(list).ExecuteCommand();

            db.Fastest<TempStudent>().BulkUpdate(newlist, new string[] { "GUID" }, new string[] { "NUM" });

            db.DbMaintenance.TruncateTable<TempStudent>();
        }
        catch (Exception ex)
        {
        }
    }
}

///<summary>
///消毒
///</summary>
[SugarTable("TempStudent")]
public class TempStudent
{
    public TempStudent()
    {
    }
    [SugarColumn(IsPrimaryKey = true)]
    public string guid { get; set; }

    public string name { get; set; }

    public int? num { get; set; }
}