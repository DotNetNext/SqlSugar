using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class QueryJsonArray4
    {
        public static void Init()
        {

            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<UserEntity, RolesEntity>();
            db.Insertable<UserEntity>(new UserEntity { Id = 111111111111111111, AccountName = "测试", RoleIds = new List<long> { 111111111111111111 }, RoleId = 111111111111111111 }).ExecuteCommand();
            db.Insertable(new RolesEntity { Id = 111111111111111111, Name = "测试" }).ExecuteCommand();


            var query6 = db.Queryable<UserEntity>()
                 .LeftJoin<RolesEntity>((o, cus) => o.RoleId == cus.Id)//多个条件用&&
                 .Select((o, cus) => new { o, cus })
                 .ToList();

            var a1 = db.Queryable<UserEntity>().ToList();
            var query5 = db.Queryable<UserEntity>()
                 .LeftJoin<RolesEntity>((o, cus) => o.RoleIds.Contains(cus.Id))//多个条件用&&
                 .Select((o, cus) => new { o.AccountName, cus.Name })
                 .ToList();

            Console.WriteLine("测试完成！");
        }



        /// <summary>
        /// 用户实体
        /// </summary>
        [SugarTable("UserComponent")]
        public class UserEntity
        {
            /// <summary>
            /// 主键Id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnName = "_id")]
            public long Id { get; set; }

            /// <summary>
            /// 账户名。
            /// </summary>
            public string? AccountName { get; set; } = null!;

            /// <summary>
            /// 角色ID列表
            /// </summary>
            public long RoleId { get; set; }

            /// <summary>
            /// 角色ID列表
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<long> RoleIds { get; set; }
        }
        [SugarTable("RoleComponent")]
        public class RolesEntity
        {
            /// <summary>
            /// 主键Id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnName = "_id")]
            public long Id { get; set; }
            public string? Name { get; set; }
        }
    }
}