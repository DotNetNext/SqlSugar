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
    public class _1_CodeFirst
    {
      
        public static void Init()
        {
            // Get a new database instance
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            // Create the database if it doesn't exist
            // 如果数据库不存在，则创建数据库
            db.DbMaintenance.CreateDatabase();

            //Support bulkcopy
            //支持大数据操作
            db.Ado.ExecuteCommand("SET GLOBAL local_infile=1");

            // Initialize tables based on UserInfo001 entity class
            // 根据 UserInfo001 实体类初始化表
            db.CodeFirst.InitTables<UserInfo001>();

            //Table structure and class are different
            //表结构和类存在差异 初始化表
            db.CodeFirst.InitTables<UserInfo002>();

            //Insert
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
            var userInfo=db.Queryable<UserInfo001>().InSingle(id);

            //Update 
            //更新
            db.Updateable(userInfo).ExecuteCommand(); 

            //Delete
            //删除
            db.Deleteable(userInfo).ExecuteCommand();
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
            [SugarColumn(IsIdentity = true,ColumnName ="Id", IsPrimaryKey = true)]
            public int UserId { get; set; }

            /// <summary>
            /// User name
            /// 用户名
            /// </summary>
            [SugarColumn(Length = 50,ColumnName ="Name", IsNullable = false)]
            public string UserName { get; set; }

           
        }
    }
}