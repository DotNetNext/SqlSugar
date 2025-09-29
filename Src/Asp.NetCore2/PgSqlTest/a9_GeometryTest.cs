using NpgsqlTypes;
using SqlSugar;

namespace OrmTest;

public class _a9_GeometryTest
{
    public static void Init()
    {
        // Get a new database instance
        // 获取新的数据库实例
        var db = DbHelper.GetNewDb();

        // Create the database if it doesn't exist
        // 如果数据库不存在，则创建数据库
        db.DbMaintenance.CreateDatabase();

        // Initialize tables based on G1 entity class
        // 根据 G1 实体类初始化表
        db.CodeFirst.InitTables<Geometries>();

        //Prepare data
        //准备数据
        var points = new NpgsqlPoint[]
        {
            new NpgsqlPoint(0,-10),
            new NpgsqlPoint(7,-7),
            new NpgsqlPoint(10,0),
            new NpgsqlPoint(7,7),
            new NpgsqlPoint(0,10),
            new NpgsqlPoint(-7,7),
            new NpgsqlPoint(-10,0),
            new NpgsqlPoint(-7,-7),
        };

        //Insert
        //插入
        var id = db.Insertable(new Geometries
        {
            Box = new NpgsqlBox(5, 4, 0, 0),
            Circle = new NpgsqlCircle(4, 5, 3),
            Line = new NpgsqlLine(1, 2, 3),
            Lseg = new NpgsqlLSeg(1, 2, 3, 4),
            Path = new NpgsqlPath(points),
            Point = new NpgsqlPoint(0, 1),
            Polygon = new NpgsqlPolygon(points),
        }).ExecuteReturnIdentity();

        //Query
        //查询
        var geom = db.Queryable<Geometries>().InSingle(id);

        var container = db.Queryable<Geometries>()
            .Where($"@point <@ {nameof(Geometries.Polygon)}", new { point = new NpgsqlPoint(3, 4) })
            .First();

        var area = db.Queryable<Geometries>()
            .Select<double>($"area({nameof(Geometries.Circle)})")
            .First();

        var length = db.Queryable<Geometries>()
            .Select<double>($"@-@ {nameof(Geometries.Path)}")
            .First();

        var center = db.Queryable<Geometries>()
            .Select<NpgsqlPoint>($"@@ {nameof(Geometries.Box)}")
            .First();

        //Update
        //更新
        db.Updateable(geom).ExecuteCommand();

        //Delete
        //删除
        db.Deleteable(geom).ExecuteCommand();
    }
}

/// <summary>
/// Geometry entity class
/// 几何实体类
/// </summary>
public class Geometries
{
    /// <summary>
    /// ID (Primary Key)
    /// ID（主键）
    /// </summary>
    [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
    public int Id { get; set; }

    /// <summary>
    /// 矩形框
    /// </summary>
    public NpgsqlBox Box { get; set; }

    /// <summary>
    /// 圆
    /// </summary>
    public NpgsqlCircle Circle { get; set; }

    /// <summary>
    /// 线 Ax + By + C = 0
    /// </summary>
    public NpgsqlLine Line { get; set; }

    /// <summary>
    /// 线段
    /// </summary>
    public NpgsqlLSeg Lseg { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    public NpgsqlPath Path { get; set; }

    /// <summary>
    /// 坐标点
    /// </summary>
    public NpgsqlPoint Point { get; set; }

    /// <summary>
    /// 多边形
    /// </summary>
    public NpgsqlPolygon Polygon { get; set; }
}