using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class Aop
    {

        public static void Init()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.SqlServer, IsAutoCloseConnection = true });
            db.Aop.OnLogExecuted = (sql, pars) =>
            {

            };
            db.Aop.OnLogExecuting = (sql, pars) =>
            {

            };
            db.Aop.OnError = (exp) =>
            {
                
            };
            db.Aop.OnExecutingChangeSql = (sql, pars) =>
            {
                return new KeyValuePair<string, SugarParameter[]>(sql,pars);
            };

            db.Queryable<CMStudent>().ToList();

            try
            {
                db.Queryable<CMStudent>().AS(" ' ").ToList();
            }
            catch (Exception)
            {

                
            }
        }

}
}