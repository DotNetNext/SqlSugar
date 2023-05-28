using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OrmTest.entityMap
{
    public class app_topic : entity.app_topic
    {
        /// <summary>
        /// 导航表：分类
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(category_id))]
        public entity.app_category map_app_category { get; set; }

        /// <summary>
        /// 导航表：评论
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(entity.app_topic.topic_id), nameof(entity.app_comment.topic_id))]
        public List<entity.app_comment> map_app_comment { get; set; }

        /// <summary>
        /// 导航表：版本
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(entity.app_topic.topic_id), nameof(entity.app_revision.topic_id))]
        public List<entity.app_revision> map_app_revision { get; set; }
    }
}
