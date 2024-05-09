using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace OrmTest.entity
{
    ///<summary>
    ///修订记录
    ///</summary>
    //[Tenant(nameof(enumTenant.app))]
    [SugarTable("app_revision")]
    public partial class app_revision
    {
           public app_revision(){

            this.topic_id =Convert.ToInt64("0");

           }
           /// <summary>
           /// Desc:修订编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long revision_id {get;set;}

           /// <summary>
           /// Desc:主题编号
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public long? topic_id {get;set;}

           /// <summary>
           /// Desc:版本号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
           public Guid? ver {get;set;}

    }
}
