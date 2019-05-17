using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("tLogonHistory")]
    public class tLogonHistoryModel
    {
        public long ClientID { get; set;}

        public DateTime CreateTime { get; set; }

        public int Type { get; set;}

        public string IPAddress { get; set; }

        public string Location { get; set; }

        public string TimeZone { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public string RegionName { get; set;}

        public string RegionCode { get; set; }

        public string CityName { get; set; }

        public string CityCode { get; set; }

        public string Version { get; set;}

        public string OriginalCountryCode { get; set; }

        public int DeviceVirtualizeModel { get; set; }

        public int DeviceCPUArchitecture { get; set; }

        public int DeviceVirtualizeIndexNumber { get; set;}
    }
}
