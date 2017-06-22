using System;
using System.Linq;
using System.Text;

namespace OrmTest.Models
{
    ///<summary>
    ///
    ///</summary>
    public class DataTestInfo2
    {
           public DataTestInfo2(){

           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public Guid PK {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public bool Bool1 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public bool? Bool2 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Text1 {get;set;}

    }
}
