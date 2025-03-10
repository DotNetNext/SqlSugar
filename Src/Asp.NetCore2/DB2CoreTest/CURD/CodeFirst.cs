using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB2CoreTest.CURD
{
    public class CodeFirst
    {
        public static void Init()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            // 如果数据库不存在，则创建数据库
            //db.DbMaintenance.CreateDatabase();

            //实体类初始化表
            db.CodeFirst.InitTables<UserInfo>();

            //实体类初始化表
            db.CodeFirst.InitTables<UserLogins>();
        }

        public static void Insertable()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //插入用户
            var userId = db.Insertable(new UserInfo()
            {
                Context = "Context",
                Email = "dfafa@qq.com",
                Price = Convert.ToDecimal(1.1),
                UserName = "admin",
                RegistrationDate = DateTime.Now,

            }).ExecuteReturnIdentity();

            //插入用户登录
            var loginId = db.Insertable(new UserLogins()
            {
                LoginTime = DateTime.Now,
                UserId = userId,

            }).ExecuteReturnIdentity();
        }

        public static void Queryable()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //查询用户
            var userInfo = db.Queryable<UserInfo>().ToList();

            //查询用户登录
            var userLogin = db.Queryable<UserLogins>().ToList();
        }

        public static void JoinQuery()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            var queryResult = db.Queryable<UserInfo, UserLogins>((ui, ul) => new JoinQueryInfos(JoinType.Inner, ui.UserId == ul.UserId)).Select((ui, ul) => new
            {
                ui.UserName,
                ul.LoginTime,
            }).ToList();
        }

        public static void PageQuery()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //查询用户
            var userList = db.Queryable<UserInfo>().ToPageList(2, 2);

            var queryResult = db.Queryable<UserInfo, UserLogins>((ui, ul) => new JoinQueryInfos(JoinType.Inner, ui.UserId == ul.UserId)).Select((ui, ul) => new
            {
                ui.UserName,
                ul.LoginTime,
            }).ToPageList(2, 2);
        }

        public static void OrderBy()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //查询用户
            var userList = db.Queryable<UserInfo>().OrderBy(p => p.UserName).ToList();

            var queryResult = db.Queryable<UserInfo, UserLogins>((ui, ul) => new JoinQueryInfos(JoinType.Inner, ui.UserId == ul.UserId)).Select((ui, ul) => new
            {
                ui.UserName,
                ul.LoginTime,
            }).OrderBy(ui => ui.UserName).ToList();
        }

        public static void GroupBy()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //查询用户
            var userList = db.Queryable<UserInfo>().GroupBy(p => p.Email).Select(p => new { UserId = SqlFunc.AggregateMax(p.UserId), p.Email, }).OrderBy(p => p.UserId).ToList();

            var queryResult = db.Queryable<UserInfo, UserLogins>((ui, ul) => new JoinQueryInfos(JoinType.Inner, ui.UserId == ul.UserId)).GroupBy((ui, ul) =>
            new
            {
                ui.UserName,
            })
            .Select((ui, ul) => new
            {
                UserId = SqlFunc.AggregateMax(ui.UserId),
                ui.UserName,
                LoginTime = SqlFunc.AggregateMax(ul.LoginTime),
            }).OrderBy(ui => ui.UserName).ToList();
        }

        public static void Deleteable()
        {
            // 获取新的数据库实例
            var db = DbHelper.GetNewDb();

            //查询用户
            var userList = db.Queryable<UserInfo>().ToList();
            //删除用户
            var deleteUserRet = db.Deleteable(userList).ExecuteCommand();

            //查询用户登录
            var userLoginList = db.Queryable<UserInfo>().ToList();
            //删除用户登录
            var deleteUserLoginRet = db.Deleteable(userLoginList).ExecuteCommand();
        }
    }

    /// <summary>
    /// 用户信息实体类
    /// </summary> 
    [SugarTable("USER_INFO", TableDescription = "用户信息")]
    public class UserInfo
    {
        /// <summary>
        /// 用户ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true, ColumnDescription = "用户ID")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(Length = 50, IsNullable = false, ColumnDescription = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "用户邮箱")]
        public string Email { get; set; }

        /// <summary>
        /// 产品价格
        /// </summary> 
        [SugarColumn(ColumnDescription = "产品价格")]
        public decimal Price { get; set; }

        /// <summary>
        /// 用户内容
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true, ColumnDescription = "用户内容")]
        public string Context { get; set; }

        /// <summary>
        /// 用户注册日期
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "用户注册日期")]
        public DateTime? RegistrationDate { get; set; }
    }

    /// <summary>
    /// 用户登录实体类
    /// </summary> 
    [SugarTable("USER_LOGINS")]
    public class UserLogins
    {
        /// <summary>
        /// ID（主键）
        /// </summary>
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int UserId { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime LoginTime { get; set; }
    }
}
