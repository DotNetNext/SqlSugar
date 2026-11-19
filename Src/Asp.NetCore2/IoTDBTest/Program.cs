using Apache.IoTDB.Data;
using SqlSugar;
using SqlSugar.IoTDB;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DataTable = System.Data.DataTable;

RunUnitSmoke();

var connectionString = Environment.GetEnvironmentVariable("IOTDB_CONNECTION_STRING");
if (!string.IsNullOrWhiteSpace(connectionString))
    RunServerSmoke(connectionString);

static void RunUnitSmoke()
{
    var options = IoTDBConnectionOptions.Parse("Host=127.0.0.1;Port=6667;Username=root;Password=root;Database=root.smoke;FetchSize=100");
    Assert(options.Database == "root.smoke", "database path is extracted from connection string");
    Assert(options.ProviderConnectionString.Contains("DataSource=127.0.0.1"), "Host is translated to DataSource");
    Assert(!options.ProviderConnectionString.Contains("Database="), "custom database setting is removed before ADO client parsing");
    Assert(IoTDBDbMaintenance.BuildCreateDatabaseSql("root.smoke") == "CREATE DATABASE root.smoke", "database creation uses IoTDB 2.x syntax");
    Assert(IoTDBDbMaintenance.BuildCreateTimeseriesSql("root.smoke.device.temperature", "DOUBLE") == "CREATE TIMESERIES root.smoke.device.temperature WITH DATATYPE=DOUBLE, ENCODING=GORILLA", "timeseries creation uses IoTDB 2.x syntax");
    Assert(options.GetDevicePath("sensor") == "root.smoke.sensor", "device names use the configured root path");
    Assert(options.GetMeasurementName("Timestamp") == "time", "timestamp columns map to IoTDB time");

    var sql = "select * from root.smoke.sensor where temperature=@value and note='@value' -- @value\n/* @value */ and ok=@flag";
    var bound = IoTDBParameterBinder.Bind(sql, new[]
    {
        new KeyValuePair<string, object>("@value", "a'b"),
        new KeyValuePair<string, object>("@flag", true)
    });
    Assert(bound.Contains("temperature='a''b'"), "string parameter is escaped");
    Assert(bound.Contains("note='@value'"), "quoted text is unchanged");
    Assert(bound.Contains("-- @value"), "line comments are unchanged");
    Assert(bound.Contains("/* @value */"), "block comments are unchanged");
    Assert(bound.Contains("ok=true"), "boolean parameter is formatted");
    Assert(IoTDBParameterBinder.ToSqlLiteral(DateTime.UnixEpoch) == "0", "timestamps are bound as epoch milliseconds");

    var db = new SqlSugarClient(new ConnectionConfig
    {
        DbType = DbType.IoTDB,
        ConnectionString = "Host=127.0.0.1;Port=6667;Username=root;Password=root;Database=root.smoke",
        IsAutoCloseConnection = true
    });
    Assert(db.Ado.GetType().Name == "IoTDBProvider", "SqlSugar loads the IoTDB provider plugin");
    Assert(db.Ado.Connection is IoTDBConnection, "provider creates the Apache IoTDB connection");
    Assert(InstanceFactory.GetDbFirst(db.CurrentConnectionConfig).GetType().Name == "IoTDBDbFirst", "SqlSugar loads the IoTDB DbFirst provider");
    Assert(db.Queryable<SmokePoint>().GetType().Name == "IoTDBQueryable`1", "SqlSugar loads the IoTDB queryable provider");
    Assert(InstanceFactory.CreateInstance<IFastBuilder>("SqlSugar.IoTDBFastBuilder") is IoTDBFastBuilder, "SqlSugar loads the IoTDB fastest provider");
    var bulkSchema = db.Queryable<SmokePoint>().Where(item => false).Select("*").ToDataTable();
    Assert(bulkSchema.Columns.Contains("time") && bulkSchema.Columns.Contains("Temperature"), "Fastest schema discovery uses entity metadata without querying IoTDB");

    var bulkData = new DataTable("root.smoke.bulk_points");
    bulkData.Columns.Add("time", typeof(DateTime));
    bulkData.Columns.Add(nameof(SmokePoint.Temperature), typeof(double));
    bulkData.Columns.Add(nameof(SmokePoint.Note), typeof(string));
    bulkData.Rows.Add(DateTime.UnixEpoch.AddMilliseconds(2), 12.5, "later");
    bulkData.Rows.Add(DateTime.UnixEpoch.AddMilliseconds(1), 11.5, "earlier");
    var tablet = IoTDBFastBuilder.BuildTablet(bulkData, options, db.EntityMaintenance.GetEntityInfo(typeof(SmokePoint)));
    Assert(tablet.InsertTargetName == "root.smoke.bulk_points", "Fastest tablet targets the configured device path");
    Assert(tablet.Measurements.Count == 2 && tablet.Measurements[0] == nameof(SmokePoint.Temperature), "Fastest tablet excludes the time column from measurements");
    Assert(tablet.DataTypes[0] == Apache.IoTDB.TSDataType.DOUBLE && tablet.DataTypes[1] == Apache.IoTDB.TSDataType.TEXT, "Fastest tablet maps SqlSugar entity types to IoTDB types");
    var encodedTimestamps = tablet.GetBinaryTimestamps();
    var firstTimestamp = BinaryPrimitives.ReadInt64BigEndian(encodedTimestamps.AsSpan(0, 8));
    var secondTimestamp = BinaryPrimitives.ReadInt64BigEndian(encodedTimestamps.AsSpan(8, 8));
    Assert(firstTimestamp == 1, $"IoTDB tablet sorts rows by timestamp (first: {firstTimestamp})");
    Assert(secondTimestamp == 2, $"IoTDB tablet keeps timestamp/value rows aligned (second: {secondTimestamp})");
    Assert(tablet.GetBinaryValues().Length > 0, "Fastest tablet encodes measurement values");

    var insert = db.Insertable(new SmokePoint { Timestamp = DateTime.UnixEpoch, Temperature = 12.5, Note = "smoke" }).ToSql();
    Assert(insert.Key.Contains("INSERT INTO root.smoke.smoke_points"), "insert SQL targets the configured device");
    Assert(insert.Key.Contains("time"), "insert SQL maps timestamp property to IoTDB time");
    Assert(insert.Key.Contains("@Temperature"), "insert values remain parameterized until provider execution");

    var query = db.Queryable<SmokePoint>().Where(it => it.Temperature > 10).Take(10).ToSql();
    Assert(query.Key.Contains("FROM root.smoke.smoke_points"), "query SQL targets the configured device");
    Assert(query.Key.Contains("LIMIT"), "query pagination uses IoTDB LIMIT syntax");
    var firstQuery = db.Queryable<SmokePoint>().Where(it => it.Timestamp == DateTime.UnixEpoch).Take(1).ToSql();
    Assert(!firstQuery.Key.Contains("SELECT time,"), "entity reads rely on IoTDB's implicit time column");
    Assert(firstQuery.Key.Contains("ORDER BY time"), "First queries use IoTDB time ordering");

    var timeQuery = db.Queryable<SmokePoint>().Where(it => it.Timestamp > DateTime.UnixEpoch).Take(1).ToSql();
    Assert(timeQuery.Key.Contains("time"), "timestamp predicates map to the IoTDB time column");
}

