using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCM.Manager.Models
{
    [SugarTable("Clients")]
    public class ClientsModel
    {
        public long ClientID { get; set; }

        public long ParentClientID { get; set; }

        public string RefClientID { get; set; }

        public string ClientName { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string AvatarID { get; set; }

        public string AvatarCustomization { get; set; }

        public DateTime RegisteredTime { get; set; }

        public string NickName { get; set; }

        public int UserType { get; set; }

        public string UserSource { get; set; }

        public string UserDeviceUniqueIdentifier { get; set; }

        public string UserDeviceModel { get; set; }

        public int TimeZoneID { get; set; }

        public string UserChannel { get; set; }

        public string IDFA { get; set; }

        public string CorrelationPlayerID { get; set; }

        public string DeviceToken { get; set; }

        public int Language { get; set; }

        public int Platform { get; set; }

        public string AdsID { get; set; }

        public string SocialEmail { get; set; }

        public string ClientIdentity { get; set; }
    }
}
