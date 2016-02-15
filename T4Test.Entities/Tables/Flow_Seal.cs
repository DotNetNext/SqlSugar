using System;
namespace T4Test.Entities.Tables
{
    public class Flow_Seal{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Path {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public DateTime? CreateTime {get;set;}

     /// <summary>
     /// 说明:使用者 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Using {get;set;}

   }
            
}