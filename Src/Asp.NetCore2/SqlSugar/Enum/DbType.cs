using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum DbType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4,
        Dm = 5,
        Kdbndp = 6,
        Oscar = 7,
        MySqlConnector = 8,
        Access = 9,
        OpenGauss = 10,
        QuestDB = 11,
        HG = 12,
        ClickHouse = 13,
        GBase = 14,
        Odbc = 15,
        OceanBaseForOracle = 16,
        TDengine = 17,
        GaussDB = 18,
        OceanBase = 19,
        Tidb = 20,
        Vastbase = 21,
        PolarDB = 22,
        Doris = 23,
        Xugu = 24,
        GoldenDB = 25,
        TDSQLForPGODBC = 26,
        TDSQLForOracleODBC = 27,
        TDSQL = 28,
        HANA = 29,
        DB2 = 30,
        GaussDBNative = 31,
        DuckDB = 32,
        MongoDb = 33,
        Custom = 900
    }
}
