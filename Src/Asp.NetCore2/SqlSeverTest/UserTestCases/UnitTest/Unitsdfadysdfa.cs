using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitsdfadysdfa
    {
        public static void Init() 
        {
            var myService = new MyService();

            myService.Query();
        }

        public partial class MyService
        {

            protected user_info UserInfo { get { return new user_info { user_id = "a" }; } }


            public void Query()
            {
                Expression<Func<address_info, bool>> EWhere = c => c.area_id == UserInfo.user_name;  //user_name 有问题

                SqlSugarClient dbClient = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = "PORT=5432;DATABASE=test;HOST=127.0.0.1;PASSWORD=123456;USER id=postgres;CommandTimeout=300",
                    DbType = SqlSugar.DbType.PostgreSQL,
                    IsAutoCloseConnection = true,
                    InitKeyType = InitKeyType.Attribute
                });
                  
                var resData = dbClient.Queryable<address_info>()
                                      .Where(EWhere) 
                                      .ToSql();
            } 

        }
        public class user_info
        {
            public string user_id { get; set; }
            public string user_name { get; set; }
        }
        /// <summary>
        ///address_info
        /// </summary>		
        [Serializable]
        public partial class address_info
        {
            #region 属性
            /// <summary>
            /// 
            /// </summary>

            [SugarColumn(IsPrimaryKey = true)]

            public long id { get; set; }
            /// <summary>
            /// 
            /// </summary>

            public string addr { get; set; }

            public string user_id { get; set; }
            /// <summary>
            /// 
            /// </summary>

            public string user_name { get; set; }

            public string area_id { get; set; }
            #endregion


        }
    }
}
