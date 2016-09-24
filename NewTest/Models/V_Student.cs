using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class V_Student 
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
        public int? sch_id { get; set; }

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
        public Boolean? isOk { get; set; }

        public string SchoolName { get; set; }

        public string AreaName { get; set; }

        public string SubjectName { get; set; }
    }
}
