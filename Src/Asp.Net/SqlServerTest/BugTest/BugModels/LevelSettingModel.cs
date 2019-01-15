using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("LevelSetting")]
    public class LevelSettingModel
    {
        /// <summary>
        /// 
        /// </summary>
        public LevelSettingModel()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 UserLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MinExperiencePoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Decimal MaxExperiencePoints { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String UnlockAppModules { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 RouletteLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String LevelUpRemindTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String LevelUpRemindSubTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.String LevelUpRemindDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public System.Int32 FortuneRouletteLevel { get; set; }
    }
}