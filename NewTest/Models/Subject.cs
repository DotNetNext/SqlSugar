using System;
using System.Linq;
using System.Text;

namespace Models
{
    public class Subject
    {
        
     /// <summary>
     /// Desc:- 
     /// Default:- 
     /// Nullable:False 
     /// </summary>
        public int id {get;set;}

     /// <summary>
     /// Desc:学生ID 
     /// Default:- 
     /// Nullable:True 
     /// </summary>
        public int? studentId {get;set;}

     /// <summary>
     /// Desc:- 
     /// Default:- 
     /// Nullable:True 
     /// </summary>
        public string name {get;set;}

    }
}
