using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    internal class UnitFilteradfa
    {
        public static void Init()
        {
            var _db = NewUnitTest.Db;
            _db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                IsAutoUpdateQueryFilter = true,
                IsAutoDeleteQueryFilter = true,
            };
            _db.QueryFilter.AddTableFilter<IDeleted>(it => it.IsDelete == false);

            _db.CodeFirst.InitTables(typeof(PostInfoEntity));//创建表

            //新增数据
            var postInfoEntity = new PostInfoEntity();
            postInfoEntity.Id = SnowFlakeSingle.Instance.NextId();
            postInfoEntity.PostCode = "测试的哦";

            _ =   _db.Insertable(postInfoEntity).ExecuteCommand ();

            //假删除数据

            _ =  _db.Deleteable(postInfoEntity).WhereColumns(postInfoEntity, e => e.Id).IsLogic().ExecuteCommand ();
            _ = _db.Deleteable(postInfoEntity).WhereColumns(postInfoEntity, e => e.Id).IsLogic()
                .ExecuteCommandAsync().GetAwaiter()
                .GetResult();
            Console.WriteLine("完成");
        }   
    }
    public interface IDeleted
    {
        /// <summary>
        /// 默认假删除
        /// </summary>
        //[FakeDelete(true)]  // 设置假删除的值
        bool IsDelete { get; set; }
    }
    [SugarTable("Employee_PostInfo_test")]
    public class PostInfoEntity : IDeleted
    {
        [SugarColumn(ColumnDescription = "岗位编码", Length = 255)]
        public string PostCode { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "岗位名称", Length = 255)]
        public string PostName { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "是否删除", DefaultValue = "0")]
        public virtual bool IsDelete { get; set; }

        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "自增主键")]
        public virtual long Id { get; set; }

        [SugarColumn(ColumnDescription = "上次修改时间", IsNullable = true)]
        public virtual DateTimeOffset? LastModificationTime { get; set; }

        [SugarColumn(ColumnDescription = "上次修改人ID", IsNullable = true)]
        public virtual long? LastModifierUserId { get; set; }

        [SugarColumn(ColumnDescription = "上次修改人名称", IsNullable = true)]
        public virtual string? LastModifierUserName { get; set; }

        [SugarColumn(ColumnDescription = "创建时间")]
        public virtual DateTimeOffset CreationTime { get; set; }

        [SugarColumn(ColumnDescription = "创建人Id", IsNullable = true)]
        public virtual long CreatorUserId { get; set; }

        [SugarColumn(ColumnDescription = "创建人名称", IsNullable = true)]
        public virtual string CreatorUserName { get; set; } = string.Empty;

        [SugarColumn(ColumnDescription = "备注", IsNullable = true, Length = 500)]
        public virtual string Remark { get; set; } = string.Empty;
    }
}
