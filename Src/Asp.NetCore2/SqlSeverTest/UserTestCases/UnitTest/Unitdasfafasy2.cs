using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Class for demonstrating CodeFirst initialization operations
    /// 用于展示 CodeFirst 初始化操作的类
    /// </summary>
    public class Unitetdgsatsdfaffa
    {

        public static void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                IsAutoCloseConnection = true,
                DbType = DbType.SqlServer,
                ConnectionString =DbHelper.Connection,
                SlaveConnectionConfigs = new System.Collections.Generic.List<SlaveConnectionConfig>()
                {
                    new SlaveConnectionConfig(){ HitRate=1, ConnectionString= "aa"}
                },
                LanguageType = LanguageType.Default//Set language

            },
           it => {
               // Logging SQL statements and parameters before execution
               // 在执行前记录 SQL 语句和参数
               it.Aop.OnLogExecuting = (sql, para) =>
               {
                   Console.WriteLine(UtilMethods.GetNativeSql(sql, para));
               };
           });


            //Query
            //查询
            var userInfo = db.MasterQueryable<UserInfo001>().AsWithAttr()
                .LeftJoin<UserInfo001>((x, y) => true).Count();


        }

        /// <summary>
        /// User information entity class
        /// 用户信息实体类
        /// </summary> 
        public class UserInfo001
        {
            /// <summary>
            /// User ID (Primary Key)
            /// 用户ID（主键）
            /// </summary>
            [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
            public int UserId { get; set; }

            /// <summary>
            /// User name
            /// 用户名
            /// </summary>
            [SugarColumn(Length = 50, IsNullable = false)]
            public string UserName { get; set; }

            /// <summary>
            /// User email
            /// 用户邮箱
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public string Email { get; set; }


            /// <summary>
            /// Product price
            /// 产品价格
            /// </summary> 
            public decimal Price { get; set; }

            /// <summary>
            /// User context
            /// 用户内容
            /// </summary>
            [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
            public string Context { get; set; }

            /// <summary>
            /// User registration date
            /// 用户注册日期
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public DateTime? RegistrationDate { get; set; }
        }


        /// <summary>
        /// User information entity class
        /// 用户信息实体类
        /// </summary> 
        [SugarTable("UserInfoAAA01")]
        public class UserInfo002
        {
            /// <summary>
            /// User ID (Primary Key)
            /// 用户ID（主键）
            /// </summary>
            [SugarColumn(IsIdentity = true, ColumnName = "Id", IsPrimaryKey = true)]
            public int UserId { get; set; }

            /// <summary>
            /// User name
            /// 用户名
            /// </summary>
            [SugarColumn(Length = 50, ColumnName = "Name", IsNullable = false)]
            public string UserName { get; set; }


        }
    }
}