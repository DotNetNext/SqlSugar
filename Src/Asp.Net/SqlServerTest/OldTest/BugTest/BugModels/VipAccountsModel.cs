using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("VipAccounts")]
    public class VipAccountsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public VipAccountsModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int64 ClientID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 VipCredit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 VipLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal AccumulatedRechargeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal AccumulatedConsumeAmount { get; set; }
    }
}