using System;
namespace T4Test.Entities.Tables
{
    public class Flow_Type{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:类别 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Name {get;set;}

     /// <summary>
     /// 说明:说明 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Remark {get;set;}

     /// <summary>
     /// 说明:创建时间 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public DateTime CreateTime {get;set;}

     /// <summary>
     /// 说明:排序 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int Sort {get;set;}

   }
            
}