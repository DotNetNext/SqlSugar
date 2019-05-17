using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace sugarentity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("logtype")]
    public partial class logtype
    {
        public logtype()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string LogKey { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public string KeyName { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public string Description { get; set; }

        /// <summary>
        /// Desc:0:否 1:是
        /// Default:
        /// Nullable:False
        /// </summary>           
        public byte IsKeyNode { get; set; }

        /// <summary>
        /// Desc:
        /// Default:current_timestamp()
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsIgnore = true)]
        public DateTime InTime { get; set; }

    }
}
