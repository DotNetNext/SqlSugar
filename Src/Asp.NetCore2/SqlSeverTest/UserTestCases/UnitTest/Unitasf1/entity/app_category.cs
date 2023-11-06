using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace OrmTest.entity
{
    ///<summary>
    /// 分类
    ///</summary>
    //[Tenant(nameof(enumTenant.app))]
    [SugarTable("app_category")]
    public partial class app_category
    {
           public app_category(){


           }
           /// <summary>
           /// Desc:分类编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long category_id {get;set;}

           /// <summary>
           /// Desc:上级分类编号
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? category_pid {get;set;}

           /// <summary>
           /// Desc:分类名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string category_name {get;set;}

           /// <summary>
           /// Desc:版本号
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]//标识版本字段
           public Guid? ver {get;set;}

    }
}
