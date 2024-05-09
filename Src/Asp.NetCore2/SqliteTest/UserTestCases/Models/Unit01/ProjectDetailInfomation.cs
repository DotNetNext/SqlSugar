using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    [SugarTable("ProjectDetailInfomation")]
    public class ProjectDetailInfomation : DEntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        //public string Name { get; set; }

        /// <summary>
        /// 项目信息主键， ProjectMainInfomation表
        ///</summary>
        [SugarColumn(ColumnName = "MainId")]
        public long MainId { get; set; }

        /// <summary>
        /// 项目的主信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        [Navigate(NavigateType.OneToOne, nameof(MainId))]
        public ProjectMainInfomation ProjectMainInfo { get; set; }

    }
}
