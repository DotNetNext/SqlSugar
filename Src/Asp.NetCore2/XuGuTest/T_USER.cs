using System;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using SqlSugar;
using SqlSugar.Xugu;
using XuguClient;

namespace Data.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("MY_USER1",TableDescription ="测试表")]
    public partial class MY_USER
    {
        public MY_USER()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity=true, ColumnDescription ="主键")]
        public int ID { get; set; }

 
        public long C_BIGINT { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public byte[] C_BINARY { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public byte[] C_BLOB { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public bool? C_BOOLEAN { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string C_VARCHAR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>          
        [SugarColumn(ColumnDataType = "Char")]
        public string C_CHAR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "clob")]
        public string C_CLOB { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "date")]
        public DateTime? C_DATE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? C_DATETIME { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert))]
        public DateTimeOffset? C_DATETIME_WITH_TIME_ZONE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? C_DECIMAL { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? C_DOUBLE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public float? C_FLOAT { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public Guid? C_GUID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? C_INT { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? C_INTEGER { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL YEAR")]
        public string C_INTERVAL_YEAR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL YEAR TO MONTH")]
        public string C_INTERVAL_YEAR_TO_MONTH { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL MONTH")]
        public string C_INTERVAL_MONTH { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL DAY")]
        public string C_INTERVAL_DAY { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL DAY TO HOUR")]
        public string C_INTERVAL_DAY_TO_HOUR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL HOUR")]
        public string C_INTERVAL_HOUR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL DAY TO MINUTE")]
        public string C_INTERVAL_DAY_TO_MINUTE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>          
        [SugarColumn(ColumnDataType = "INTERVAL HOUR TO MINUTE")]
        public string C_INTERVAL_HOUR_TO_MINUTE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL MINUTE")]
        public string C_INTERVAL_MINUTE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL DAY TO SECOND")]
        public string C_INTERVAL_DAY_TO_SECOND { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "INTERVAL HOUR TO SECOND")]
        public string C_INTERVAL_HOUR_TO_SECOND { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [SugarColumn(ColumnDataType = "INTERVAL MINUTE TO SECOND")]
        public string C_INTERVAL_MINUTE_TO_SECOND { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>        
        [SugarColumn(ColumnDataType = "INTERVAL SECOND")]
        public string C_INTERVAL_SECOND { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "NCHAR")]
        public string C_NCHAR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? C_NUMERIC { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "NVARCHAR")]
        public string C_NVARCHAR { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "ROWID")]
        public string C_ROWID { get; set; }

        

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public sbyte? C_TINYINT { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert), ColumnDataType = "TIME")]
        public TimeSpan? C_TIME { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>    
        [SugarColumn(ColumnDataType = "TIMESTAMP")]
        public DateTime? C_TIMESTAMP { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnDataType = "TIMESTAMP AUTO UPDATE")]
        public DateTime? C_TIMESTAMP_AUTO_UPDATE { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert), ColumnDataType = "TIME WITH TIME ZONE")]
        public TimeSpan? C_TIME_WITH_TIME_ZONE { get; set; }

    }
}
