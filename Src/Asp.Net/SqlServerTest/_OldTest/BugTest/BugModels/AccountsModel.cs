using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("Accounts")]
    public class AccountsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AccountsModel()
        {
        }

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
        public System.Decimal Balance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal ExperiencePoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal RewardPoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal Payout { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Byte[] TS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.DateTime RegisteredTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 GoldBadges { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 SilverBadges { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 CurrentLevelMinPoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 NextLevelRequiredPoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 UserLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String CorrelationAccountID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal Energy { get; set; }
    }
}