using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.entityMapDTO
{
    public class app_topic : entity.app_topic
    {
        /// <summary>
        /// 分类名称
        /// <para>映射至：<seealso cref="entity.app_category.category_name"/></para>
        /// </summary>
        public string fk_category_name { get; set; }

        /// <summary>
        /// 评论数量
        /// <para>映射至：<seealso cref="entity.app_comment.comment_id"/></para>
        /// </summary>
        public Int64 fk_comment_total { get; set; } = 0;

        /// <summary>
        /// 版本数量
        /// <para>映射至：<seealso cref="entity.app_revision.revision_id"/></para>
        /// </summary>
        public Int64 fk_revision_total { get; set; } = 0;
        public bool IsAny { get;   set; }
    }
}
