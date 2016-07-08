using System;
namespace T4Test.Entities.Tables
{
    public class TestUpdateColumns{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Guid VGUID {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int IdentityField {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Name {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Name2 {get;set;}

     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public DateTime? CreateTime {get;set;}

   }
            
}