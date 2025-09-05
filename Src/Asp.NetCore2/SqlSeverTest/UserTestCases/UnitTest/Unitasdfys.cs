using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{

    internal class Unitsadfasysss
    {
        
        public static void Init()
        {
            var db = NewUnitTest.Db; 
            string password = "pws";

            var user = db.Queryable<User>()
                .WhereIF(!string.IsNullOrEmpty(password), x => x.Password == password)
                .ToSql();
            if (user.Value.FirstOrDefault().Value+"" != "pws-模拟加密") throw new Exception("unit error");
        }
        /// <summary>
        /// 数据库加密解密实现
        /// 用于实体、DTO
        /// </summary>
        public class SugarDataAesEncryptConverter : ISugarDataConverter
        {
            /// <summary>
            /// 参数加密
            /// </summary>
            /// <param name="columnValue">列的值</param>
            /// <param name="columnIndex">索引值</param>
            /// <returns></returns>
            public SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
            {
                /*
                 * 问题描述：
                 * 查询时，同个字段会执行两次自定义字段处理，导致数据库查询不到值
                 * columnValue 是上一次处理后的值！！！
                 */
                return new SugarParameter($"@Column{columnIndex}", columnValue + "-模拟加密");
            }

            /// <summary>
            /// 查询结果解密
            /// </summary>
            /// <typeparam name="T">字段的类型</typeparam>
            /// <param name="dr">行值</param>
            /// <param name="i">字段的类中索引</param>
            /// <returns></returns>
            public T QueryConverter<T>(IDataRecord dr, int i)
            {
                var value = dr.GetString(i);//字段的值

                try
                {
                    Console.WriteLine("value," + value);
                    return (T)(object)value.Replace("-模拟加密", "");
                }
                catch (Exception e)
                {
                    Console.WriteLine("SugarDataAesEncryptConverter.QueryConverter解密出错,返回原文！", e);

                    return (T)(object)value;
                }
            }
        }
        [SugarTable(TableName = "USER")]
        public class User
        {
            [SugarColumn(IsPrimaryKey = true, Length = 50)]
            public string Id { get; set; }

            /// <summary>
            /// </summary>
            [SugarColumn(ColumnName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// </summary>
            [SugarColumn(ColumnName = "Email")]
            public string Email { get; set; }

            /// <summary>
            /// SugarDataAesEncryptConverter aes加密
            /// </summary>
            [SugarColumn(ColumnName = "Password", SqlParameterDbType = typeof(SugarDataAesEncryptConverter))]
            public string Password { get; set; }

            [SugarColumn(ColumnName = "Remark")]
            public string Remark { get; set; }

            public string tenant_id { get; set; }
        }
    }


}
