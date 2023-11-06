using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICrewApi.Models
{
    public class PersonScheduleParam
    {
        public List<string> P_CODE { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public string MODULEFLAG { get; set; }
    }
}
