using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfasda
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Tax_Base_EmpSerial>();
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings() { IsCorrectErrorSqlParameterName = true };
            var ent = new  Tax_Base_EmpSerial { Serial = "202401", 工号 = "1001", 户籍所在地_详细地址 = "湖南长沙" };
            var i = db.Updateable(ent).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommand();
        }
    }
    /// <summary>
    /// Tax_Base_EmpSerial
    /// </summary>
    [Serializable]
    [SugarTable("tax_base_empserial", "Tax_Base_EmpSerial", IsDisabledDelete = true)]
    public partial class Tax_Base_EmpSerial
    {
        /// <summary>
        ///Serial
        /// </summary>
        [SugarColumn(ColumnName = "serial", ColumnDescription = "Serial", IsPrimaryKey = true, Length = 20, IsNullable = false)]
        public string Serial { get; set; }

        /// <summary>
        ///工号
        /// </summary>
        [SugarColumn(ColumnName = "工号", ColumnDescription = "工号", Length = 50, IsNullable = true)]
        public string 工号 { get; set; }

        /// <summary>
        ///户籍所在地（详细地址）
        /// </summary>
        [SugarColumn(ColumnName = "户籍所在地（详细地址）", ColumnDescription = "户籍所在地（详细地址）", Length = 255, IsNullable = true)]
        public string 户籍所在地_详细地址 { get; set; }

        /// <summary>
        ///户籍所在地（详细地址）
        /// </summary>
        [SugarColumn(ColumnName = "户籍所在地（省）", ColumnDescription = "户籍所在地（省）", Length = 255, IsNullable = true)]
        public string 户籍所在地_省 { get; set; }

    }
}
