using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    public class CustomerTranslatorBindingRelationship:ERPEntitySubBase
    {
        /// <summary>
        /// 译员ID 
        ///</summary>
        [SugarColumn(ColumnName = "gysbh")]
        public string TranslatorID { get; set; }


        /// <summary>
        /// 状态 
        ///</summary>
        [SugarColumn(ColumnName = "zt")]
        public string Status { get; set; }
      
        /// <summary>
        /// 客户的主信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(CustomerTranslatorBindingRelationship.ParentID), nameof(CustomerMainInfomation.RecordID))]
        public CustomerMainInfomation CustomerInfo { get; set; }
    }
}
