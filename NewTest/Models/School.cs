using System;
using System.Linq;
using System.Text;

namespace Models
{
    public class School
    {
        
     /// <summary>
     /// Desc:地域ID，关联area表 
     /// Default:- 
     /// Nullable:False 
     /// </summary>
        public int id {get;set;}

     /// <summary>
     /// Desc:- 
     /// Default:- 
     /// Nullable:True 
     /// </summary>
        public string name {get;set;}

     /// <summary>
     /// Desc:- 
     /// Default:- 
     /// Nullable:True 
     /// </summary>
        public int? AreaId {get;set;}

    }
}
