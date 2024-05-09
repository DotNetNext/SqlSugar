using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace OrmTest.entity
{
    ///<summary>
    ///主题
    ///</summary>
    //[Tenant(nameof(enumTenant.app))]
    [SugarTable("app_topic")]
    public partial class app_topic
    {
           public app_topic(){

            this.category_id =Convert.ToInt64("0");

           }
           /// <summary>
           /// Desc:编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long topic_id {get;set;}

           /// <summary>
           /// Desc:分类编号
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public long? category_id {get;set;}

           /// <summary>
           /// Desc:主题
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string title {get;set;}

           /// <summary>
           /// Desc:版本号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
           public Guid? ver {get;set;}

    }
}
