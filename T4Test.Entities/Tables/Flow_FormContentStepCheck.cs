using System;
namespace T4Test.Entities.Tables
{
    public class Flow_FormContentStepCheck{
                        
     /// <summary>
     /// 说明:ID 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:所属公文 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string ContentId {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string StepId {get;set;}

     /// <summary>
     /// 说明:0不通过1通过2审核中 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int State {get;set;}

     /// <summary>
     /// 说明:true此步骤审核完成 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean StateFlag {get;set;}

     /// <summary>
     /// 说明:创建时间 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public DateTime CreateTime {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean IsEnd {get;set;}

   }
            
}