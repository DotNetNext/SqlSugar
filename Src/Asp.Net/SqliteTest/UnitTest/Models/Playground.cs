using SqlSugar;
using System.Security.Principal;

namespace Test.Model
{
    [SugarTable("PlaygroundA01")]
    public class Playground
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public int? Id { get; set; }

        [SugarColumn(ColumnName = "Name")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "PId")]
        public int? PId { get; set; }
    }
}
