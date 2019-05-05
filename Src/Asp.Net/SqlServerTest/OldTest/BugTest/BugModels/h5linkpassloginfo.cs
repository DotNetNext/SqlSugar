using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace sugarentity
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("h5linkpassloginfo")]
    public partial class h5linkpassloginfo
    {
        public h5linkpassloginfo()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int LogKeyId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string SessionId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int UserId { get; set; }

        /// <summary>
        /// Desc:0:外部用户   1:内部用户
        /// Default:0
        /// Nullable:False
        /// </summary>           
        public byte Inside { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int H5PackageId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int LinkPassId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int LinkType { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public int? LinkBusinessTemplateId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public int? LinkBusinessObjectId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public int? IntVal { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public string StrVal { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public string JsonVal { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int ClientIP { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int ServerIP { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string UserAgentHashKey { get; set; }

        /// <summary>
        /// Desc:
        /// Default:NULL
        /// Nullable:True
        /// </summary>           
        public string OtherLogsGuid { get; set; }

        /// <summary>
        /// Desc:
        /// Default:current_timestamp()
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsIgnore = true)]
        public DateTime Intime { get; set; }

    }
}
