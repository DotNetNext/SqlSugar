using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum DbType
    {
        MySql ,
        SqlServer,
        Sqlite,
        Oracle,
        PostgreSQL,
        Dm,
        Kdbndp,
        Oscar,
        [Obsolete("使用DbType.MySql，已经全部统一用MySqlConnector取代 MySql.Data 原因.NET7下面差了几倍性能")]
        MySqlConnector,
        Access,
        OpenGauss,
        QuestDB,
        HG,
        ClickHouse,
        GBase,
        Odbc,
        Custom =900
    }
}
