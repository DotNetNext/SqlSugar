using System;
namespace T4Test.Entities.Tables
{
    public class TestOfNull{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int id {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public DateTime? createDate {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public Byte[] bytes {get;set;}

   }
            
}