static void RunServerSmoke(string connectionString)
{
    var database = "root.smoke_" + Guid.NewGuid().ToString("N");
    var connectionValues = new DbConnectionStringBuilder { ConnectionString = connectionString };
    foreach (var key in connectionValues.Keys.Cast<string>().Where(key => key.Equals("Database", StringComparison.OrdinalIgnoreCase) || key.Equals("RootPath", StringComparison.OrdinalIgnoreCase)).ToList())
        connectionValues.Remove(key);
    connectionValues["Database"] = database;

    SqlSugarClient db = null;
    Exception? failure = null;
    var databaseCreated = false;
    try
    {
        db = new SqlSugarClient(new ConnectionConfig
        {
            DbType = DbType.IoTDB,
            ConnectionString = connectionValues.ConnectionString,
            IsAutoCloseConnection = true
        });
        databaseCreated = true;
        db.CodeFirst.InitTables<SmokePoint>();
        db.CodeFirst.InitTables<SmokePoint>();
        var timestamp = DateTime.UtcNow.AddMilliseconds(-5);
        var inserted = db.Fastest<SmokePoint>().AS("smoke_points").BulkCopy(new List<SmokePoint>
        {
            new SmokePoint { Timestamp = timestamp, Temperature = 12.5, Note = "server-smoke" }
        });
        Assert(inserted == 1, "server bulk insert reports inserted row count");
        var row = db.Queryable<SmokePoint>().Where(it => it.Timestamp == timestamp).First();
        Assert(row != null && Math.Abs(row.Temperature - 12.5) < 0.0001, "server query returns the inserted IoTDB point");
        Console.WriteLine("IoTDB server smoke passed for " + database + ".");
    }
    catch (Exception error)
    {
        failure = error;
        throw;
    }
    finally
    {
        if (db != null)
        {
            try
            {
                if (databaseCreated)
                {
                    db.Ado.ExecuteCommand("DROP DATABASE " + database);
                    var remains = db.DbMaintenance.GetDataBaseList().Any(item => item.Equals(database, StringComparison.OrdinalIgnoreCase));
                    if (remains) throw new InvalidOperationException("IoTDB smoke database cleanup did not remove " + database + ".");
                }
            }
            catch (Exception cleanupError)
            {
                if (failure == null) throw;
                Console.Error.WriteLine("IoTDB smoke database cleanup failed for " + database + ": " + cleanupError.Message);
            }
            db.Dispose();
        }
    }
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException("Smoke assertion failed: " + message);
}

[SugarTable("smoke_points")]
public sealed class SmokePoint
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "time")]
    public DateTime Timestamp { get; set; }

    public double Temperature { get; set; }

    public string Note { get; set; } = string.Empty;
}
