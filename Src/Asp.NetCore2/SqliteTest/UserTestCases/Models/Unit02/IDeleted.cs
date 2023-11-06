using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTest
{

    public interface IDeleted
    {
        /// <summary>
        /// 默认假删除
        /// </summary>
        //[FakeDelete(true)]  // 设置假删除的值
        bool IsDeleted { get; set; }

        /// <summary>
        /// 删除用户ID
        /// </summary>
        long? DeletedUserId { get; set; }

        /// <summary>
        /// 删除用户名称
        /// </summary>
        string DeletedUserName { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTimeOffset? DeletedTime { get; set; }
    }
}