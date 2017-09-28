using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class ConnectionConfig
    {
        /// <summary>
        ///DbType.SqlServer Or Other
        /// </summary>
        public DbType DbType { get; set; }
        /// <summary>
        ///Database Connection string
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// true does not need to close the connection
        /// </summary>
        public bool IsAutoCloseConnection { get; set; }
        /// <summary>
        /// Default SystemTable,If you do not have system table permissions, use attribute
        /// </summary>
        public InitKeyType InitKeyType = InitKeyType.SystemTable;
        /// <summary>
        /// Configure External Services replace default services,For example, Redis storage
        /// </summary>
        public ConfigureExternalServices ConfigureExternalServices = _DefaultServices;
        private static ConfigureExternalServices _DefaultServices = new ConfigureExternalServices();
    }

    public class ConfigureExternalServices
    {

        private ISerializeService _SerializeService;
        private ICacheService _ReflectionInoCache;
        private ICacheService _DataInfoCache;

        public ISerializeService SerializeService
        {
            get
            {
                if (_SerializeService == null)
                    return DefaultServices.Serialize;
                else
                    return _SerializeService;
            }
            set{ _SerializeService = value;}
        }

        public ICacheService ReflectionInoCache
        {
            get
            {
                if (_ReflectionInoCache == null)
                    return DefaultServices.ReflectionInoCache;
                else
                    return _ReflectionInoCache;
            }
            set{_ReflectionInoCache = value;}
        }

        public ICacheService DataInfoCache
        {
            get
            {
                if (_DataInfoCache == null)
                    return DefaultServices.DataInoCache;
                else
                    return _DataInfoCache;
            }
            set { _DataInfoCache = value; }
        }
    }
}
