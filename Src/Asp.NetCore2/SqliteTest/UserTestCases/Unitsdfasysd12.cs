using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    
    internal class Unitsdfasysd12
    {
        public static void Init() 
        {
            InitAsync().GetAwaiter().GetResult();
        }
        public static async Task InitAsync()
        {

            SqlSugarClient Db = NewUnitTest.Db;

            //建库
            Db.DbMaintenance.CreateDatabase();//达梦和Oracle不支持建库

            //建表（看文档迁移）
            Db.CodeFirst.InitTables<Unit2sdsaTestDb>(); //所有库都支持

            if (!await Db.Queryable<Unit2sdsaTestDb>().AnyAsync())
                await Db.Insertable(new Unit2sdsaTestDb() { Id = 1, IsDeleted = false, Num = 1254632, My = MyType.B }).ExecuteCommandAsync();

            var input = new Dto() { IsDeleted = true, Num = 6666, My = MyType.E };



            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            //我的环境是Tidb有这个问题，现在测试用的Sqlite，问题依旧存在，应该与数据库无关，且SqlSugar已是最新版本
            try
            {
                //long没有问题
                await Db.GetSimpleClient<Unit2sdsaTestDb>().UpdateSetColumnsTrueAsync(s => new Unit2sdsaTestDb
                {
                    Num = input.Num!.Value
                }, s => s.Id == 1);

                //enmu没有问题
                await Db.GetSimpleClient<Unit2sdsaTestDb>().UpdateSetColumnsTrueAsync(s => new Unit2sdsaTestDb
                {
                    My = input.My!.Value
                }, s => s.Id == 1);

                //bool报错
                await Db.GetSimpleClient<Unit2sdsaTestDb>().UpdateSetColumnsTrueAsync(s => new Unit2sdsaTestDb
                {
                    IsDeleted = input.IsDeleted.Value//这里没加，都要报错
                }, s => s.Id == 1);

                //bool报错
                await Db.GetSimpleClient<Unit2sdsaTestDb>().UpdateSetColumnsTrueAsync(s => new Unit2sdsaTestDb
                {
                    IsDeleted = input.IsDeleted!.Value //这里加上【!】
                }, s => s.Id == 1);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public class Dto
        {
            [Required(ErrorMessage = "我问的deepspeek，必填项验证方式就是加上Required特性，然后使用可空类型，以此触发模型验证")]
            public bool? IsDeleted { get; set; }
            [Required(ErrorMessage = "我问的deepspeek，必填项验证方式就是加上Required特性，然后使用可空类型，以此触发模型验证")]
            public long? Num { get; set; }
            [Required(ErrorMessage = "我问的deepspeek，必填项验证方式就是加上Required特性，然后使用可空类型，以此触发模型验证")]
            public MyType? My { get; set; }
        }
        public class Unit2sdsaTestDb
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

            public bool IsDeleted { get; set; }

            public long Num { get; set; }
            public MyType My { get; set; }
        }


        public enum MyType
        {
            A, B, C, D, E, F
        }

    }
}
