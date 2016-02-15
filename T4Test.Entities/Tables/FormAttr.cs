using System;
namespace T4Test.Entities.Tables
{
    public class FormAttr{
                        
     /// <summary>
     /// 说明:ID 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:字段标题 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Title {get;set;}

     /// <summary>
     /// 说明:字段英文名称 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Name {get;set;}

     /// <summary>
     /// 说明:文本,日期,数字,多行文本 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string AttrType {get;set;}

     /// <summary>
     /// 说明:校验脚本 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string CheckJS {get;set;}

     /// <summary>
     /// 说明:所属类别 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string TypeId {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public DateTime? CreateTime {get;set;}

     /// <summary>
     /// 说明:下拉框的值 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string OptionList {get;set;}

     /// <summary>
     /// 说明:是否必填 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public Boolean? IsValid {get;set;}

   }
            
}