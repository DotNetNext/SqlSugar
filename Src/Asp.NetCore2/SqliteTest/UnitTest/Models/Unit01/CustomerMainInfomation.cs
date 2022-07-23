using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    public class CustomerMainInfomation:ERPEntityParentBase
    {
        /// <summary>
        /// 客户编号 
        ///</summary>
        [SugarColumn(ColumnName = "CustomerNo")]
        public string CustomerNo { get; set; }
   
    
        /// <summary>
        /// 客户名称 
        ///</summary>
        [SugarColumn(ColumnName = "CustomerCorpName")]
        public string CustomerName { get; set; }

    }
}
