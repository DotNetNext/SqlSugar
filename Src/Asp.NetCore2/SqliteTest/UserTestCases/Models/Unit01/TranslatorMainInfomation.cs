using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    public abstract class TranslatorMainInfomation : ERPEntityBase
    {
        /// <summary>
        /// 译员ID
        /// </summary>
        [SugarColumn(ColumnName = "SupplierNo", IsNullable = true)]
        public string TranslatorID { get; set; }



    }

    public class ERPTranslatorMainInfomation : TranslatorMainInfomation
    {
        /// <summary>
        /// 客户绑定关系
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToMany, nameof(CustomerTranslatorBindingRelationship.TranslatorID), nameof(ERPTranslatorMainInfomation.TranslatorID))]
        public List<CustomerTranslatorBindingRelationship> CustomerBindingRelationships { get; set; }

    }
}
