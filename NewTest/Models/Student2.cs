using System;
using System.Linq;
using System.Text;

namespace Models
{
    public class Student2
    {

        /// <summary>
        /// Desc:- 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Desc:- 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Desc:学校ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int sch_id { get; set; }

        /// <summary>
        /// Desc:- 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// Desc:- 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean isOk { get; set; }

    }
}
