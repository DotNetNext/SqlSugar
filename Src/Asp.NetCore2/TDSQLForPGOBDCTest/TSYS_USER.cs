using SqlSugar;
using System;

namespace TDSQLForPGOBDCTest
{
    ///<summary>
    /// XIR用户表
    ///</summary>
    [SugarTable("TSYS_USER")]
    public partial class TSYS_USER
    {
        public TSYS_USER()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>            
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, Length = 30)]
        public string U_ID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 50)]
        public string U_NAME { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 1)]
        public string U_STATE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 2000)]
        public string U_REMARK { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(IsNullable = true, Length = 100)]
        public string U_EMAIL { get; set; }

    }
}
