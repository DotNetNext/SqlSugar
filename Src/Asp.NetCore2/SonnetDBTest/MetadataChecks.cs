using System.Diagnostics;
using System.Security;
using System.Text;
using SqlSugar;

public static class MetadataChecks
{
    public static void Run(SqlSugarClient db)
    {
        db.Ado.ExecuteCommand("CREATE TABLE metadata_time (Id INT, At TIME, PRIMARY KEY(Id))");
        db.Ado.ExecuteCommand("INSERT INTO metadata_time(Id,At) VALUES (1,@at), (2,NULL)",
            new SugarParameter("@at", new TimeOnly(8, 30)));
        Ensure(db.Queryable<TimeSpanRecord>().InSingle(1).At == TimeSpan.FromHours(8.5),
            "TIME must materialize into TimeSpan entities.");
        Ensure(db.Queryable<TimeOnlyRecord>().InSingle(1).At == new TimeOnly(8, 30),
            "TIME must materialize into TimeOnly entities.");
        Ensure(db.Queryable<TimeSpanRecord>().InSingle(2).At == null &&
               db.Queryable<TimeOnlyRecord>().InSingle(2).At == null,
            "Nullable TIME values must remain null.");

        db.Ado.ExecuteCommand("CREATE TABLE metadata_defaults (" +
            "Id INT DEFAULT 5000000000 NOT NULL, " +
            "Number FLOAT DEFAULT 1.23456789012345, " +
            "Amount DECIMAL(18,4) DEFAULT 0.1234, " +
            "Enabled BOOL DEFAULT TRUE, " +
            "Name STRING DEFAULT 'O''Brien', " +
            "At TIME DEFAULT '08:30:00', " +
            "Created DATETIME DEFAULT '2026-10-09T08:30:00Z', " +
            "Data JSON DEFAULT '{\"quote\":\"x\"}', " +
            "LocalNow DATETIME DEFAULT CURRENT_DATETIME(), " +
            "UtcNow DATETIME DEFAULT CURRENT_UTC_DATETIME(), PRIMARY KEY(Id))");
        var generated = db.DbFirst.Where("metadata_defaults").IsCreateAttribute().IsCreateDefaultValue()
            .ToClassStringList("MetadataGenerated").Single().Value;
        Ensure(generated.Contains("TimeOnly? At", StringComparison.Ordinal), "DbFirst must use the native TIME reader type.");
        Ensure(generated.Contains("[SugarColumn(IsJson=true)]", StringComparison.Ordinal),
            "DbFirst must preserve the JSON attribute when metadata uses upper case type names.");
        CompileAndCheckDefaults(generated);
    }

    private static void CompileAndCheckDefaults(string generated)
    {
        var directory = Path.Combine(Path.GetTempPath(), "sqlsugar-sonnetdb-generated-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var sugarAssembly = SecurityElement.Escape(typeof(SqlSugarClient).Assembly.Location);
            File.WriteAllText(Path.Combine(directory, "Generated.csproj"),
                "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType>" +
                "<TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings>" +
                "<UseSharedCompilation>false</UseSharedCompilation></PropertyGroup>" +
                "<ItemGroup><Reference Include=\"SqlSugar\"><HintPath>" + sugarAssembly +
                "</HintPath></Reference></ItemGroup></Project>", new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(directory, "Model.cs"), generated, new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(directory, "Program.cs"), """
                using MetadataGenerated;
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
                var row = new metadata_defaults();
                if (row.Id != 5000000000L || row.Number != 1.23456789012345d || row.Amount != 0.1234m || row.Enabled != true ||
                    row.Name != "O'Brien" || row.At != new TimeOnly(8, 30) ||
                    row.Created != new DateTime(2026, 10, 9, 8, 30, 0, DateTimeKind.Utc) || row.Data != "{\"quote\":\"x\"}" ||
                    row.LocalNow?.Kind != DateTimeKind.Local || row.UtcNow?.Kind != DateTimeKind.Utc)
                    throw new InvalidOperationException("Generated defaults lost a value or its type.");
                Console.WriteLine("Generated DbFirst defaults passed.");
                """, new UTF8Encoding(false));

            var startInfo = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.Environment["MSBUILDDISABLENODEREUSE"] = "1";
            foreach (var argument in new[] { "run", "--project", "Generated.csproj", "--disable-build-servers", "--nologo", "-v", "quiet" })
                startInfo.ArgumentList.Add(argument);
            using var process = Process.Start(startInfo)!;
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            Console.WriteLine($"Generated model compiler PID={process.Id}; startedUtc={process.StartTime.ToUniversalTime():O}.");
            if (!process.WaitForExit(120_000))
            {
                process.Kill(entireProcessTree: true);
                if (!process.WaitForExit(10_000))
                    throw new TimeoutException("Generated model compiler did not exit after cancellation.");
                throw new TimeoutException("Generated model compiler exceeded 120 seconds.");
            }
            Ensure(process.ExitCode == 0, "Generated DbFirst models must compile and retain their defaults: " +
                output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
        }
        finally
        {
            if (Path.GetFullPath(directory).StartsWith(Path.GetFullPath(Path.GetTempPath()), StringComparison.OrdinalIgnoreCase))
                DeleteGeneratedFiles(directory);
        }
    }

    private static void DeleteGeneratedFiles(string directory)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                Directory.Delete(directory, recursive: true);
                return;
            }
            catch (IOException error) when ((error.HResult & 0xffff) is 32 or 33)
            {
                // Windows may briefly retain compiler/runtime handles after the child exits.
                if (attempt == 4)
                {
                    Console.WriteLine("Cleanup deferred for locked generated temporary files: " + directory);
                    return;
                }
                Thread.Sleep(100);
            }
        }
    }

    private static void Ensure(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    [SugarTable("metadata_time")]
    public class TimeSpanRecord
    {
        [SugarColumn(IsPrimaryKey = true)] public int Id { get; set; }
        public TimeSpan? At { get; set; }
    }

    [SugarTable("metadata_time")]
    public class TimeOnlyRecord
    {
        [SugarColumn(IsPrimaryKey = true)] public int Id { get; set; }
        public TimeOnly? At { get; set; }
    }
}
