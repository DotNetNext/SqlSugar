using SqlSugar;
using SqlSugar.DbConvert;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            if (data.First().DcValue["1"].ObjToInt() != 1)
            {
                throw new Exception("unit error");
            }
            db.Updateable(new Uinitadfa22122()
            {
                Id = 1,
                DcValue = new Dictionary<string, object>() { { "1", 2 } },
                EnumValue=SqlSugar.DbType.MySql
            }
           ).ExecuteCommand();
             data =db.Queryable<Uinitadfa22122>().Where(it=>it.EnumValue==SqlSugar.DbType.MySql).ToList();
             var data2 = db.Queryable<Uinitadfa22122>().Where(it => SqlSugar.DbType.MySql== it.EnumValue   ).ToList();
            if (data.First().EnumValue != SqlSugar.DbType.MySql)
            {
                throw new Exception("unit error");
            }
            if (data.First().DcValue["1"].ObjToInt() != 2)
            {
                throw new Exception("unit error");
            }
            var dt= db.Queryable<Uinitadfa22122>().ToDataTable();
            if (dt.Columns["EnumValue"].DataType != typeof(string)) 
            {
                throw new Exception("unit error");
            }
            if (data2.First().EnumValue!=data.First().EnumValue)
            {
                throw new Exception("unit error");
            }
        }
    }

    public class Uinitadfa22122
    {

        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public int Id { get; set; }

        [SqlSugar.SugarColumn(ColumnDataType = "varchar(max)", SqlParameterDbType = typeof(DictionaryConvert),IsNullable =true)]
        public Dictionary<string, object> DcValue { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(20)", SqlParameterDbType = typeof(EnumToStringConvert),IsNullable =true)]
        public SqlSugar.DbType? EnumValue { get;set; }
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
