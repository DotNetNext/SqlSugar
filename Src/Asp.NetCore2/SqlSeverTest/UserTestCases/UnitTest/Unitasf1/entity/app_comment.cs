using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace OrmTest.entity
{
    ///<summary>
    ///评论
    ///</summary>
    //[Tenant(nameof(enumTenant.app))]
    [SugarTable("app_comment")]
    public partial class app_comment
    {
           public app_comment(){

            this.topic_id =Convert.ToInt64("0");

           }
           /// <summary>
           /// Desc:评论编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long comment_id {get;set;}

           /// <summary>
           /// Desc:主题编号
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public long? topic_id {get;set;}

           /// <summary>
           /// Desc:评论内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string content {get;set;}

           /// <summary>
           /// Desc:版本号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
           public Guid? ver {get;set;}

    }
}
