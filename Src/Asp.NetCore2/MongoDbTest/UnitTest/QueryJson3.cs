
using SqlSugar;
namespace MongoDbTest
{
    public class QueryJson3
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.DbMaintenance.TruncateTable<NoticeEntity>();
            db.Insertable<NoticeEntity>(new NoticeEntity() { Id = 12312312L, NoticeTitle = "88888", noticeContentInfos = new List<NoticeContentInfo>() { new NoticeContentInfo { SubjectId = 1 } } }).ExecuteCommand();
            var da = db.Updateable<NoticeEntity>()
                .SetColumns(it => new NoticeEntity { NoticeTitle = "66666" }, true).Where(it => it.Id == 12312312L)
                .ExecuteCommand() > 0;
        }
    }
}
/// <summary>
/// 通知公告
/// </summary>
[SugarTable("NoticeInfoComponent")]
public class NoticeEntity
{
    [SugarColumn(ColumnName = "_id")]
    public long Id { get; set; }
    /// <summary>
    /// 通知公告标题
    /// </summary>
    public string NoticeTitle { get; set; } = null!;

    /// <summary>
    /// 通知公告内容
    /// </summary>
    [SugarColumn(IsJson = true)]
    public List<NoticeContentInfo> noticeContentInfos { get; set; } = null!;
}

public class NoticeContentInfo
{
    /// <summary>
    /// 模块
    /// </summary>
    public int SubjectId { get; set; }
}
