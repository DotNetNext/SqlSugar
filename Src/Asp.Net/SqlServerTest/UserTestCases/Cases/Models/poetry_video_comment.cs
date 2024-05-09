using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace SqlSugarDemo
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("poetry_video_comment")]
    public partial class poetry_video_comment
    {
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(user_id))]
        public user_name_simple UserInfo { set; get; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(reply_user_id))]
        public user_name_simple ReplyUser { set; get; }

        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(first_reply_id))]
        public poetry_video_comment FirstReply { set; get; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int user_id { get; set; }

        public long? first_reply_id { set; get; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int reply_user_id { get; set; }





    }


}
