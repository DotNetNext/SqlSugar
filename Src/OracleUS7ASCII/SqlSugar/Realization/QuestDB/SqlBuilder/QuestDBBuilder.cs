using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class QuestDBBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft
        {
            get
            {
                return "\"";
            }
        }
        public override string SqlTranslationRight
        {
            get
            {
                return "\"";
            }
        }
        public override string SqlDateNow
        {
            get
            {
                //https://questdb.io/docs/guides/working-with-timestamps-timezones/#using-utc-offset-for-conversions
                //https://questdb.io/docs/reference/function/date-time/#to_timezone
                //SELECT 
                //      now(),  --2022-10-21T07:19:50.680134Z
                //      systimestamp(), --2022-10-21T07:19:50.680278Z
                //      sysdate(), --2022-10-21T07:19:50.679Z
                //      to_timezone(NOW(), 'Asia/ShangHai'), --2022-10-21T15:19:50.680134Z
                //      to_timezone(NOW(), 'HKT'), --2022-10-21T15:19:50.680134Z
                //      to_timezone(NOW(),'+08') --2022-10-21T15:19:50.680134Z
                return $"to_timezone(NOW(),'{TimeZoneInfo.Local.BaseUtcOffset.Hours:00}')";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                //https://questdb.io/docs/guides/working-with-timestamps-timezones/#using-utc-offset-for-conversions
                //https://questdb.io/docs/reference/function/date-time/#to_timezone
                //SELECT 
                //      now(),  --2022-10-21T07:19:50.680134Z
                //      systimestamp(), --2022-10-21T07:19:50.680278Z
                //      sysdate(), --2022-10-21T07:19:50.679Z
                //      to_timezone(NOW(), 'Asia/ShangHai'), --2022-10-21T15:19:50.680134Z
                //      to_timezone(NOW(), 'HKT'), --2022-10-21T15:19:50.680134Z
                //      to_timezone(NOW(),'+08') --2022-10-21T15:19:50.680134Z
                return $"SELECT to_timezone(NOW(),'{TimeZoneInfo.Local.BaseUtcOffset.Hours:00}')";
            }
        }

        public bool isAutoToLower => false;
        public override string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains(".") && !propertyName.Contains(SqlTranslationLeft))
            {
                return string.Join(".", propertyName.Split('.').Select(it => $"{SqlTranslationLeft}{it.ToLower(isAutoToLower)}{SqlTranslationRight}"));
            }

            if (propertyName.Contains(SqlTranslationLeft)) return propertyName;
            else
                return SqlTranslationLeft + propertyName.ToLower(isAutoToLower) + SqlTranslationRight;
        }

        //public override string GetNoTranslationColumnName(string name)
        //{
        //    return name.TrimEnd(Convert.ToChar(SqlTranslationRight)).TrimStart(Convert.ToChar(SqlTranslationLeft)).ToLower();
        //}
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName.ToLower(isAutoToLower) + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName.ToLower(isAutoToLower) + SqlTranslationRight);
        }

        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            var context = this.Context;

            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.Contains(".") && !name.Contains("(") && !name.Contains("\".\""))
            {
                return string.Join(".", name.ToLower(isAutoToLower).Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else if (name.Contains("("))
            {
                return name;
            }
            else if (name.Contains(SqlTranslationLeft) && name.Contains(SqlTranslationRight))
            {
                return name;
            }
            else
            {
                return SqlTranslationLeft + name.ToLower(isAutoToLower).TrimEnd('"').TrimStart('"') + SqlTranslationRight;
            }
        }
        public override string GetUnionFomatSql(string sql)
        {
            return " ( " + sql + " )  ";
        }
    }
}
