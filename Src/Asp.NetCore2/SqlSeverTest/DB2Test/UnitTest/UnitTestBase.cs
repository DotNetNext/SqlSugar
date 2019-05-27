using System;
using System.Collections.Generic;
using SqlSugar;
using System.Linq;
namespace OrmTest.UnitTest
{
    public class UnitTestBase
    {
        public int Count { get; set; }
        private DateTime BeginTime { get; set; }
        private DateTime EndTime { get; set; }

        public void Begin()
        {
            this.BeginTime = DateTime.Now;
        }

        public void End(string title)
        {
            this.EndTime = DateTime.Now;
            Console.WriteLine(title + " \r\nCount: " + this.Count + "\r\nTime:  " + (this.EndTime - this.BeginTime).TotalSeconds + " Seconds \r\n");
        }

        internal void Check(string value, List<SugarParameter> pars, string validValue, List<SugarParameter> validPars, string errorMessage)
        {
            if (value.Trim() != validValue.Trim())
            {
                throw new Exception(errorMessage);
            }
            if (pars != null && pars.Count > 0)
            {
                if (pars.Count != validPars.Count)
                {
                    throw new Exception(errorMessage);
                }
                else
                {
                    foreach (var item in pars)
                    {
                        if (!validPars.Any(it => it.ParameterName.Equals(item.ParameterName) && it.Value.ObjToString().Equals(item.Value.ObjToString())))
                        {
                            throw new Exception(errorMessage);
                        }
                    }
                }
            }
        }

        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Oracle, IsAutoCloseConnection = true });
            return db;
        }
    }
}