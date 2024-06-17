using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    internal class UnitManyToMany121231
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<OperatorInfo, Role, OptRole>();
            db.DbMaintenance.TruncateTable<OperatorInfo, Role, OptRole>();

            var id = db.Insertable(new Role()
            {
                id = 1,
                createTime = DateTime.Now,
                name = "admin"

            }).ExecuteReturnIdentity();
            var id2 = db.Insertable(new Role()
            {
                id = 2,
                createTime = DateTime.Now,
                name = "admin"

            }).ExecuteReturnIdentity();
            db.InsertNav(new OperatorInfo()
            {
                id = "1",
                createTime = DateTime.Now,
                isDel = 1,
                isDisabled = 1,
                openid = "",
                phone = "",
                pwd = "",
                realname = "a01",
                remark = "a",
                sno = "a",
                username = "a01",
                Roles = new List<Role>() {
                   new Role() { id = 2 },
                    new Role() { id = 1 } }
            }).Include(it => it.Roles).ExecuteCommand();

            var list1 = db.Queryable<OperatorInfo>()
                .Includes(it => it.Roles)
                .ToList();

            var list2 = db.Queryable<OperatorInfo>()
                       .Includes(it => it.Roles.Select(it => new Role() {
                           name = it.name, 

                       }).ToList())
                       .Select(it => new {
                           id=it.id,
                           roles = it.Roles.Select(it => new   { name = it.name }).ToList()
                       })
                       .ToList();

            if (list2.First().roles.Count == 0) 
            {
                throw new Exception("unit error");
            } 
            TestLength2(db);
            TestLength3(db);
            TestLength1(db);
        }

        private static void TestLength1(SqlSugarClient db)
        {
            var par = "aa";
            db.Queryable<Role>()
                .Where(it => it.name.ToString().Substring(1, par.Length) == "")
                .ToList();
            var sql1 = db.Queryable<Role>()
               .Where(it => it.name.ToString().Substring(1, par.Length) == "")
               .ToSqlString();

            if (sql1.Contains("LEN(LEN"))
            {
                throw new Exception("error;");
            }
        }
        private static void TestLength2(SqlSugarClient db)
        {
            var par = "aa";
            db.Queryable<Role>()
                .Where(it => it.name.ToString().Substring(1, "aa".Length) == "")
                .ToList();
            var sql1 = db.Queryable<Role>()
               .Where(it => it.name.ToString().Substring(1, "aa".Length) == "")
               .ToSqlString();

            if (sql1.Contains("LEN(LEN"))
            {
                throw new Exception("error;");
            }
        }
        private static void TestLength3(SqlSugarClient db)
        { 
            db.Queryable<Role>()
                .Where(it => it.name.ToString().Substring(1, it.name.Length) == "")
                .ToList();
            var sql1 = db.Queryable<Role>()
               .Where(it => it.name.ToString().Substring(1, it.name.Length) == "")
               .ToSqlString();

            if (sql1.Contains("LEN(LEN"))
            {
                throw new Exception("error;");
            }
        }
        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_operatorinfo3313")]
        public partial class OperatorInfo
        {           /// <summary>
                    /// 多角色
                    /// </summary>
            [Navigate(typeof(OptRole), nameof(OptRole.operId), nameof(OptRole.roleId))]//名字换
            public List<Role> Roles { get; set; }
            /// <summary>
            /// 主键
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string id { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            public string realname { get; set; }

            /// <summary>
            /// 账号
            /// </summary>
            public string username { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string pwd { get; set; }

            /// <summary>
            /// 学号
            /// </summary>
            public string sno { get; set; }

            /// <summary>
            /// openid
            /// </summary>
            public string openid { get; set; }

            /// <summary>
            /// 手机号码
            /// </summary>
            public string phone { get; set; }

            /// <summary>
            /// 备注信息
            /// </summary>
            public string remark { get; set; }

            /// <summary>
            /// 创建日期
            /// </summary>
            public DateTime createTime { get; set; }

            /// <summary>
            /// 状态（1：启用，2：禁用）
            /// </summary>
            public int isDisabled { get; set; }

            /// <summary>
            /// 是否删除（1：正常；2：删除）
            /// </summary>
            public int isDel { get; set; }

        }

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_role13313")]
        public partial class Role
        {
            /// <summary>
            /// 角色
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public long id { get; set; }

            /// <summary>
            /// 角色名称
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime createTime { get; set; }


        }

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-21 14:35:09
        /// </summary>
        [SugarTable("unit_operator_role3313")]
        public partial class OptRole
        {
            /// <summary>
            /// 
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public long id { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string operId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int roleId { get; set; }


        }
    }
}