using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///品牌
    ///</summary>
    [SugarTable("Brand")]
    public partial class Brand
    {
           public Brand(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string Name {get;set;}

           /// <summary>
           /// Desc:品牌别名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AliasName {get;set;}

           /// <summary>
           /// Desc:顺序
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? Sequence {get;set;}

           /// <summary>
           /// Desc:网址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Url {get;set;}

           /// <summary>
           /// Desc:Logo图片路径
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string LogoPath {get;set;}

           /// <summary>
           /// Desc:模板类型(0:默认)
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? Templet {get;set;}

           /// <summary>
           /// Desc:页面标题
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PageTitle {get;set;}

           /// <summary>
           /// Desc:页面关键字
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PageKeywords {get;set;}

           /// <summary>
           /// Desc:PageDesc页面描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string PageDesc {get;set;}

           /// <summary>
           /// Desc:详细说明
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DetailedDesc {get;set;}

           /// <summary>
           /// Desc:状态（0正常，1暂停，3删除等）
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public short? Status {get;set;}

           /// <summary>
           /// Desc:
           /// Default:DateTime.Now
           /// Nullable:False
           /// </summary>           
           public DateTime CreateTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CreateUser {get;set;}

           /// <summary>
           /// Desc:ModifyTime
           /// Default:DateTime.Now
           /// Nullable:True
           /// </summary>           
           public DateTime? ModifyTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ModifyUser {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? BrandType {get;set;}

    }
}
