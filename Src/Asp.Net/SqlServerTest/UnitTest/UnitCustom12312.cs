using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class UnitCustom12312
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;

            var sql = db.Queryable<Conflict>()
                           .LeftJoin<Department>((cf, d) => cf.DepartmentId == d.Id)
                .Where((cf, d) => cf.IsDeleted == 0 &&
                                 cf.Id == SqlFunc.Subqueryable<ConflictCirculation>().GroupBy(x => x.ConflictId).Select(x => x.ConflictId))
                .Select(cf => new Conflict() { Id = cf.Id.SelectAll() }).ToSqlString();
            if (!sql.Contains("[conflict_id]")) { throw new Exception("unit error"); }
 
        }
    }
    [SugarTable("conflict")]
    public class Conflict
    {
       
        [SugarColumn(ColumnName = "id", ColumnDescription = "Id主键", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "user_id")]
        public int? UserId { get; set; }

        [SugarColumn(ColumnName = "department_id")]
        public int? DepartmentId { get; set; }

        [SugarColumn(ColumnName = "code")]
        public string Code { get; set; }

        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "happen_date")]
        public DateTime? HappenDate { get; set; }

        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [SugarColumn(ColumnName = "is_deleted")]
        public int IsDeleted { get; set; }

        [SugarColumn(ColumnName = "process_status")]
        public int? ProcessStatus { get; set; }

        [SugarColumn(ColumnName = "mediation_status")]
        public int? MediationStatus { get; set; }

        [SugarColumn(ColumnName = "is_func")]
        public int IsFunc { get; set; }

        [SugarColumn(ColumnName = "func_type")]
        public int FuncType { get; set; }

        /// <summary>
        /// 所属部门Uuid
        /// </summary>
        [SugarColumn(ColumnName = "department_uuid")]
        public Guid DepartmentUuid { get; set; }

        /// <summary>
        /// 当事人
        /// </summary>
        [SugarColumn(ColumnName = "person_json")]
        public string PersonJson { get; set; }

    }
    [SugarTable("Department")]
    public class Department
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }
        [SugarColumn(ColumnName = "Level")]
        public int Level { get; set; }
        [SugarColumn(ColumnName = "ParentId")]
        public int ParentId { get; set; }
        [SugarColumn(ColumnName = "PlatformId")]
        public int? PlatformId { get; set; }
        [SugarColumn(ColumnName = "PlatformCode")]
        public string PlatformCode { get; set; }
        [SugarColumn(ColumnName = "MapData")]
        public string MapData { get; set; }
        [SugarColumn(ColumnName = "Sort")]
        public int Sort { get; set; }
        [SugarColumn(ColumnName = "Remark")]
        public string Remark { get; set; }
        [SugarColumn(ColumnName = "Createdate")]
        public DateTime Createdate { get; set; }
        [SugarColumn(ColumnName = "IsDeleted")]
        public int IsDeleted { get; set; }
        [SugarColumn(ColumnName = "Lat")]
        public decimal? Lat { get; set; }
        [SugarColumn(ColumnName = "Lng")]
        public decimal? Lng { get; set; }
        [SugarColumn(ColumnName = "UUID")]
        public Guid UUID { get; set; }
    }
    [SugarTable("conflict_circulation")]
    public class ConflictCirculation
    {
 
        [SugarColumn(ColumnName = "id", ColumnDescription = "Id主键", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "conflict_id")]
        public long ConflictId { get; set; }

        [SugarColumn(ColumnName = "is_deleted")]
        public int IsDeleted { get; set; } = 0;

        [SugarColumn(ColumnName = "create_date")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [SugarColumn(ColumnName = "update_date")]
        public DateTime? UpdateDate { get; set; } = DateTime.Now;

        [SugarColumn(ColumnName = "status")]
        public int? Status { get; set; }

        [SugarColumn(ColumnName = "is_func")]
        public bool? IsFunc { get; set; }
    }
}
