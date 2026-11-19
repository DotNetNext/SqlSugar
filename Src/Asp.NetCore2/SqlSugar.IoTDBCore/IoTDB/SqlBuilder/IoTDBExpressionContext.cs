using System;
using System.Linq;
using System.Text.RegularExpressions;
using SqlSugar;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBExpressionContext : ExpressionContext, ILambdaExpressions
    {
        private static readonly Regex IdentifierPath = new Regex(@"^[A-Za-z0-9_$.]+$", RegexOptions.Compiled);

        public SqlSugarProvider Context { get; set; }

        public IoTDBExpressionContext()
        {
            DbMehtods = new IoTDBExpressionMethods();
        }

        public override string SqlTranslationLeft => string.Empty;
        public override string SqlTranslationRight => string.Empty;

        public override string GetTranslationColumnName(string columnName)
        {
            if (string.IsNullOrEmpty(columnName) || columnName[0] == SqlParameterKeyWord[0] || !IdentifierPath.IsMatch(columnName))
                return columnName;

            return string.Join(".", columnName.Split('.').Select(name =>
                name.Equals("timestamp", StringComparison.OrdinalIgnoreCase) ? "time" : name));
        }
    }

    internal sealed class IoTDBExpressionMethods : DefaultDbMethod
    {
        public override string TrueValue() => "true";
        public override string FalseValue() => "false";
    }
}
