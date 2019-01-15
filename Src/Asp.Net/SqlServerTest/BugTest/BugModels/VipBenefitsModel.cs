using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("VipBenefits")]
    public class VipBenefitsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public VipBenefitsModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 VipLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 MinVipCredit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 MaxVipCredit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MinRechargeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MaxRechargeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal RechargeBenefit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal RechargeRebate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal SoloWonBenefit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 RouletteLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 FortuneRouletteLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MinConsumeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MaxConsumeAmount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Boolean RewardTournamentEnable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal FortuneRouletteRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Boolean EnableVipService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Boolean EnableTransfer { get; set; }
    }
}