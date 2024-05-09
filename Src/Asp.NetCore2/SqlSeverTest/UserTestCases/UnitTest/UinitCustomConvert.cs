using SqlSugar.DbConvert;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Linq;

namespace OrmTest
{
    internal class UinitCustomConvert
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Uinitadfa22122>();
            db.DbMaintenance.TruncateTable<Uinitadfa22122>();
            db.Insertable(new Uinitadfa22122()
            {
                DcValue = new Dictionary<string, object>() { { "1", 1 } }
            }
            ).ExecuteCommand();
            var data = db.Queryable<Uinitadfa22122>().ToList();
            if (data.First().EnumValue != null)
            {
                throw new Exception("unit error");
            }

            db.Updateable(new Uinitadfa22122()
            {
                Id = 1,
                DcValue = new Dictionary<string, object>() { { "1", 2 } },
                EnumValue = SqlSugar.DbType.MySql
            }
           ).ExecuteCommand();
            data = db.Queryable<Uinitadfa22122>().Where(it => it.EnumValue == SqlSugar.DbType.MySql).ToList();
            var data2 = db.Queryable<Uinitadfa22122>().Where(it => SqlSugar.DbType.MySql == it.EnumValue).ToList();
            if (data.First().EnumValue != SqlSugar.DbType.MySql)
            {
                throw new Exception("unit error");
            }

            var dt = db.Queryable<Uinitadfa22122>().ToDataTable();
            if (dt.Columns["EnumValue"].DataType != typeof(string))
            {
                throw new Exception("unit error");
            }
            if (data2.First().EnumValue != data.First().EnumValue)
            {
                throw new Exception("unit error");
            }
            Demo1(db);
            Demo2(db);
            Demo3(db);
            var afterMsgContent = "1";
            var msgContent="2";
            var updateable =db.Updateable<TestContent>()

             .SetColumns(it => it.CONTENT == afterMsgContent)
                 .Where(it => it.CONTENT==msgContent) 
             .ToSql();
            if (updateable.Value.First().ParameterName == updateable.Value.Last().ParameterName) 
            {
                throw  new Exception("unit error");
            }
        }

        private static void Demo1(SqlSugarClient db)
        {
            var datas = db.Queryable<Uinitadfa22122X>()
                .Where(it => it.EnumValue.Contains("a"))
                .ToSql();
            if (datas.Value.FirstOrDefault().DbType != System.Data.DbType.AnsiString)
            {
                throw new Exception("unit error");
            }
        }
        private static void Demo2(SqlSugarClient db)
        {
            var datas = db.Queryable<Uinitadfa22122X>()
                .Where(it => it.EnumValue2.Contains("a"))
                .ToSql();
            if (datas.Value.FirstOrDefault().IsNvarchar2 !=true)
            {
                throw new Exception("unit error");
            }
        }
        private static void Demo3(SqlSugarClient db)
        {
            var datas = db.Queryable<Uinitadfa22122X>()
                .Where(it => it.EnumValue3.Contains("a"))
                .ToSql();
            if (datas.Value.FirstOrDefault().DbType != System.Data.DbType.String)
            {
                throw new Exception("unit error");
            }
        }
    }
    [SugarTable("TEST_CONTENT")]
    public class TestContent
    {
        /// <summary>
        ///  ID
        ///</summary>
        [SugarColumn(ColumnName = "ID", ColumnDescription = "Id", OracleSequenceName = "TEST_CONTENT_ID_SEQ", IsIdentity = true, IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        ///  内容
        ///</summary>
        [SugarColumn(ColumnName = "CONTENT", ColumnDescription = "CONTENT", ColumnDataType = "nvarchar2", SqlParameterDbType = typeof(Nvarchar2PropertyConvert))]
        public string CONTENT { get; set; } = string.Empty;


        [SugarColumn(ColumnName = "EditId", ColumnDescription = "EditId")]
        public long EditId { get; set; } = 218;

    }
    public class Uinitadfa22122X
    {

        
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(20)", SqlParameterDbType = System.Data.DbType.AnsiString, IsNullable = true)]
        public string EnumValue { get; set; }

        [SqlSugar.SugarColumn(ColumnDataType = "varchar(20)", SqlParameterDbType =typeof(Uinitadfa22122XConvert), IsNullable = true)]
        public string EnumValue2 { get; set; }

        public string EnumValue3 { get; set; }
    }
    public class Uinitadfa22122XConvert : ISugarDataConverter
    {
        public SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
        {
            var name = "@MyEnmu" + columnIndex;
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            if (columnValue == null)
            {
                return new SugarParameter(name, null);
            }
            else
            { 
                return new SugarParameter(name, columnValue) {  DbType=System.Data.DbType.AnsiString,IsNvarchar2=true };
            }
        }

        public T QueryConverter<T>(IDataRecord dr, int i)
        {

            var str = dr.GetString(i);
            Type undertype = SqlSugar.UtilMethods.GetUnderType(typeof(T));//获取没有nullable的枚举类型
            return (T)Enum.Parse(undertype, str);
        }
    }
    public class Uinitadfa22122
    {

        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SqlSugar.SugarColumn(ColumnDataType = "varchar(max)", SqlParameterDbType = typeof(DictionaryConvert), IsNullable = true)]
        public Dictionary<string, object> DcValue { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(20)", SqlParameterDbType = typeof(EnumToStringConvert), IsNullable = true)]
        public SqlSugar.DbType? EnumValue { get; set; }
    }


    public class DictionaryConvert : ISugarDataConverter
    {
        public SugarParameter ParameterConverter<T>(object value, int i)
        {
            var name = "@myp" + i;
            var str = new SerializeService().SerializeObject(value);
            return new SugarParameter(name, str);
        }

        public T QueryConverter<T>(IDataRecord dr, int i)
        {
            var str = dr.GetValue(i) + "";
            return new SerializeService().DeserializeObject<T>(str);
        }
    }
}
