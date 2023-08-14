using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    internal class UnitManyToMany
	{
		public static void Init()
		{
			var db = NewUnitTest.Db;
			db.CodeFirst.InitTables<OperatorInfo, Role, OptRole>();
			db.DbMaintenance.TruncateTable<OperatorInfo, Role, OptRole>();
			db.Insertable(new OperatorInfo()
			{
				 id="1",
				  createTime=DateTime.Now,	
				   isDel=1,
				    isDisabled=1,
					 openid="",
					  phone="",
					   pwd="",
					    realname="a01",
						 remark="a",
						   sno="a",
						    username="a01"
			}).ExecuteCommand();
			db.Insertable(new OperatorInfo()
			{
				id = "2",
				createTime = DateTime.Now,
				isDel = 1,
				isDisabled = 1,
				openid = "",
				phone = "",
				pwd = "",
				realname = "a01",
				remark = "a",
				sno = "a",
				username = "admin"
			}).ExecuteCommand();
			db.Insertable(new OperatorInfo()
			{
				id = "3",
				createTime = DateTime.Now,
				isDel = 1,
				isDisabled = 1,
				openid = "",
				phone = "",
				pwd = "",
				realname = "a01",
				remark = "a",
				sno = "a",
				username = "admin"
			}).ExecuteCommand();
			var id=db.Insertable(new Role()
			{
				 id=1,
				  createTime=DateTime.Now,
				   name="admin"

			}).ExecuteReturnIdentity();
			var id2 = db.Insertable(new Role()
			{
				id = 2,
				createTime = DateTime.Now,
				name = "admin"

			}).ExecuteReturnIdentity();
			db.Insertable(new OptRole() { operId="1", roleId=id }).ExecuteCommand();
			db.Insertable(new OptRole() { id=2, operId = "2", roleId = id2 }).ExecuteCommand();
			db.Queryable<OperatorInfo>()
				.Includes(x => x.Roles).Where(x => x.Roles.Any(z=>z.id==1))
				.ToList();
			var list = db.Queryable<OperatorInfo>()
				.Includes(x => x.Roles).Where(x=>x.Roles.Any()).ToList();
            if (list.Count != 2) 
			{
				throw new Exception("unit error");
			}
			var list3 = db.Queryable<OperatorInfo>()
			.Includes(x => x.Roles).Where(x => x.Roles.Count()==0).ToList();
			if (list3.Count != 1)
			{
				throw new Exception("unit error");
			}
			var list2=db.Queryable<OperatorInfo>()
                .Includes(x => x.Roles.MappingField(z=>z.name,()=>x.username).ToList())
                .ToList();

			var list4 = db.Queryable<OperatorInfo>()
			.Includes(x => x.Roles.Skip(10).Take(1).ToList())
			.ToList();

            db.QueryFilter.AddTableFilter<OptRole>(x => x.roleId == 2);
            db.Queryable<OperatorInfo>()
                .Includes(x => x.Roles)
                .Where(x => x.Roles.Any() || x.Roles.Any())
                .ToList();

			db.QueryFilter.Clear();
			List<int> tags = new List<int>() {1, 2 };
            var list5=db.Queryable<OperatorInfo>()
                .Includes(x => x.Roles)
                .Where(x => x.Roles.Any(s=> tags.Contains(s.id)))
                .ToList();

            var list6 = db.Queryable<OperatorInfo>()
              .Includes(x => x.Roles)
              .LeftJoin<OperatorInfo>((x, y) => x.id == y.id)
              .Where(x => x.Roles.Any(s => tags.Contains(s.id)))
              .ToList();

            db.QueryFilter.AddTableFilter<OptRole>(x => x.roleId == 2);
            db.Queryable<OperatorInfo>()
                
                .ToList(it=>new { 
				 list=it.Roles.Count(),
                    list2 = it.Roles.Count(),
                });

        }

        /// <summary>
        /// 描述：
        /// 作者：synjones
        /// 时间：2022-04-20 21:30:28
        /// </summary>
        [SugarTable("unit_operatorinfo")]
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
		[SugarTable("unit_role1")]
		public partial class Role
		{
			/// <summary>
			/// 角色
			/// </summary>
			[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
			public int id { get; set; }

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
		[SugarTable("unit_operator_role")]
		public partial class OptRole
		{
			/// <summary>
			/// 
			/// </summary>
			[SugarColumn(IsPrimaryKey = true)]
			public int id { get; set; }

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
