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
    public class Unitsdfaafa
    {
      
        public static void Init()
        {
            // Get a new database instance
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            // Create the database if it doesn't exist
            // 如果数据库不存在，则创建数据库
            db.DbMaintenance.CreateDatabase();

            // Initialize tables based on UserInfo001 entity class
            // 根据 UserInfo001 实体类初始化表
            db.CodeFirst.InitTables<UserInfo001>();

            db.DbMaintenance.TruncateTable<UserInfo001>();
            //插入
            var id=db.Insertable(new UserInfo001()
            {
                Context = "Context",
                Email="dfafa@qq.com",
                Price=Convert.ToDecimal(1.1),
                UserName="admin",
                RegistrationDate=DateTime.Now,

            }).ExecuteReturnIdentity();

            //Query
            //查询
            var userInfo = db.Queryable<object>()
                .AsType(typeof(UserInfo001))
                .Select<IUserInfo001>().ToList();

            var userInfo2 = db.Queryable<object>()
               .AsType(typeof(UserInfo001))
               .Select<IUserInfo001>().ToListAsync().GetAwaiter().GetResult();


            var userInfo3 = db.Queryable<UserInfo001>() 
              .Select<IUserInfo001>().ToList();

            var userInfo4 = db.Queryable<UserInfo001>()
            .Select<IUserInfo001>().ToSqlString();

            var userInfo5 = db.Queryable<UserInfo001>()
            .OfType<IUserInfo001>().ToList();

        }
        public interface IUserInfo001
        {
            string Context { get; set; }
            string Email { get; set; }
            decimal Price { get; set; }
            DateTime? RegistrationDate { get; set; }
            int UserId { get; set; }
            string UserName { get; set; }
        }
        /// <summary>
        /// User information entity class
        /// 用户信息实体类
        /// </summary> 
        [SugarTable("UnitUserInfo001")]
        public class UserInfo001 : IUserInfo001
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


    }
}