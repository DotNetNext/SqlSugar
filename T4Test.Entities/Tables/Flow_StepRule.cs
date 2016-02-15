using System;
namespace T4Test.Entities.Tables
{
    public class Flow_StepRule{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:哈哈 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string StepId {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string AttrId {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Operator {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Result {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string NextStep {get;set;}

   }
            
}