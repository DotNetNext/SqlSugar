using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    /// <summary>
    /// 项目信息表
    ///</summary>
    [SugarTable("ProjectMainInfomation")]
    public class ProjectMainInfomation : DEntityBase
    {
        //public string Name { get; set; }

        /// <summary>
        /// 项目调研状态
        /// </summary>
        public ProjectResearchStateDto ResearchState { get; set; } = ProjectResearchStateDto.RESEARCHING;

    }
}
