using System;
namespace T4Test.Entities.Tables
{
    public class Flow_FormContentStepCheckState{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string StepCheckId {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string UserId {get;set;}

     /// <summary>
     /// 说明:1通过0不通过2审核中 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int CheckFlag {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Reamrk {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string TheSeal {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public DateTime CreateTime {get;set;}

   }
            
}