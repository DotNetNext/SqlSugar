using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarDemo
{
	/// <summary>
	/// 账号角色表
	/// </summary>
	[SugarTable("Unit_SYS_UserRole")]
	public partial class UserRoleEntity
	{

		/// <summary>
		/// 系统账号Id
		/// </summary>	
		[SugarColumn(IsPrimaryKey = true)]
		public Guid UserId { get; set; }

		/// <summary>
		/// 角色Id
		/// </summary>	
		[SugarColumn(IsPrimaryKey = true)]
		public Guid RoleId { get; set; }

	}


}
