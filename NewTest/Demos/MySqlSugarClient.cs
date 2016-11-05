using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace NewTest.Demos
{
    /// <summary>
    /// 重写SqlSugar底层
    /// </summary>
    public class MySqlSugarClient:SqlSugarClient
    {
        public MySqlSugarClient(string connectionString)
            : base(connectionString)
        { 
        
        }
        public override System.Data.DataSet GetDataSetAll(string sql, object pars)
        {
            return base.GetDataSetAll(sql, pars);
        }

        public override void BeginTran(System.Data.IsolationLevel iso, string transactionName)
        {
            base.BeginTran(iso, transactionName);
        }

        public override int ExecuteCommand(string sql, object pars)
        {
            return base.ExecuteCommand(sql, pars);
        }
    }
}
