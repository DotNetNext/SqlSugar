using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public interface IConnectionConfig
    {
        string DbType { get; set; }
        string ConnectionString { get; set; }
        bool IsAutoCloseConnection { get; set; }
    }

    public class SystemTableConfig : IConnectionConfig
    {
        public string DbType { get; set; }
        public string ConnectionString { get; set; }
        public bool IsAutoCloseConnection { get; set; }
    }

    public class AttributeConfig : IConnectionConfig
    {
        public string EntityNamespace { get; set; }
        public string DbType { get; set; }
        public string ConnectionString { get; set; }
        public bool IsAutoCloseConnection { get; set; }
    }
}
