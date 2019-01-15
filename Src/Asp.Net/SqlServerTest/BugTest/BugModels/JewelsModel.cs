using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("Jewels")]
    public class JewelsModel
    {
        public long ClientID { get; set; }

        public decimal JewelCount { get; set; }

        public int PropType { get; set; }

    }
}
