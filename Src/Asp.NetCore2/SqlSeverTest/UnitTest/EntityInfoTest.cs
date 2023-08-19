using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace OrmTest
{
    internal class EntityInfoTest
    {
        public static void Init()
        {
            for (int i = 0; i < 100; i++)
            {
                var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
             //这儿声名所有上下文都生效
             new ConnectionConfig(){ConfigId="0",
             ConfigureExternalServices=new ConfigureExternalServices(){
              EntityService=(x,y)=>{ y.IsPrimarykey=true; }
             },
                 DbType=DbType.SqlServer,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true},
             new ConnectionConfig(){ConfigId="1",
               ConfigureExternalServices=new ConfigureExternalServices(){
              EntityService=(x,y)=>{ y.IsIdentity=true; }
             },DbType=DbType.SqlServer,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true  }
            });

                if (db.GetConnection("0").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != false
                    || db.GetConnection("0").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != true)
                {
                    throw new Exception("unit error");
                }

                if (db.GetConnection("1").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != true
               || db.GetConnection("1").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != false)
                {
                    throw new Exception("unit error");
                }

                if (db.EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != false
                   || db.EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != true)
                {
                    throw new Exception("unit error");
                } 
            }
        }
    }
    public class classTest 
    {
        public string Id { get; set; }
    }
}
