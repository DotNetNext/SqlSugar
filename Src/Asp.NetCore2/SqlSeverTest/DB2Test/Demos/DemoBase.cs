﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class DemoBase
    {
        public  static SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() {
                ConnectionString = Config.ConnectionString, DbType = DbType.DB2, IsAutoCloseConnection = true
            });
            db.Ado.IsEnableLogEvent = true;
            db.Ado.LogEventStarting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it=>it.ParameterName,it=>it.Value)));
                Console.WriteLine();
            };
            return db;
        }
    }
}
