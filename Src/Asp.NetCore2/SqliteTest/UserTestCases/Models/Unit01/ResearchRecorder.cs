using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    [SugarTable("ResearchRecorder")]
    public class ResearchRecorder : DEntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        //public string Name { get; set; }
        public long TranslaterId { get; set; }
        /// <summary>
        /// 关联ProjectDetailInfomation表，对应这个调研属于哪个项目哪个语言对下面
        /// 因为某一个译员如果会多个语言对，同一个项目是可以做不同的任务的
        /// </summary>
        public long ProjectDetailInfomationId { get; set; }


        #region 翻译员回复

        /// <summary>
        /// 回复结果，如果为空表示未回复
        /// </summary>
        public RessearchReplyType ReplyReseult { get; set; }

        #endregion


        /// <summary>
        /// 该调研是针对哪一个项目详细信息的
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(ResearchRecorder.ProjectDetailInfomationId))]
        public ProjectDetailInfomation ProjectDetailInfo { get; set; }
    }
}
