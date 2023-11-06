using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICrewApi.Models
{
    public class DSPersonScheduleModel
    {
        public string AC_REG { get; set; }
        public string AC_TYPE { get; set; }
        public string ARRIVAL_AIRPORT { get; set; }
        public DateTime ATA { get; set; }
        public DateTime ATD { get; set; }
        public long CREW_LINK_LINE { get; set; }
        public long CREW_PAIRING_LINE { get; set; }
        public string DDO_CODE { get; set; }
        public string DEPARTURE_AIRPORT { get; set; }
        public string D_OR_I { get; set; }
        public string FLIGHT_COM { get; set; }
        public long FLIGHT_ID { get; set; }
        public string FLIGHT_VOYAGE { get; set; }
        public DateTime FROM_DATE { get; set; }
        public string MODIFY_REASON { get; set; }
        public string PAIRINGVOYAGE { get; set; }
        public string P_CODE { get; set; }
        public string RANK_NO { get; set; }
        public string REASON { get; set; }
        public string SCHEDULE_TYPE { get; set; }
        public DateTime TA { get; set; }
        public DateTime TD { get; set; }
        public DateTime TIME_FROM_PORT { get; set; }
        public DateTime TIME_TO_PORT { get; set; }
        public DateTime TO_DATE { get; set; }

        public string FLG_CS { get; set; }
        public string REMARKS { get; set; }

        public int FLY_HOURS { get; set; }
        /// <summary>
        /// 前车时间
        /// </summary>
        public short? PREVIOUS_RIDE_TIME { set; get; }
        /// <summary>
        /// 后车程时间
        /// </summary>
        public short? REAR_RIDE_TIME { set; get; }
        public string DEPA_AIRPORT { get; set; }
        public string DEPA_NATIVE { get; set; }
        public int DEPA_RESTTIME { get; set; }
        public int DEPA_ZONE_TIME { get; set; }
        /// <summary>
        /// 起飞机场类别
        /// </summary>
        public string DEPA_TYPE { get; set; }
        /// <summary>
        /// 落地机场类别
        /// </summary>
        public string ARRI_TYPE { get; set; }
        public string ARRI_AIRPORT { get; set; }
        public string ARRI_NATIVE { get; set; }
        public int ARRI_RESTTIME { get; set; }
        public int ARRI_ZONE_TIME { get; set; }
    }

}
