using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PerformanceTest
{
    [Dapper.Contrib.Extensions.Table("Test")]
    public class Test
    {
        [Dapper.Contrib.Extensions.Key]
        [SqlSugar.SugarColumn(IsIdentity =true ,IsPrimaryKey =true)]
        public int Id { get; set; }
        public byte? F_Byte { get; set; }
        public Int16? F_Int16 { get; set; }
        public int? F_Int32 { get; set; }
        public long? F_Int64 { get; set; }
        public double? F_Double { get; set; }
        public float? F_Float { get; set; }
        public decimal? F_Decimal { get; set; }
        public bool? F_Bool { get; set; }
        public DateTime? F_DateTime { get; set; }
        public Guid? F_Guid { get; set; }
        public string F_String { get; set; }
    }

    public class EFContext : DbContext
    {
        public EFContext(string connectionString) : base(connectionString) {

        }
        public DbSet<Test> TestList { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>().ToTable("Test");
        }
    }
}
