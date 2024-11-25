using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{

    internal class Unitadfayyadfa
    {
        public static void Init()
        {
            //创建数据库对象
            SqlSugarClient Db = NewUnitTest.Db;
            Db.CodeFirst.InitTables<A1, Dic, B1>();

            var data = Db.Context.Queryable<A1>()
                   .Select(x => new
                   {
                       CategoryName = x.DataType1BInfo.DicInfo.Name
                   })
                   .ToList();

            var sql = Db.Context.Queryable<A1>()
            .Select(x => new
            {
                CategoryName = x.DataType1BInfo.DicInfo.Name
            })
            .ToSqlString();
            if (!sql.Contains("WHERE DataType='1'")) 
            {
                throw new Exception("unit error");
            }
        }
    }

    public class Dic
    {
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnDescription = "type", Length = 50)]
        public string? Type { get; set; }
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnDescription = "code", Length = 50)]
        public string? Code { get; set; }
        /// <summary>
        ///
        /// </summary>
        [SugarColumn(ColumnDescription = "Name", Length = 50)]
        public string? Name { get; set; }
    }
    public class View1
    {
        public string Name { get; set; }
    }

    public enum CategoryEnum
    {
        Category1 = 1,
        Category2 = 2,
        Category3 = 3
    }

    [SugarTable("unitAdfafas")]
    public class A1
    {
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(B1.AId), "DataType='1'")]
        public B1 DataType1BInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(B1.AId), "DataType='2'")]
        public B1 DataType2BInfo { get; set; }
    }
    [SugarTable("unitBadfafas")]
    public class B1
    {
        public string Id { get; set; }

        public string DataType { get; set; }

        public string AId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(Dic.Code), "Type='Category'")]
        public Dic DicInfo { get; set; }
    }

}
