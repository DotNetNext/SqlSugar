using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public interface IConnectionConfig
    {
        string DbType { get; set; }
        string ConnectionString { get; set; }
        bool IsAutoCloseConnection { get; set; }
    }

}
