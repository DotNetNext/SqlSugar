
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("Exposure")]
    public class ExposureModel
    {
        /// <summary>
        /// 
        /// </summary>
        public System.Int64 ClientID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 TournamentID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 NodeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 Period { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal Exposure { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Byte[] TS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 ExtraMatchID { get; set; }
    }
}