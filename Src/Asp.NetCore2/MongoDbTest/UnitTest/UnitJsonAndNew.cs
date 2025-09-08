using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar; 
namespace MongoDbTest 
{
 
    public class UnitJsonAndNew

    {

        public static void Init()

        {

            { 
                var db = DbHelper.GetNewDb();
                db.DbMaintenance.TruncateTable<UserEntity, RolesEntity>();

                db.Insertable<UserEntity>(new UserEntity { Id = 1, AccountName = "A", RoleIds = new List<long> { 1 }, RoleId = 1 }).ExecuteCommand();
                db.Insertable<UserEntity>(new UserEntity { Id = 2, AccountName = "B", RoleIds = new List<long> { 3 }, RoleId = 1 }).ExecuteCommand();
                db.Insertable(new RolesEntity { Id = 1, Name = "存在" }).ExecuteCommand();
                db.Insertable(new RolesEntity { Id = 2, Name = "不存在" }).ExecuteCommand();

                var query1 = db.Queryable<UserEntity>()

                             .InnerJoin<RolesEntity>((u, role) => u.RoleIds.Contains(role.Id))//多个条件用&&

                             .Select((u, role) => new { u.AccountName, role.Name })

                             .ToList();

                if (query1.Count != 1) Cases.ThrowUnitError(); 
                if (query1.First().Name != "存在") Cases.ThrowUnitError();
                var query2 = db.Queryable<UserEntity>()

                             .LeftJoin<RolesEntity>((u, role) => u.RoleIds.Contains(role.Id))//多个条件用&&

                             .Select((u, role) => new { u.AccountName, role.Name })

                             .ToList();
                if (query2.Count != 2) Cases.ThrowUnitError();
                if (query2.First().Name!= "存在") Cases.ThrowUnitError();
                if (query2.Last().Name != null) Cases.ThrowUnitError();
            }

        }

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
